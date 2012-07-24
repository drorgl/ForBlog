/*
 NHibernate MsSql2005Dialect
 This dialect was written to better handle pagination subqueries
 June 11, 2008 - Dror Gluska
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using NHibernate.Dialect.Function;
using NHibernate.Type;
using NHibernate;
using NHibernate.SqlCommand;
using System.Data;

namespace NHibernate.Dialect
{
    public class MsSql2005Dialect : NHibernate.Dialect.MsSql2000Dialect
   {
      public MsSql2005Dialect()
      {
         RegisterColumnType(DbType.String, 1073741823, "NVARCHAR(MAX)");
         RegisterColumnType(DbType.AnsiString, 2147483647, "VARCHAR(MAX)");
         RegisterColumnType(DbType.Binary, 2147483647, "VARBINARY(MAX)");
      }

      private string[] getSortExpression(string orderby)
      {
          int parenthesis = 0;
          List<string> orderlist = new List<string>();
          int lastcomma = 0;

          int orderbylength = orderby.Length;

          string currentchar;

          for (int i = 0; i < orderbylength; i++)
          {
              currentchar = orderby.Substring(i, 1);
              if ((currentchar == ",") && (parenthesis == 0))
              {
                  orderlist.Add(orderby.Substring(lastcomma, i - lastcomma));
                  lastcomma = i + 1;
              }

              if (currentchar == "(")
                  parenthesis++;
              if (currentchar == ")")
                  parenthesis--;
          }

          orderlist.Add(orderby.Substring(lastcomma, orderby.Length - lastcomma));

          return orderlist.ToArray();
      }

      /// <summary>
      /// Add a <c>LIMIT</c> clause to the given SQL <c>SELECT</c>
      /// </summary>
      /// <param name="querySqlString">The <see cref="SqlString"/> to base the limit query off of.</param>
      /// <param name="offset">Offset of the first row to be returned by the query (zero-based)</param>
      /// <param name="last">Maximum number of rows to be returned by the query</param>
      /// <returns>A new <see cref="SqlString"/> with the <c>LIMIT</c> clause applied.</returns>
      /// <remarks>
      /// The <c>LIMIT</c> SQL will look like
      /// <code>
      ///
      /// SELECT TOP last * FROM (
      /// SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__ {sort direction 1} [, __hibernate_sort_expr_2__ {sort direction 2}, ...]) as row, query.* FROM (
      ///      {original select query part}, {sort field 1} as __hibernate_sort_expr_1__ [, {sort field 2} as __hibernate_sort_expr_2__, ...]
      ///      {remainder of original query minus the order by clause}
      /// ) query
      /// ) page WHERE page.row > offset
      ///
      /// </code>
      /// </remarks>
      public override SqlString GetLimitString(SqlString querySqlString, int offset, int last)
      {

          int parenthesis = 0;

          int fromIndex = 0;

          string querystring = querySqlString.ToString();

          string currentchar;

          int querystringlength = querystring.Length;

          for (int i = 0; i < querystringlength; i++)
          {
              currentchar = querystring.Substring(i, 1);

              //if ( ((i + 6) <= querystring.Length) && (querystring.Substring(i, 6).ToLower() == " from ") && (parenthesis == 0))
              if (((i + 6) <= querystringlength) && (querystring.Substring(i, 6).IndexOf(" from ", StringComparison.InvariantCultureIgnoreCase) == 0) && (parenthesis == 0))
              {
                  fromIndex = i;
                  break;
              }
              if (currentchar == "(")
                  parenthesis++;
              if (currentchar == ")")
                  parenthesis--;

          }


         SqlString select = querySqlString.Substring(0, fromIndex);

         int orderIndex = 0;

         querystringlength = querystring.Length;

         for (int i = querystringlength - 1; i >= 0; i--)
         {
             currentchar = querystring.Substring(i, 1);
             if (((i + 10) <= querystringlength) && (querystring.Substring(i, 10).IndexOf(" order by ", StringComparison.InvariantCultureIgnoreCase) == 0) && (parenthesis == 0))
             {
                 orderIndex = i;
                 break;
             }
             if (currentchar == "(")
                 parenthesis++;
             if (currentchar == ")")
                 parenthesis--;

         }


         SqlString from;
         string[] sortExpressions;
         if (orderIndex > 0)
         {
            from = querySqlString.Substring(fromIndex, orderIndex - fromIndex).Trim();
            string orderBy = querySqlString.Substring(orderIndex).ToString().Trim();
            //sortExpressions = orderBy.Substring(9).Split(',');
            sortExpressions = getSortExpression(orderBy.Substring(9));
         }
         else
         {
            from = querySqlString.Substring(fromIndex).Trim();
            // Use dummy sort to avoid errors
            sortExpressions = new string[] { "CURRENT_TIMESTAMP" };
         }

         SqlStringBuilder result = new SqlStringBuilder()
            .Add("SELECT TOP ")
            .Add(last.ToString())
            .Add(" * FROM (SELECT ROW_NUMBER() OVER(ORDER BY ");

         for (int i = 1; i <= sortExpressions.Length; i++)
         {
            if (i > 1)
               result.Add(", ");

            result.Add("__hibernate_sort_expr_")
               .Add(i.ToString())
               .Add("__");

            if (sortExpressions[i - 1].Trim().EndsWith("desc", StringComparison.InvariantCultureIgnoreCase))
               result.Add(" DESC");
         }

         result.Add(") as row, query.* FROM (")
            .Add(select);

         for (int i = 1; i <= sortExpressions.Length; i++)
         {

            string sortExpression = sortExpressions[i - 1].Trim();
            if (sortExpression.EndsWith("desc", StringComparison.InvariantCultureIgnoreCase)) {
               sortExpression = sortExpression.Remove(sortExpression.Length - 4);
            }

            result.Add(", ")
               .Add(sortExpression)
               .Add(" as __hibernate_sort_expr_")
               .Add(i.ToString())
               .Add("__");
         }

         result.Add(" ")
            .Add(from)
            .Add(") query ) page WHERE page.row > ")
            .Add(offset.ToString());

         bool ordering = false;
         for (int i = 1; i <= sortExpressions.Length; i++)
         {
             if (ordering == false)
             {
                 result.Add(" order by ");
                 ordering = true;
             }

             if (i > 1)
                 result.Add(", ");

             result.Add("__hibernate_sort_expr_")
                .Add(i.ToString())
                .Add("__");

             if (sortExpressions[i - 1].Trim().EndsWith("desc", StringComparison.InvariantCultureIgnoreCase))
                 result.Add(" DESC");
         }

         return result.ToSqlString();
      }

      /// <summary>
      /// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
      /// functionality.
      /// </summary>
      /// <value><c>true</c></value>
      public override bool SupportsLimit
      {
         get
         {
            return true;
         }
      }

      /// <summary>
      /// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
      /// functionality with an offset.
      /// </summary>
      /// <value><c>true</c></value>
      public override bool SupportsLimitOffset
      {
         get
         {
            return true;
         }
      }

      /// <summary>
      /// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
      /// functionality with an offset.
      /// </summary>
      /// <value><c>false</c></value>
      public override bool UseMaxForLimit {
         get {
            return false;
         }
      }
   }
} 