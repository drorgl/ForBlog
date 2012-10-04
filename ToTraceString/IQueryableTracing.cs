using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Data.EntityClient;

namespace ToTraceString
{
    public static class IQueryableTracing
    {
        /// <summary>
        /// Retrieves the sql text command + simple representation of values
        /// </summary>
        public static string ToTraceString<T>(this IQueryable<T> query)
        {
            if (query != null)
            {
                ObjectQuery<T> objectQuery = query as ObjectQuery<T>;

                StringBuilder sb = new StringBuilder();
                sb.Append(objectQuery.ToTraceString());
                foreach (var p in objectQuery.Parameters)
                    sb.AppendFormat("\r\n{0} ({2}): {1}", p.Name, p.Value, p.ParameterType);
                return sb.ToString();
            }
            return "No Trace string to return";
        }

        /// <summary>
        /// Retrieves SQL text command in sp_executesql ready for execution.
        /// </summary>
        public static string ToSqlString<T>(this IQueryable<T> query)
        {
            if (query == null)
                return string.Empty;

            //get query
            ObjectQuery<T> objectQuery = query as ObjectQuery<T>;
            var sqlquery = objectQuery.ToTraceString();


            //if there are parameters, first add parameter types, then the parameter values.
            if (objectQuery.Parameters.Count() > 0)
            {
                //create a list of parameters with sql type
                StringBuilder retval = new StringBuilder();

                retval.Append("sp_executesql ");
                retval.AppendFormat("@statement = N'{0}'", sqlquery);

                //create sql parameter for each parameter
                List<string> parameters = new List<string>();
                List<string> values = new List<string>();
                foreach (var p in objectQuery.Parameters)
                {
                    //get sqldbtype
                    parameters.Add(string.Format("@{0} {1}", p.Name, GetDbType(p.Value)));
                    //get sql values for each parameter
                    values.Add(string.Format("@{0} = {1}", p.Name, GetDbValue(p.Value))); 
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

        /// <summary>
        /// Retrieve sql type from object value
        /// </summary>
        private static string GetDbType(object value)
        {
            var p1 = new System.Data.SqlClient.SqlParameter();
            p1.Value = value;

            if (p1.SqlDbType == SqlDbType.NVarChar)
                return "nvarchar(max)";
            if ((p1.SqlDbType == SqlDbType.Binary) || (p1.SqlDbType == SqlDbType.VarBinary))
                return "varbinary(max)";

            return p1.SqlDbType.ToString().ToLower();
        }

        /// <summary>
        /// Retrieve escaped sql value
        /// </summary>
        private static string GetDbValue(object obj)
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
