﻿/*
RenderSQL - Converts SqlCommand to raw SQL with sp_executesql
Copyright (c) 2012 Dror Gluska
	
This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public License
(LGPL) as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.
The terms of redistributing and/or modifying this software also
include exceptions to the LGPL that facilitate static linking.
 	
This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.
 	
You should have received a copy of the GNU Lesser General Public License
along with this library; if not, write to Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA

  * taken from: http://uhurumkate.blogspot.com/2012/09/iqueryable-to-sql.html
  
Change log:
2012-01-04 - Initial version
 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Microsoft.SqlServer.Server;

namespace MultipleParametersSQL
{
    /// <summary>
    /// Converts SqlCommand to raw SQL with sp_executesql
    /// </summary>
    public class RenderSQL
    {
        private const string structured_param_name = "@s_p_";

        /// <summary>
        /// Generates a sp_executesql query
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string GenerateQuery(SqlCommand command)
        {
            var sqlquery = command.CommandText;

            //if there are parameters, first add parameter types, then the parameter values.
            if (command.Parameters.Count > 0)
            {
                //create a list of parameters with sql type
                StringBuilder retval = new StringBuilder();

                retval.Append(GenerateTVF(command));
                retval.Append("\r\n");

                retval.Append("exec sp_executesql ");
                retval.AppendFormat("@statement = N'{0}'", sqlquery.Replace("'","''"));

                //create sql parameter for each parameter
                List<string> parameters = new List<string>();
                List<string> values = new List<string>();
                for (var i = 0; i< command.Parameters.Count;i++)
                {
                    SqlParameter p = command.Parameters[i];
                    
                    if (!string.IsNullOrEmpty(p.TypeName))
                    {
                        //get custom type
                        parameters.Add(string.Format("{0} {1} readonly", p.ParameterName, p.TypeName));

                        //get sql values for each parameter
                        values.Add(string.Format("{0} = {1}", p.ParameterName, structured_param_name + i));
                    }
                    else
                    {
                        //get sqldbtype
                        parameters.Add(string.Format("{0} {1}", p.ParameterName, GetDbType(p)));

                        //get sql values for each parameter
                        values.Add(string.Format("{0} = {1}", p.ParameterName, GetDbValue(p.Value)));
                    }
                    
                }

                retval.AppendFormat(", @parameters = N'{0}'", string.Join(", ", parameters.ToArray()));
                retval.Append(", ");
                retval.Append(string.Join(", ", values.ToArray()));

                return retval.ToString();
            }
            else
            {
                return sqlquery;
            }
        }

        private static string GenerateTVF(SqlCommand command)
        {
            StringBuilder sb = new StringBuilder();
            for(var i = 0; i < command.Parameters.Count;i++)
            {
                SqlParameter p = command.Parameters[i];

                if (p.SqlDbType == SqlDbType.Structured)
                {
                    if (p.Value != null)
                    {
                        var paramName = structured_param_name + i;
                        sb.AppendFormat("declare {0} {1};", paramName, p.TypeName);
                        if (p.Value is IEnumerable<SqlDataRecord>)
                            sb.Append(GenerateTVFSqlDataRecord(paramName, p.Value as IEnumerable<SqlDataRecord>));
                        else if (p.Value is SqlDataReader)
                            sb.Append(GenerateTVFDataReader(paramName, p.Value as SqlDataReader));
                        else if (p.Value is DataTable)
                            sb.Append(GenerateTVFDataTable(paramName, p.Value as DataTable));
                    }
                }
            }

            return sb.ToString();
        }


        private static string ConcatInvoker(int count, Func<int, string> iteratorFunc)
        {
            return string.Join(",", Enumerable.Range(0, count).Select(i => iteratorFunc(i)).ToArray());
        }

        private static string GenerateTVFSqlDataRecord(string paramName, IEnumerable<SqlDataRecord> records)
        {
            StringBuilder sb = new StringBuilder();

            string fields = null;

            int blockcounter = -1;

            foreach (var r in records)
            {
                //if no more items left in block or if initial, restart block
                if ((blockcounter == 0) || (blockcounter == -1))
                {
                    sb.Append(";Insert into ");
                    sb.Append(paramName);
                    sb.Append("(");

                    if (fields == null)
                        fields = ConcatInvoker(r.FieldCount, i => r.GetSqlMetaData(i).Name);

                    sb.Append(fields);

                    sb.Append(") values ");
                    blockcounter = 999;

                    sb.Append("(");
                    sb.Append(ConcatInvoker(r.FieldCount, i => GetDbValue(r[i])));
                    sb.Append(")");

                    continue;
                }

                sb.Append(",(");
                sb.Append(ConcatInvoker(r.FieldCount, i => GetDbValue(r[i])));
                sb.Append(")");
                blockcounter--;
            }

            sb.Append(";");
            return sb.ToString();

        }

        private static string GenerateTVFDataReader(string paramName, SqlDataReader reader)
        {
            StringBuilder sb = new StringBuilder();

            string fields = null;

            int blockcounter = -1;

            while (reader.Read() == true)
            {
                //if no more items left in block or if initial, restart block
                if ((blockcounter == 0) || (blockcounter == -1))
                {
                    sb.Append(";Insert into ");
                    sb.Append(paramName);
                    sb.Append("(");

                    if (fields == null)
                        fields = ConcatInvoker(reader.FieldCount, i => reader.GetName(i));

                    sb.Append(fields);

                    sb.Append(") values ");
                    blockcounter = 999;

                    sb.Append("(");
                    sb.Append(ConcatInvoker(reader.FieldCount, i => GetDbValue(reader[i])));
                    sb.Append(")");

                    continue;
                }

                sb.Append(",(");
                sb.Append(ConcatInvoker(reader.FieldCount, i => GetDbValue(reader[i])));
                sb.Append(")");
                blockcounter--;
            }
            sb.Append(";");
            return sb.ToString();
        }

        private static string GenerateTVFDataTable(string paramName, DataTable dataTable)
        {
            StringBuilder sb = new StringBuilder();

            string fields = null;

            int blockcounter = -1;

            foreach (DataRow r in dataTable.Rows)
            {
                //if no more items left in block or if initial, restart block
                if ((blockcounter == 0) || (blockcounter == -1))
                {
                    sb.Append(";Insert into ");
                    sb.Append(paramName);
                    sb.Append("(");

                    if (fields == null)
                        fields = ConcatInvoker(dataTable.Columns.Count, i => dataTable.Columns[i].ColumnName);

                    sb.Append(fields);

                    sb.Append(") values ");
                    blockcounter = 999;

                    sb.Append("(");
                    sb.Append(ConcatInvoker(dataTable.Columns.Count, i => GetDbValue(r[i])));
                    sb.Append(")");

                    continue;
                }

                sb.Append(",(");
                sb.Append(ConcatInvoker(dataTable.Columns.Count, i => GetDbValue(r[i])));
                sb.Append(")");
                blockcounter--;
            }
            sb.Append(";");
            return sb.ToString();
        }

        /// <summary>
        /// Retrieve sql type from object value
        /// </summary>
        private static string GetDbType(SqlParameter param)
        {
            if (param.SqlDbType == SqlDbType.NVarChar)
                return "nvarchar(max)";
            if ((param.SqlDbType == SqlDbType.Binary) || (param.SqlDbType == SqlDbType.VarBinary))
                return "varbinary(max)";
            if (param.SqlDbType == SqlDbType.Xml)
                return "xml";

            return param.SqlDbType.ToString().ToLower();
        }

        /// <summary>
        /// Retrieve escaped sql value
        /// </summary>
        public static string GetDbValue(object obj)
        {
            if (obj == null)
                return "null";

            if (obj is string)
                return "'" + obj.ToString().Replace("'", "''") + "'";

            if (obj is Guid)
                return "'" + obj.ToString() + "'";

            if (obj is DateTime)
                return "N'" + ((DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss.ff") + "'";

            if (obj is bool)
                return (((bool)obj) == true) ? "1" : "0";

            if (obj is byte[])
                return "0x" + BitConverter.ToString((byte[])obj).Replace("-", "") + "";

            if (obj is int)
                return ((int)obj).ToString();

            if (obj is decimal)
                return ((decimal)obj).ToString();

            if (obj is long)
                return ((long)obj).ToString();

            if (obj is double)
                return ((double)obj).ToString();

            if (obj is Single)
                return ((Single)obj).ToString();

            throw new NotImplementedException(obj.GetType().Name + " not implemented");
        }

    }
}
