using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ewbi {

  /*

    Copyright (c) 2007 E. W. Bachtal, Inc.

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
    and associated documentation files (the "Software"), to deal in the Software without restriction, 
    including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
    and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
    subject to the following conditions:

      The above copyright notice and this permission notice shall be included in all copies or substantial 
      portions of the Software.

    The software is provided "as is", without warranty of any kind, express or implied, including but not 
    limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In 
    no event shall the authors or copyright holders be liable for any claim, damages or other liability, 
    whether in an action of contract, tort or otherwise, arising from, out of or in connection with the 
    software or the use or other dealings in the software. 

    --------------------------------------------------------------------------------------------------------

    FullTextSearch
    
    Parses and rewrites ad hoc SQL Server 2005 full-text search conditions into their valid normal form.  
    Always returns a valid full-text search condition suitable for use in a CONTAINS or CONTAINSTABLE query.  
    Exceptions can optionally be thrown when certain improper constructs are present.  If exceptions are not 
    thrown, the invalid constructs are replaced, removed, and/or massaged as needed to form a valid condition.
   
    See the following post for additional information:
   
    http://ewbi.blogs.com/develops/2007/05/normalizing_sql.html

    In the production version, eligible search terms are gathered into a collection of stemmed regex patterns 
    for matching in the resulting text for presentation, similar to the way Google highlights search terms in 
    its results.  The elided code uses UniqueList<> (http://ewbi.blogs.com/develops/2007/05/uniquelistt_for.html) 
    to gather unique terms using a SearchTerm class and stems terms and phrases using a C# implementation of the
    English Porter2 stemming algorithm (http://snowball.tartarus.org/otherlangs/english_cpp.txt) by Kamil Bartocha.  
    For simplicity, the code provided here simply gathers the terms into a List<string>, accessible as an array of 
    strings.  To learn more about these additional features, contact me at info@ewbi.com.
    
    v1.0 Original
    
  */

  [Flags]
  public enum FullTextSearchOptions {
    None = 0,
    Default = StemAll | TrimPrefixAll,
    StemAll = StemTerms | StemPhrases,
    TrimPrefixAll = TrimPrefixTerms | TrimPrefixPhrases,
    ThrowOnAll = ThrowOnUnbalancedParens | ThrowOnUnbalancedQuotes | ThrowOnInvalidNearUse,

    StemTerms = 1,                  // Apply FORMSOF(INFLECTIONAL) when not a prefix term or adjoining a NEAR.
    StemPhrases = 2,

    TrimPrefixTerms = 4,            // Trim prefix terms to first intra-word asterisk (leaving inner asterisks
    TrimPrefixPhrases = 8,          // will always result in no matches).

    ThrowOnUnbalancedParens = 128,  // Otherwise silently patches up and prevents under/overflow.
    ThrowOnUnbalancedQuotes = 256,  // Otherwise closes at end and assumes inner single instance quotes are intentional.
    ThrowOnInvalidNearUse = 512     // Otherwise silently switches the bad NEARs to ANDs.
  }

  public sealed class FullTextSearch {

    private string condition;
    private FullTextSearchOptions options;
    private string normalForm;
    private List<string> searchTerms;
    
    private FullTextSearch() {}
    
    public FullTextSearch(string condition) : this(condition, FullTextSearchOptions.Default) {}
    
    public FullTextSearch(string condition, FullTextSearchOptions options) {
    
      this.condition = condition;
      this.options = options;
      
      ConditionParser parser = new ConditionParser(condition, options);      
      
      normalForm = parser.RootExpression.ToString();

      searchTerms = new List<string>();
      
      foreach (ConditionExpression exp in parser.RootExpression) {
        if (exp.IsSubexpression) continue;
        if (exp.Term.Length == 0) continue;
        searchTerms.Add(exp.Term);
      }
      
    }
    
    public string Condition {
      get { return condition; }
    }
    
    public FullTextSearchOptions Options {
      get { return options; }
    }
    
    public string NormalForm {
      get { return normalForm; }
    }
    
    public string[] SearchTerms {
      get { return searchTerms.ToArray(); }
    }
        
    private sealed class ConditionParser {

      private FullTextSearchOptions options;
      
      private StringBuilder token;
      private ConditionOperator lastOp;
      private bool inQuotes;
      
      private ConditionExpression rootExpression;
      private ConditionExpression currentExpression;
      
      private ConditionParser() {}
      
      public ConditionParser(String condition, FullTextSearchOptions options) {

        ConditionStream stream = new ConditionStream(condition, options);    
      
        this.options = options;
      
        rootExpression = new ConditionExpression(options);
        currentExpression = rootExpression;

        Reset();

        while (stream.Read()) {
          if (ConditionOperator.IsSymbol(stream.Current)) {
            PutToken();
            SetToken(stream.Current);
            PutToken();
            continue;
          }
          switch (stream.Current) {
            case ' ': PutToken(); continue;
            case '(': PushExpression(); continue;
            case ')': PopExpression(); continue;
            case '"': 
              PutToken(); 
              inQuotes = true;
              SetToken(stream.ReadQuote());
              PutToken();
              inQuotes = false;
              continue;
          }
          AddToken(stream.Current);
        }
        PutToken();
        
        if (!object.ReferenceEquals(rootExpression, currentExpression)) {
          if ((options & FullTextSearchOptions.ThrowOnUnbalancedParens) != 0) {
            throw new InvalidOperationException("Unbalanced parentheses.");
          }
        }
        
      }
      
      public ConditionExpression RootExpression {
        get { return rootExpression; }      
      }

      private void Reset() {
        ResetToken();
        lastOp = ConditionOperator.And;
      }
      
      private void ResetToken() {
        token = new StringBuilder();
      }
    
      private void PushExpression() {
        PutToken();
        currentExpression = currentExpression.AddSubexpression(lastOp);
      }

      private void PopExpression() {
        PutToken();
        if (currentExpression.IsRoot) {
          if ((options & FullTextSearchOptions.ThrowOnUnbalancedParens) != 0) {
            throw new InvalidOperationException("Unbalanced parentheses.");
          }
        } else {
          currentExpression = currentExpression.Parent;
        }
        Reset();
      }
      
      private void AddToken(char c) {
        token.Append(c);
      }

      private void SetToken(char c) {
        SetToken(c.ToString());
      }

      private void SetToken(string s) {
        token = new StringBuilder(s);
      }
      
      private void PutToken() {
      
        // Check to see if the token is an operator.

        if (!inQuotes && ConditionOperator.TryParse(token.ToString(), ref lastOp)) {
          ResetToken();
          return;
        }
        
        // Not an operator, so it's a term.

        string term = token.ToString();
        if (inQuotes) {
          term = Regex.Replace(term.Trim(), @"[ ]{2,}", " ");
        }        
        if ((term.Length == 0) && !inQuotes) return;
        
        currentExpression.AddTerm(lastOp, term);
        
        Reset();
        
      }
      
    }

    private sealed class ConditionStream {
    
      private FullTextSearchOptions options;
      
      private string condition;
      private int index;

      private ConditionStream() {}
      
      public ConditionStream(string condition, FullTextSearchOptions options) {
        this.options = options;
        this.condition = Regex.Replace((condition ?? String.Empty), @"\x09|\x0D|\x0A|[\x01-\x08]|\x10|[\x0B-\x0C]|[\x0E-\x1F]", " ");
        index = -1;
      }
      
      public char Current {
        get { return (Eoq() || Boq()) ? (char) 0 : condition[index]; }
      }
      
      public bool Read() {
        index++;
        if (Eoq()) return false;          
        return true;
      }
      
      public string ReadQuote() {
        StringBuilder sb = new StringBuilder();
        while (Read()) {
          if (Current.Equals('"')) {
            if ((index + 1) == condition.Length) {
              index = condition.Length;
              return sb.ToString();
            }
            char peek = condition[index + 1];
            if ((peek == ' ') || (peek == ')') || (peek == '(') || (ConditionOperator.IsSymbol(peek))) {
              return sb.ToString();
            }
            if (peek == '"') {
              index += 1;
            } else {
              if ((options & FullTextSearchOptions.ThrowOnUnbalancedQuotes) != 0) {
                return sb.ToString();
              }
            }
          }
          sb.Append(Current);
        }
        if ((options & FullTextSearchOptions.ThrowOnUnbalancedQuotes) != 0) {
          throw new InvalidOperationException("Unbalanced quotes.");
        }
        return sb.ToString();
      }

      private bool Boq() {
        return (index < 0);
      }

      private bool Eoq() {
        return (index >= condition.Length);
      }
      
    }

    private sealed class ConditionExpression : IEnumerable<ConditionExpression> {

      private FullTextSearchOptions options;
      private int index;
      private ConditionExpression parent;
      private ConditionOperator op;
      private string term;
      private List<ConditionExpression> subexpressions;
      private bool isTerm;
      private bool isPhrase;
      private bool isPrefix;
      
      private ConditionExpression() {
        term = String.Empty;
        subexpressions = new List<ConditionExpression>();
      }
      
      public ConditionExpression(FullTextSearchOptions options) : this() {
        this.options = options;
      }
      
      private ConditionExpression(ConditionExpression parent, ConditionOperator op) : this(parent.options) {
        index = parent.subexpressions.Count;
        this.parent = parent;
        this.op = op;
      }

      private ConditionExpression(ConditionExpression parent, ConditionOperator op, string term) : this(parent, op) {
      
        this.term = term;
        
        isTerm = true;
        
        isPhrase = (term.IndexOf(' ') != -1);        
        int prefixIndex = term.IndexOf('*');        
        isPrefix = (prefixIndex != -1);        
        
        if (!isPrefix) return;        
        
        if (!isPhrase) {
          if ((options & FullTextSearchOptions.TrimPrefixTerms) == 0) return;
          if (prefixIndex == (term.Length - 1)) return;
          this.term = (prefixIndex == 0) ? "" : term.Remove(prefixIndex + 1);
          return;
        }
        
        if ((options & FullTextSearchOptions.TrimPrefixPhrases) == 0) return;
        term = Regex.Replace(term, @"(\*[^ ]+)|(\*)", "");
        term = Regex.Replace(term.Trim(), @"[ ]{2,}", " ");
        this.term = term + "*";
        
      }

      public ConditionExpression Parent {
        get { return parent; }
      }
      
      public bool IsRoot {
        get { return (parent == null); }
      }
      
      public bool IsLastSubexpression {
        get { return (IsRoot || (!IsRoot && (index == (parent.subexpressions.Count - 1)))); }
      }
      
      public ConditionExpression NextSubexpression {
        get { return (!IsLastSubexpression) ? parent.subexpressions[index + 1] : null; }
      }
      
      public ConditionOperator Operator {
        get { return op; }
      }
      
      public bool IsTerm {
        get { return isTerm; }
      }
      
      public bool TermIsPhrase {
        get { return isPhrase; }
      }

      public bool TermIsPrefix {
        get { return isPrefix; }
      }
      
      public bool IsSubexpression {
        get { return !isTerm; }
      }
      
      public bool HasSubexpressions {
        get { return subexpressions.Count > 0; }
      }
      
      public ConditionExpression LastSubexpression {
        get { return (HasSubexpressions) ? subexpressions[subexpressions.Count - 1] : null; }
      }

      public ConditionExpression AddSubexpression(ConditionOperator op) {

        ConditionOperator newOp = op;
        if (op == ConditionOperator.Near) {
          if ((options & FullTextSearchOptions.ThrowOnInvalidNearUse) != 0) {
            throw new InvalidOperationException("Invalid near operator before subexpression.");
          }
          newOp = ConditionOperator.And;
        }
      
        ConditionExpression exp = new ConditionExpression(this, newOp);

        subexpressions.Add(exp);

        return exp;

      }
            
      public void AddTerm(ConditionOperator op, string term) {
      
        if (!HasSubexpressions) {
          op = ConditionOperator.And;
        } else {
          if (op == ConditionOperator.Near) {
            if (LastSubexpression.HasSubexpressions) {
              if ((options & FullTextSearchOptions.ThrowOnInvalidNearUse) != 0) {
                throw new InvalidOperationException("Invalid near operator after subexpression.");
              }
              op = ConditionOperator.And;
            }
          }
        }
        
        ConditionExpression exp = new ConditionExpression(this, op, term);
        
        subexpressions.Add(exp);
        
      }

      public string Term {
        get { return term; }
      }

      public override string ToString() {
      
        StringBuilder sb = new StringBuilder();
        
        if (IsTerm) {
        
          bool doStem = DoStem();
        
          if (doStem) sb.Append("formsof(inflectional, ");
          
          sb.Append("\"");
          sb.Append(term.Replace("\"", "\"\""));
          sb.Append("\"");
          
          if (doStem) sb.Append(")");
          
        } else {
        
          if (!IsRoot) sb.Append("(");
          
          if (!HasSubexpressions) {
            sb.Append("\"\"");  // Want to avoid 'Null or empty full-text predicate' exception.
          } else {
            for (int i = 0; i < subexpressions.Count; i++) {
              ConditionExpression exp = subexpressions[i];
              if (i > 0) {
                sb.Append(" ");
                sb.Append(exp.op.ToString());
                sb.Append(" ");
              }
              sb.Append(exp.ToString());
            }
          }
          
          if (!IsRoot) sb.Append(")");
          
        }
        
        return sb.ToString();
        
      }
      
      private bool DoStem() {

        if (IsSubexpression) return false;
        if (Term.Length < 2) return false;
        if (TermIsPrefix) return false;
        if ((!TermIsPhrase && ((options & FullTextSearchOptions.StemTerms) == 0)) || (TermIsPhrase && ((options & FullTextSearchOptions.StemPhrases) == 0))) return false;
        if (op == ConditionOperator.Near) return false;
        if (!IsLastSubexpression && (NextSubexpression.op == ConditionOperator.Near)) return false;
        
        return true;
      
      }

      IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
      }

      public IEnumerator<ConditionExpression> GetEnumerator() {
        foreach (ConditionExpression exp in subexpressions) {
          yield return exp;
          if (exp.HasSubexpressions) {
            foreach (ConditionExpression exp2 in exp) {
              yield return exp2;
            }
          }
        }
      }

    }

    private struct ConditionOperator {
    
      private const char And1Symbol    = '&';
      private const char And2Symbol    = '+';
      private const char And3Symbol    = ',';
      private const char And4Symbol    = ';';
      private const char AndNot1Symbol = '-';
      private const char AndNot2Symbol = '!';
      private const char OrSymbol      = '|';
      private const char NearSymbol    = '~';

      private const int opAnd    = 0;
      private const int opAndNot = 1;
      private const int opOr     = 2;
      private const int opNear   = 3;

      public static ConditionOperator And    = new ConditionOperator(opAnd);
      public static ConditionOperator AndNot = new ConditionOperator(opAndNot);
      public static ConditionOperator Or     = new ConditionOperator(opOr);
      public static ConditionOperator Near   = new ConditionOperator(opNear);

      private int value;

      private ConditionOperator(int value) {
        this.value = value;
      }

      public override string ToString() {
        switch (value) {
          case opAndNot: return "and not";
          case opOr:     return "or";
          case opNear:   return "near";
          default: 
            return "and";
        }
      }
      
      public static bool IsSymbol(char symbol) {
        switch (symbol) {
          case And1Symbol:    return true;
          case And2Symbol:    return true;
          case And3Symbol:    return true;
          case And4Symbol:    return true;
          case AndNot1Symbol: return true;
          case AndNot2Symbol: return true;
          case OrSymbol:      return true;
          case NearSymbol:    return true;
        }
        return false;
      }

      public static bool TryParse(string s, ref ConditionOperator op) {

        if (s.Length == 1) {
          switch (s[0]) {
            case And1Symbol: goto case And4Symbol;
            case And2Symbol: goto case And4Symbol;
            case And3Symbol: goto case And4Symbol;
            case And4Symbol: 
              op = ConditionOperator.And; return true;
            case AndNot1Symbol: 
              op = ConditionOperator.AndNot; return true;
            case AndNot2Symbol:
              if (op != ConditionOperator.And) return false;
              op = ConditionOperator.AndNot;
              return true;
            case OrSymbol:      
              op = ConditionOperator.Or; return true;
            case NearSymbol:    
              op = ConditionOperator.Near; return true;
          }
          return false;
        }

        if (s.Equals(ConditionOperator.And.ToString(), StringComparison.OrdinalIgnoreCase)) {
          op = ConditionOperator.And;
          return true;
        }
        if (s.Equals("not", StringComparison.OrdinalIgnoreCase) && (op == ConditionOperator.And)) {
          op = ConditionOperator.AndNot;
          return true;
        }
        if (s.Equals(ConditionOperator.Or.ToString(), StringComparison.OrdinalIgnoreCase)) {
          op = ConditionOperator.Or;
          return true;
        }
        if (s.Equals(ConditionOperator.Near.ToString(), StringComparison.OrdinalIgnoreCase)) {
          op = ConditionOperator.Near;
          return true;
        }

        return false;

      }

      public static bool operator ==(ConditionOperator obj1, ConditionOperator obj2) {
        return obj1.Equals(obj2);
      }
      public static bool operator !=(ConditionOperator obj1, ConditionOperator obj2) {
        return !obj1.Equals(obj2);
      }
      public override bool Equals(object obj) {
        return (obj is ConditionOperator) && (Equals((ConditionOperator) obj));
      }
      private bool Equals(ConditionOperator obj) {
        return (value == obj.value);
      }
      public override int GetHashCode() {
        return value.GetHashCode();
      }

    }
  
  }
  
}