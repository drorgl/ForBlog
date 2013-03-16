﻿/*
VirtualView - Memory Mapped Virtual View
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
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using FileStore.Mapper.Collections;

namespace FileStore.Mapper
{
    /// <summary>
    /// Memory Mapped Virtual View
    /// </summary>
    public class VirtualView : IDisposable
    {
        private VirtualViewContainer m_viewaccessor;
        private long m_viewoffset;
        private long m_offset;
        private long m_capacity;

        internal VirtualView(VirtualViewContainer accessor, long view_offset, long offset, long capacity)
        {
            m_viewoffset = view_offset;
            m_viewaccessor = accessor;
            m_offset = offset;
            m_capacity = capacity;
        }

        public   bool CanRead { get { return m_viewaccessor.Container.View.CanRead; } }

        public  bool CanWrite { get { return m_viewaccessor.Container.View.CanWrite; } }

        public  long Capacity { get { return m_capacity; } }

        
        public  void Read<T>(long position, out T structure) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));

            if ((size + position) > m_capacity)
                throw new ArgumentOutOfRangeException("Either position is invalid or structure is too big");

            m_viewaccessor.Container.View.Read<T>(GetPosition(position), out structure);
        }

        
        public  int ReadArray<T>(long position, T[] array, int offset, int count) where T : struct
        {
            return m_viewaccessor.Container.View.ReadArray<T>(GetPosition(position), array, offset, count);
        }


        public  bool ReadBoolean(long position)
        {
            return m_viewaccessor.Container.View.ReadBoolean(GetPosition(position));
        }


        public  byte ReadByte(long position)
        {
            return m_viewaccessor.Container.View.ReadByte(GetPosition(position));
        }


        
        public  char ReadChar(long position)
        {
            return m_viewaccessor.Container.View.ReadChar(GetPosition(position));
        }


        
        public  decimal ReadDecimal(long position)
        {
            return m_viewaccessor.Container.View.ReadDecimal(GetPosition(position));
        }


        
        public  double ReadDouble(long position)
        {
            return m_viewaccessor.Container.View.ReadDouble(GetPosition(position));
        }


        
        public  short ReadInt16(long position)
        {
            return m_viewaccessor.Container.View.ReadInt16(GetPosition(position));
        }


        
        public  int ReadInt32(long position)
        {
            return m_viewaccessor.Container.View.ReadInt32(GetPosition(position));
        }


        
        public  long ReadInt64(long position)
        {
            return m_viewaccessor.Container.View.ReadInt64(GetPosition(position));
        }


        
        
        public  sbyte ReadSByte(long position)
        {
            return m_viewaccessor.Container.View.ReadSByte(GetPosition(position));
        }


        
        public  float ReadSingle(long position)
        {
            return m_viewaccessor.Container.View.ReadSingle(GetPosition(position));
        }


        
        
        public  ushort ReadUInt16(long position)
        {
            return m_viewaccessor.Container.View.ReadUInt16(GetPosition(position));
        }


        
        
        public  uint ReadUInt32(long position)
        {
            return m_viewaccessor.Container.View.ReadUInt32(GetPosition(position));
        }


        
        
        public  ulong ReadUInt64(long position)
        {
            return m_viewaccessor.Container.View.ReadUInt64(GetPosition(position));
        }


        public  void Write(long position, bool value)
        {
            m_viewaccessor.Container.View.Write(GetPosition(position), value);
        }


        public  void Write(long position, byte value)
        {
            m_viewaccessor.Container.View.Write(GetPosition(position), value);
        }


        
        public  void Write(long position, char value)
        {
            m_viewaccessor.Container.View.Write(GetPosition(position), value);
        }


        
        public  void Write(long position, decimal value)
        {
            m_viewaccessor.Container.View.Write(GetPosition(position), value);
        }


        
        public  void Write(long position, double value)
        {
            m_viewaccessor.Container.View.Write(GetPosition(position), value);
        }


        
        public  void Write(long position, float value)
        {
            m_viewaccessor.Container.View.Write(GetPosition(position), value);
        }


        
        public  void Write(long position, int value)
        {
            m_viewaccessor.Container.View.Write(GetPosition(position), value);
        }


        
        public  void Write(long position, long value)
        {
            m_viewaccessor.Container.View.Write(GetPosition(position), value);
        }


        public  void Write<T>(long position, ref T structure) where T: struct
        {
            Write<T>(position, structure);
        }

        
        public  void Write<T>(long position, T structure) where T : struct
        {
            this.m_viewaccessor.Container.View.Write<T>(GetPosition(position), ref structure);
        }


        
        
        public  void Write(long position, sbyte value)
        {
            m_viewaccessor.Container.View.Write(GetPosition(position), value);
        }


        
        public  void Write(long position, short value)
        {
            m_viewaccessor.Container.View.Write(GetPosition(position), value);
        }


        
        
        public  void Write(long position, uint value)
        {
            m_viewaccessor.Container.View.Write(GetPosition(position), value);
        }


        
        
        public  void Write(long position, ulong value)
        {
            m_viewaccessor.Container.View.Write(GetPosition(position), value);
        }


        
        
        public  void Write(long position, ushort value)
        {
            m_viewaccessor.Container.View.Write(GetPosition(position), value);
        }


        
        public  void WriteArray<T>(long position, T[] array, int offset, int count) where T : struct
        {
            m_viewaccessor.Container.View.WriteArray<T>(GetPosition(position), array, offset, count);
        }

        private long GetPosition(long requestedPosition)
        {
            return m_offset - m_viewoffset + requestedPosition;
        }

        /// <summary>
        /// Similar to Read&lt;T&gt; except it first reads the struct as byte array and then copies it to the struct's memory pointer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="structure"></param>
        public void ReadStruct<T>(long position, out T structure) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));

            byte[] readstruct = new byte[size];

            if ((size + position) > m_capacity)
                throw new ArgumentOutOfRangeException("Either position is invalid or structure is too big");

            m_viewaccessor.Container.View.ReadArray<byte>(GetPosition(position), readstruct, 0, size);

            structure = new T();

            BytesToStruct<T>(readstruct, ref structure);

            //m_viewaccessor.Container.View.Read<T>(GetPosition(position), out structure);
        }

        /// <summary>
        /// Similar to Write&lt;T&gt; except it first copies the struct to a byte array and then writes that byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="structure"></param>
        public void WriteStruct<T>(long position, T structure) where T : struct
        {
            byte[] array = StructToBytes<T>(structure);
            this.m_viewaccessor.Container.View.WriteArray<byte>(GetPosition(position), array, 0, array.Length);
        }

        /// <summary>
        /// Copy T to byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private byte[] StructToBytes<T>(T obj)
        {
            int structsize = Marshal.SizeOf(obj);
            byte[] barray = new byte[structsize];
            IntPtr structptr = Marshal.AllocHGlobal(structsize);

            Marshal.StructureToPtr(obj, structptr, true);
            Marshal.Copy(structptr, barray, 0, structsize);
            Marshal.FreeHGlobal(structptr);

            return barray;
        }

        /// <summary>
        /// Copy byte array to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytearray"></param>
        /// <param name="obj"></param>
        private void BytesToStruct<T>(byte[] bytearray, ref T obj)
        {
            int structsize = Marshal.SizeOf(obj);
            IntPtr structptr = Marshal.AllocHGlobal(structsize);

            Marshal.StructureToPtr(obj, structptr, true);
            Marshal.Copy(bytearray, 0, structptr, structsize);
            Marshal.FreeHGlobal(structptr);
        }

        public void Dispose()
        {
            //m_viewaccessor.Container.View.Flush();
            m_viewaccessor.Container.MappedViews.Remove(m_viewaccessor);
            m_viewaccessor = null;
        }
    }
}
