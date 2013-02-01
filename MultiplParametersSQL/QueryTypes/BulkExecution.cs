using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace MultipleParametersSQL.QueryTypes
{
    /// <summary>
    /// Bulk Execution Demo
    /// </summary>
    class BulkExecution : AbstractQueryExecution
    {
        public override void Select(bool display)
        {
            //Does not implement select - irrelevant
        }

        public override void Merge(bool display)
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();

                    var rows = new List<DataRow>();
                    foreach (DataRow dr in this.MergeRecords(this.BulkOperationsCount).Rows)
                        rows.Add(dr);

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default,null))
                    {
                        bulkCopy.BatchSize = this.BulkOperationsCount;
                        bulkCopy.DestinationTableName = "Entities";
                        bulkCopy.WriteToServer(rows.ToArray());
                    }
            }
        }

       
    }
}
