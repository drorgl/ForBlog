﻿/*
MemoryBuffer - Circular Memory Buffer / FIFO Implementation for C#
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


This class can be easily changed to be a generic class but will have to 
drop the Stream inheritance, if you require multiple writes/reads/peeks, feel free.
  
  
  
  
Change log:
2012-07-17 - Initial version
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace ChatMessages
{

    /// <summary>
    /// Circular Memory Buffer
    /// <para>Circular buffer/FIFO implementation</para>
    /// </summary>
    public class MemoryBuffer : Stream
    {
        /// <summary>
        /// Array for the buffer
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// Max length of buffer
        /// </summary>
        private int _length;

        /// <summary>
        /// Where the buffer starts
        /// </summary>
        private int _head;

        /// <summary>
        /// Where the buffer ends
        /// </summary>
        private int _tail;


        /// <summary>
        /// Initialize a new buffer with selected length
        /// </summary>
        /// <param name="length">Maximum length for the buffer</param>
        public MemoryBuffer(int length)
        {
            _length = length + 1;
            _buffer = new byte[_length];
            _head = 0;
            _tail = 0;
        }

        #region notimplemented

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            
        }

        

        public override long Position
        {
            get
            {
                return 0;
            }
            set
            {
                throw new InvalidOperationException("Buffer is not seekable");
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException("Buffer is not seekable");
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException("Can't set length after initialization, create a new buffer with required length");
        }

       

        #endregion

        /// <summary>
        /// The amount of bytes currently in the buffer
        /// </summary>
        public override long Length
        {
            get {
                if (_tail > _head)
                    return _tail - _head;
                else if (_head > _tail)
                    return (_length - _head) + _tail;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Peek inside the buffer without taking bytes out
        /// </summary>
        public int Peek(byte[] buffer, int offset, int count)
        {
            int retval = (int)this.Length;
            if (retval > count)
                retval = count;
            int bytesleft = count;
            int byteoffset = offset;

            int temphead = _head;


            do
            {
                if ((temphead == _length) && (_tail > 0))
                    temphead = 0;

                var copystart = temphead;
                var copylen = (_tail < temphead) ? (_length - temphead) : (_tail - temphead);
                if (copylen > bytesleft)
                    copylen = bytesleft;

                temphead += copylen;
                Array.Copy(_buffer, copystart, buffer, byteoffset, copylen);
                byteoffset += copylen;
                bytesleft -= copylen;
            } while ((
                
                (
                    ((_tail > temphead) ? (_tail - temphead) : 
                                            (
                                                (temphead > _tail) ? ((_length - temphead) + _tail) : 0
                                            )) > 0)


                ) && (bytesleft > 0));

            return count - bytesleft;
        }

        /// <summary>
        /// Read amount of bytes from the buffer
        /// </summary>
        /// <returns>number of bytes actually read from the buffer</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int retval = (int)this.Length;
            if (retval > count)
                retval = count;
            int bytesleft = count;
            int byteoffset = offset;
            do
            {
                if ((_head == _length) && (_tail > 0))
                    _head = 0;

                var copystart = _head;
                var copylen = (_tail < _head) ? (_length - _head) : (_tail - _head);
                if (copylen > bytesleft)
                    copylen = bytesleft;

                _head += copylen;
                Array.Copy(_buffer, copystart, buffer, byteoffset, copylen);
                byteoffset += copylen;
                bytesleft -= copylen;
            } while ((this.Length > 0) && (bytesleft > 0));

            return count - bytesleft;
        }

        /// <summary>
        /// Maximum Chunk Length
        /// <para>Each write/read is done in chunks, a chunk is the amount of bytes that can be done by a single Array.Copy operation</para>
        /// </summary>
        public int ChunkLength
        {
            get
            {
                if (_tail > _head)
                    return _length - _tail;
                else if (_head > _tail)
                    return (_head - _tail - 1);
                else if (_head == _tail)
                    return _length - _tail;
                
                throw new InternalBufferOverflowException("Programming Error in MemoryBuffer");

            }
        }

        /// <summary>
        /// Same as write, but blocks on buffer full
        /// </summary>
        public void Push(byte[] buffer, int offset, int count)
        {
            //get available bytes
            var availablebytes = _length - this.Length;
            var bytesleft = count;
            var bytesoffset = offset;

            //push to fill
            do
            {
                if ((_tail == _length) && (_head > 0))
                    _tail = 0;

                var copystart = _tail;
                var copylen = this.ChunkLength;
                if (copylen > bytesleft)
                    copylen = bytesleft;

                _tail += copylen;

                Array.Copy(buffer, bytesoffset, _buffer, copystart, copylen);
                bytesoffset += copylen;
                bytesleft -= copylen;

                if ((copylen == 0) && (bytesleft > 0))
                {
                    Thread.Sleep(1);
                }
            } while (bytesleft > 0);
        }

        /// <summary>
        /// Writes bytes into the buffer, is the count is more than available bytes in the buffer, throws an exception.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if ((count + this.Length) > _length)
                throw new OverflowException("Not enough room for this write");
            Push(buffer, offset, count);
        }

        
    }
}
