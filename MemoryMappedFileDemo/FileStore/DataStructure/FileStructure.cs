﻿/*
FileStructure - Contains the File Structure for all Memory Mapped Files
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
    /// Contains the File Structure for all Memory Mapped Files
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 19, CharSet = CharSet.Unicode)]
    struct FileStructure
    {
        /// <summary>
        /// Structure Length
        /// <remarks>Saves a call to Marshal</remarks>
        /// </summary>
        public const int StructureLength = 19;

        //Signature is split to individual bytes for speed

        [FieldOffset(0)]
        public byte SignatureA;
        [FieldOffset(1)]
        public byte SignatureB;
        [FieldOffset(2)]
        public byte SignatureC;

        /// <summary>
        /// Last Written Byte
        /// </summary>
        [FieldOffset(3)]
        public Int64 LastByte;

        /// <summary>
        /// Physical File Size
        /// </summary>
        [FieldOffset(11)]
        public Int64 FileSize;

        
    }
}
