/*
 CSVParser - for reading CSV files.
 
 This class was made as a quick example for showing explicit operators, its in no way complete or even working on real CSV files.
 
 Changelog:
 2012-06-01 - Dror Gluska - Initial Version
 
 
 
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace CSVParser
{
    /// <summary>
    /// CSV Parser
    /// </summary>
    public class CSVParser
    {
        /// <summary>
        /// Parsed Cell
        /// </summary>
        public class Cell
        {
            /// <summary>
            /// Column Number of cell
            /// </summary>
            public int ColumnNumber;

            /// <summary>
            /// Text value of cell
            /// </summary>
            public string Value;

            /// <summary>
            /// Explicit cast to int
            /// </summary>
            /// <param name="cell"></param>
            /// <returns></returns>
            public static explicit operator int(Cell cell)
            {
                return Convert.ToInt32(cell.Value);
            }
            
            /// <summary>
            /// Explicit cast to int?
            /// </summary>
            /// <param name="cell"></param>
            /// <returns></returns>
            public static explicit operator int?(Cell cell)
            {
                int retval;
                if (int.TryParse(cell.Value, out retval))
                    return retval;

                return null;
            }

            /// <summary>
            /// Explicit cast to string
            /// </summary>
            /// <param name="cell"></param>
            /// <returns></returns>
            public static explicit operator string(Cell cell)
            {
                return cell.Value;
            }

            /// <summary>
            /// Explicit cast to DateTime
            /// </summary>
            /// <param name="cell"></param>
            /// <returns></returns>
            public static explicit operator DateTime(Cell cell)
            {
                return DateTime.Parse(cell.Value);
            }

            /// <summary>
            /// Explicit cast to DateTime?
            /// </summary>
            /// <param name="cell"></param>
            /// <returns></returns>
            public static explicit operator DateTime?(Cell cell)
            {
                DateTime retval;
                if (DateTime.TryParse(cell.Value, out retval))
                    return retval;

                return null;
            }
        }

        /// <summary>
        /// Parsed Row
        /// </summary>
        public class Row
        {
            public int RowNumber;
            public Cell[] Cell;
        }

        /// <summary>
        /// Parsed Rows
        /// </summary>
        public List<Row> Rows = new List<Row>();

        /// <summary>
        /// Instantiate from text file (csv)
        /// </summary>
        /// <param name="textfile"></param>
        public CSVParser(string textfile)
        {
            using (TextReader tr = File.OpenText(textfile))
            {

                var all = tr.ReadToEnd();

                bool inquotes = false;
                string cellvalue = "";

                List<Cell> cells = new List<Cell>();

                for (var ti = 0; ti < all.Length; ti++)
                {
                    if (all[ti] == '"')
                    {
                        if ((inquotes == false))
                        {
                            inquotes = true;
                        }
                        else
                            inquotes = false;
                    }

                    if ((all[ti] == ',') && (inquotes == false))
                    {
                        //new field
                        cells.Add(new Cell
                        {
                            ColumnNumber = cells.Count(),
                            Value = ParseCell( cellvalue)
                        });
                        cellvalue = "";
                    }else

                    if ((all[ti] == '\r') && (inquotes == false))
                    {
                        //new row
                        this.Rows.Add(new Row
                        {
                            Cell = cells.ToArray(),
                            RowNumber = this.Rows.Count()
                        });

                        cells.Clear();
                    }
                    else
                        cellvalue += all[ti];
                }

            }

        }

        /// <summary>
        /// Parse cell, remove extra characters and decode unicode characters
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ParseCell(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;    

            string retval = text;

            if (retval.StartsWith("\"") && retval.EndsWith("\""))
                retval = retval.Substring(1, retval.Length - 2);

            //from http://stackoverflow.com/questions/8558671/how-to-unescape-unicode-string-in-c-sharp
            retval = Regex.Replace(
                                    retval,
                                    @"\\[Uu]([0-9A-Fa-f]{4})",
                                    m => char.ToString(
                                        (char)ushort.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier)));

            return retval.Trim();
        }
    }
}
