﻿/*
MapperStatistics - Statistics 
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

namespace FileStore.Mapper
{
    /// <summary>
    /// Statistics 
    /// </summary>
    public class MapperStatistics
    {
        /// <summary>
        /// Number of forced cleanups of mappings, due to maximum views/streams memory allocations
        /// </summary>
        public long NumberOfMappingsCleanup { get; internal set; }

        /// <summary>
        /// Number of File resizes
        /// <para>Important because for each resize all existing views/streams/MemoryMappedFile instances needs to be closed and reopened</para>
        /// </summary>
        public long NumberOfResizes { get; internal set; }

        /// <summary>
        /// Number of new Streams
        /// </summary>
        public long NumberOfStreamCreates { get; internal set; }

        /// <summary>
        /// Number of reused Streams
        /// </summary>
        public long NumberOfStreamReuse {get;internal set;}

        /// <summary>
        /// Number of new Views
        /// </summary>
        public long NumberOfViewCreates {get;internal set;}

        /// <summary>
        /// Number of reused Views
        /// </summary>
        public long NumberOfViewReuse { get; internal set; }

        /// <summary>
        /// Number of forced flushes
        /// </summary>
        public long NumberOfForcedFlushes { get; internal set; }

        /// <summary>
        /// Number of failed Stream/View allocations
        /// </summary>
        public long NumberOfFailedAllocations { get; internal set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("NumberOfMappingsCleanup: {0} ", NumberOfMappingsCleanup);
            sb.AppendFormat("NumberOfResizes: {0} ", NumberOfResizes);
            sb.AppendFormat("NumberOfStreamCreates: {0} ", NumberOfStreamCreates);
            sb.AppendFormat("NumberOfStreamReuse: {0} ", NumberOfStreamReuse);
            sb.AppendFormat("NumberOfViewCreates: {0} ", NumberOfViewCreates);
            sb.AppendFormat("NumberOfViewReuse: {0} ", NumberOfViewReuse);
            sb.AppendFormat("NumberOfForcedFlushes: {0} ", NumberOfForcedFlushes);
            sb.AppendFormat("NumberOfFailedAllocations: {0} ", NumberOfFailedAllocations);
            return sb.ToString();
        }
    }
}
