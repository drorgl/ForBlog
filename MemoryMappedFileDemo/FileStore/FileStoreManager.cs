﻿/*
FileStoreManager - simple Document store-like
Copyright (c) 2013 Dror Gluska
	
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

Change log:
2013-03-15 - Initial version
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.MemoryMappedFiles;
using FileStore.DataStructure;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FileStore.Mapper;

namespace FileStore
{
    /// <summary>
    /// Simple document store-like library
    /// </summary>
    public class FileStoreManager : IDisposable
    {
        /// <summary>
        /// Data file MemoryMapper
        /// </summary>
        private MemoryMapper m_datamapper;

        /// <summary>
        /// Data Table file MemoryMapper
        /// </summary>
        private MemoryMapper m_datatablemapper;

        /// <summary>
        /// Data file header VirtualView
        /// </summary>
        private VirtualView m_dataheaderview;

        /// <summary>
        /// Data Table file header VirtualView
        /// </summary>
        private VirtualView m_datatableheaderview;

        /// <summary>
        /// Data file structure
        /// </summary>
        private FileStructure m_datafilestructure = new FileStructure();

        /// <summary>
        /// Data Table file structure
        /// </summary>
        private FileStructure m_datatablefilestructure = new FileStructure(); 

        /// <summary>
        /// Filename
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Cached List of Entries
        /// </summary>
        private List<LogicalEntry> m_allentries = null;

        /// <summary>
        /// Dictionary of entries
        /// </summary>
        private Dictionary<Guid, LogicalEntry> m_allentriesDictionary = null;

        /// <summary>
        /// ctor
        /// </summary>
        public FileStoreManager(string filename)
        {
            this.Filename = filename;
            this.m_datamapper = new MemoryMapper(this.DataFilename);
            this.m_datatablemapper = new MemoryMapper(this.DataTableFilename);

            this.m_dataheaderview = this.m_datamapper.CreateView(0, 512);
            this.m_datatableheaderview = this.m_datatablemapper.CreateView(0, 512);

            MaintainHeaders();
            GetEntries();
        }

        /// <summary>
        /// Permanently deletes data file and data table file
        /// </summary>
        public void Delete()
        {
            if (m_datamapper != null)
            {
                m_dataheaderview.Dispose();
                m_dataheaderview = null;

                m_datamapper.Dispose();
                m_datamapper = null;

                if (File.Exists(this.DataFilename))
                    File.Delete(this.DataFilename);
            }

            if (m_datatablemapper != null)
            {
                m_datatableheaderview.Dispose();
                m_datatableheaderview = null;

                m_datatablemapper.Dispose();
                m_datatablemapper = null;

                if (File.Exists(this.DataTableFilename))
                    File.Delete(this.DataTableFilename);

            }
        }

        /// <summary>
        /// Data Table filename
        /// </summary>
        private string DataTableFilename
        {
            get
            {
                return this.Filename + ".datatable";
            }
        }

        /// <summary>
        /// Data filename
        /// </summary>
        private string DataFilename
        {
            get
            {
                return this.Filename + ".data";
            }
        }

     /// <summary>
     /// Checks if file has the correct header
     /// </summary>
        private bool IsValidStructure(FileStructure filestructure)
        {
            //the reason I chose to compare individual bytes instead of a string or a char array is speed
            //it makes the code a little bit less readable, but the speed improvements are significant
            return ((filestructure.SignatureA == (byte)'M') &
                (filestructure.SignatureB == (byte)'M') &&
                (filestructure.SignatureC == (byte)'F'));
        }

        /// <summary>
        /// Maintains headers for files
        /// </summary>
        private void MaintainHeaders()
        {
            //maintain data file header
            {
                if (!IsValidStructure( m_datafilestructure))
                {
                    m_dataheaderview.Read(0, out m_datafilestructure);
                    if (!IsValidStructure(m_datafilestructure))
                    {
                        m_datafilestructure.SignatureA = (byte)'M';
                        m_datafilestructure.SignatureB = (byte)'M';
                        m_datafilestructure.SignatureC = (byte)'F';
                        m_datafilestructure.LastByte = FileStructure.StructureLength;
                    }

                }

                m_dataheaderview.Write(0, ref m_datafilestructure);
            }

            //maintain data table file structure 
            {
                if (!IsValidStructure( m_datatablefilestructure))
                {
                    m_datatableheaderview.Read(0, out m_datatablefilestructure);
                    if (!IsValidStructure(m_datatablefilestructure))
                    {
                        m_datatablefilestructure.SignatureA = (byte)'M';
                        m_datatablefilestructure.SignatureB = (byte)'M';
                        m_datatablefilestructure.SignatureC = (byte)'F';
                        m_datatablefilestructure.LastByte = FileStructure.StructureLength;
                    }

                }

                m_datatableheaderview.Write(0, ref m_datatablefilestructure);
            }

        }


        /// <summary>
        /// Saves an entry to data and data table
        /// </summary>
        /// <param name="Id">id of entry</param>
        /// <param name="obj">object to be saved</param>
        public void PushEntry(Guid Id, object obj)
        {
            //stream the object via serializer
            var ms = Serialize(obj);
            ms.Seek(0, SeekOrigin.Begin);

            long lastbyte = 0;
            var lastexpectedbyte = ms.Length;

            {
                lastbyte = m_datafilestructure.LastByte;

                using (var mmfstream = m_datamapper.CreateStream(lastbyte, lastexpectedbyte))
                {
                    ms.CopyTo(mmfstream);
                    mmfstream.Close();
                    m_datafilestructure.LastByte += lastexpectedbyte;
                }
            }

            {

                //create a logical entry with start/end of serialized object
                LogicalEntry le = new LogicalEntry();
                le.Id = Id;
                le.Length = ms.Length;
                le.Offset = lastbyte;

                //save entry
                using (var view = m_datatablemapper.CreateView(m_datatablefilestructure.LastByte, LogicalEntry.StructureLength))
                {
                    view.Write(0, ref le);
                    m_datatablefilestructure.LastByte += LogicalEntry.StructureLength;
                }

                m_allentries.Add(le);
                m_allentriesDictionary[le.Id] = le;
            }

            MaintainHeaders();
        }

        /// <summary>
        /// Serializes an object to MemoryStream
        /// </summary>
        private MemoryStream Serialize(object obj)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            return ms;
        }

        /// <summary>
        /// Deserializes a Stream to object of type T
        /// </summary>
        private T Deserialize<T>(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return (T)bf.Deserialize(stream);
        }

        /// <summary>
        /// Retrieven entry by id
        /// </summary>
        public T GetEntry<T>(Guid Id)
        {
            GetEntries();
            var entry = m_allentriesDictionary[Id];
            //var entry = GetEntries().FirstOrDefault(i => i.Id == Id);
            if (entry.Offset != 0)
            {
                using (var stream = m_datamapper.CreateStream(entry.Offset, entry.Length))
                {
                    return Deserialize<T>(stream);
                }
            }
            return default(T);
        }
        
        /// <summary>
        /// Get a list of all identities in data table
        /// </summary>
        public List<Guid> GetAllEntries()
        {
            return GetEntries().Select(i => i.Id).ToList();
        }

        /// <summary>
        /// Gets a list of all entries
        /// </summary>
        private List<LogicalEntry> GetEntries()
        {
            if (m_allentries == null)
            {
                List<LogicalEntry> entries = new List<LogicalEntry>();
                {
                    long location = 0;
                    long length = m_datatablefilestructure.LastByte;

                    using (var view = m_datatablemapper.CreateView(FileStructure.StructureLength, length))
                    {
                        while ((location + LogicalEntry.StructureLength) < length)
                        {
                            LogicalEntry entry = new LogicalEntry();
                            view.Read(location, out entry);
                            location += LogicalEntry.StructureLength;
                            entries.Add(entry);
                        }
                    }
                }
                m_allentries = entries;
                m_allentriesDictionary = m_allentries.ToDictionary(i => i.Id);
            }
            return m_allentries;
        }

        /// <summary>
        /// Get Statistics string for both data and datatable mappers
        /// </summary>
        public string GetStatistics()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Data:");
            sb.AppendLine(this.m_datamapper.Statistics.ToString());
            sb.AppendLine("Table:");
            sb.AppendLine(this.m_datatablemapper.Statistics.ToString());

            return sb.ToString();
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (m_dataheaderview != null)
            {
                m_dataheaderview.Dispose();
                m_dataheaderview = null;
            }

            if (m_datatableheaderview != null)
            {
                m_datatableheaderview.Dispose();
                m_datatableheaderview = null;
            }

            if (m_datamapper != null)
            {
                m_datamapper.Dispose();
                m_datamapper = null;
            }

            if (m_datatablemapper != null)
            {
                m_datatablemapper.Dispose();
                m_datatablemapper = null;
            }


        }

        #endregion
    }
}
