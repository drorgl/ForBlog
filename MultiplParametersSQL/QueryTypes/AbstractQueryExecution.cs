using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.SqlServer.Server;
using System.Data;

namespace MultipleParametersSQL.QueryTypes
{
    /// <summary>
    /// Abstract Execution Demo
    /// </summary>
    public abstract class AbstractQueryExecution
    {
        /// <summary>
        /// Executes a Select Query
        /// </summary>
        /// <param name="display">display results to console</param>
        public abstract void Select(bool display);

        /// <summary>
        /// Executes a Merge Query
        /// </summary>
        /// <param name="display">display results to console</param>
        public abstract void Merge(bool display);

        /// <summary>
        /// Cached Connection String
        /// </summary>
        private string m_connectionString = null;

        /// <summary>
        /// Retrieve Connection String from Config file
        /// </summary>
        protected string ConnectionString
        {
            get
            {
                if (m_connectionString == null)
                    m_connectionString = ConfigurationManager.ConnectionStrings["TestConnectionString"].ConnectionString;

                return m_connectionString;
            }
        }

        /// <summary>
        /// Number of Operations to perform in one batch
        /// </summary>
        public int BulkOperationsCount { get; set; }

        /// <summary>
        /// Cached Select Records
        /// </summary>
        private List<SqlDataRecord> m_selectrecords = null;

        /// <summary>
        /// Retrieves a list of Sequential Id records
        /// </summary>
        protected List<SqlDataRecord> SelectRecords
        {
            get
            {
                if (m_selectrecords == null)
                {
                    List<SqlDataRecord> records = new List<SqlDataRecord>();
                    var mdId = new SqlMetaData("Id", SqlDbType.Int);
                    for (var i = 1; i <= this.BulkOperationsCount; i++)
                    {
                        var sdr = new SqlDataRecord(mdId);
                        sdr.SetInt32(0, i);
                        records.Add(sdr);
                    }
                    this.m_selectrecords = records;
                }

                return this.m_selectrecords;
            }
        }

        /// <summary>
        /// Cached Merge Table Data
        /// </summary>
        private DataTable m_tabledata = null;

        /// <summary>
        /// Retrieves a List of Test Data to Merge into the Table
        /// </summary>
        private DataTable GetTableData
        {
            get
            {
                if (m_tabledata == null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("EntityId");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Phone");
                    dt.Columns.Add("Email");
                    dt.Columns.Add("Address");
                    dt.Columns.Add("City");
                    dt.Columns.Add("Zip");
                    dt.Columns.Add("State");
                    dt.Columns.Add("Country");
                    dt.Columns.Add("Birthdate");

                    dt.Rows.Add(1, "", "(376) 284-2559", "diam.lorem@Nunc.edu", "Ap #289-7599 Sem St.", "Scranton", "C5D 8E7", "Yukon", "Fiji", "1994-01-01");
                    dt.Rows.Add(2, "", "(339) 513-6796", "ornare@faucibuslectusa.com", "3150 Risus. Rd.", "Farrell", "N4H 7G4", "Nova Scotia", "Virgin Islands, U.S.", "1979-01-11");
                    dt.Rows.Add(3, "", "(752) 794-1610", "malesuada.fringilla@massa.ca", "647-7117 Convallis Road", "Newburyport", "G1S 9H1", "Newfoundland and Labrador	Brunei", "Darussalam", "1975-01-29");
                    dt.Rows.Add(4, "", "(204) 898-9609", "risus@magnis.edu", "Ap #580-4432 Ornare, Ave", "Commerce", "R2R 6M8", "Yukon", "United States", "1951-02-19");

                    this.m_tabledata = dt;
                }
                return this.m_tabledata;
            }
        }

        /// <summary>
        /// Cache for Merge Records by the number of records per batch
        /// </summary>
        private Dictionary<int, DataTable> _cacherecords = new Dictionary<int, DataTable>();

        /// <summary>
        /// Retrieves Test Records for a specific batch size
        /// </summary>
        protected DataTable MergeRecords(int count)
        {
            if (!_cacherecords.ContainsKey(count))
            {
                DataTable dt = new DataTable();
                foreach (DataColumn dc in this.GetTableData.Columns)
                {
                    dt.Columns.Add(dc.ColumnName);
                }

                for (var i = 0; i < count; i++)
                {
                    var dr = this.GetTableData.Rows[i % this.GetTableData.Rows.Count];

                    DataRow drnew = dt.NewRow();
                    for (var c = 0; c < this.GetTableData.Columns.Count; c++)
                    {
                        if ((c == 0) && (i >= this.GetTableData.Rows.Count))
                            drnew[c] = 0;
                        else
                            drnew[c] = dr[c];
                    }
                    dt.Rows.Add(drnew);
                }
                _cacherecords[count] = dt;
            }
            return _cacherecords[count];
        }


        /// <summary>
        /// Displays reader's contents to console.
        /// </summary>
        protected void DisplayReader(bool display, SqlDataReader reader)
        {
            if (display)
            {
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write("{0}\t", reader.GetName(i));
                }
                Console.WriteLine();
            }

            while (reader.Read() == true)
            {
                if (display)
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write("{0}\t", reader[i]);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
