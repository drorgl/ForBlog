﻿/*
MemoryMappedStreamContainer - Contains a list of MemoryMappedStreams for each Range/Stream
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

namespace FileStore.Mapper.Collections
{
    /// <summary>
    /// Contains a list of MemoryMappedStreams for each Range/Stream
    /// </summary>
    class MemoryMappedStreamContainer
    {
        /// <summary>
        /// Accessor for MemoryMappedFile object, for late stream creation
        /// </summary>
        internal MemoryMapper MemoryMapper { get; set; }

        /// <summary>
        /// The Range of the Accessor
        /// </summary>
        public Range<Int64> Range { get; set; }

        /// <summary>
        /// Lazy stream variable
        /// </summary>
        private MemoryMappedViewStream m_stream = null;

        /// <summary>
        /// The MemoryMappedFile Stream
        /// </summary>
        public MemoryMappedViewStream Stream
        {
            get
            {
                if (m_stream == null)
                {
                    if (MemoryMapper == null)
                        throw new ApplicationException("MemoryMappedFile was not supplied to the container");

                    //attempt to create stream 4 times before failing, clear unused memory between calls
                    for (var i = 0; i <= 3; i++)
                    {
                        m_stream = CreateStream((i==3));
                        if (m_stream == null)
                        {
                            this.MemoryMapper.ClearUnusedMappings();
                        }
                        else
                        {
                            break;
                        }

                    }
                    
                }
                return m_stream;
            }
        }

        /// <summary>
        /// Attempts to create a Stream
        /// </summary>
        private MemoryMappedViewStream CreateStream(bool throwException)
        {
            try
            {
                return MemoryMapper.m_MemoryMappedFile.CreateViewStream(this.Range.Minimum, this.Range.Maximum - this.Range.Minimum);
            }
            catch (System.IO.IOException)
            {
                this.MemoryMapper.Statistics.NumberOfFailedAllocations++;
                if (throwException)
                    throw;

                return null;
            }
        }

        /// <summary>
        /// List of VirtualStream
        /// </summary>
        internal List<VirtualStreamContainer> MappedStreams { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        internal MemoryMappedStreamContainer()
        {
            this.MappedStreams = new List<VirtualStreamContainer>();
        }

        /// <summary>
        /// Internal method for clearing the stream from memory
        /// </summary>
        internal void ClearStream()
        {
            if (m_stream != null)
            {
                m_stream.Dispose();
                m_stream = null;
            }

        }
    }
}
