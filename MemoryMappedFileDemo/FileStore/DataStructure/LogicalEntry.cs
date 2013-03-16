﻿/*
LogicalEntry - Logical File Entry
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
using System.Runtime.InteropServices;

namespace FileStore.DataStructure
{
    /// <summary>
    /// Logical File Entry
    /// </summary>
    [StructLayout(LayoutKind.Explicit,Pack=1,Size=32, CharSet=CharSet.Unicode)]
    struct LogicalEntry
    {
        /// <summary>
        /// Structure Length
        /// <remarks>Saves a call to Marshal</remarks>
        /// </summary>
        public const int StructureLength = 32;

        /// <summary>
        /// The Id of the Entry
        /// </summary>
        [FieldOffset(0)]
        public Guid Id;

        /// <summary>
        /// Offset this entry is located in
        /// </summary>
        [FieldOffset(16)]
        public Int64 Offset;

        /// <summary>
        /// Length of this entry
        /// </summary>
        [FieldOffset(24)]
        public Int64 Length;
    }
}
