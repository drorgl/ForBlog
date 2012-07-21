﻿/*
StreamReadAsync - Provides an Async Reading for a blocking Stream
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
using System.Net.Security;
using System.IO;
using System.Threading;

namespace ChatMessages
{
    /// <summary>
    /// Provides an Async Reading for a blocking Stream
    /// </summary>
    public class StreamReadAsync : Stream
    {
        /// <summary>
        /// Base Stream
        /// </summary>
        Stream _basestream;

        /// <summary>
        /// Initialize a new StreamReadAsync with stream
        /// </summary>
        /// <param name="stream"></param>
        public StreamReadAsync(Stream stream)
        {
            _basestream = stream;
        }

        /// <summary>
        /// Read buffer
        /// </summary>
        private MemoryBuffer _readbuffer = new MemoryBuffer(65536);

        /// <summary>
        /// Read buffer lock
        /// </summary>
        private SpinLock _readbufferlock = new SpinLock();

        /// <summary>
        /// Read thread
        /// </summary>
        private Thread _readthread = null;

        /// <summary>
        /// Reading thread
        /// </summary>
        private void ReadThread()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytesread = _basestream.Read(buffer, 0, buffer.Length);
                if (bytesread > 0)
                {
                    bool locktaken = false;
                    _readbufferlock.Enter(ref locktaken);
                    _readbuffer.Write(buffer, 0, bytesread);
                    _readbufferlock.Exit();
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        /// <summary>
        /// Reads a stream, prevents blocking streams from interrupting flow
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            
            
            //read available bytes from buffer
            var locktaken = false;
            _readbufferlock.Enter(ref locktaken);
            {
                if (_readthread == null)
                {
                    _readthread = new Thread(new ThreadStart(ReadThread));
                    _readthread.Start();
                }
                var readcount = _readbuffer.Read(buffer, offset, count);

                _readbufferlock.Exit();

                return readcount;

            }
        }

        #region Stream copy implementation

        public new void Dispose()
        {
            _readthread.Abort();
            _readthread = null;
        }


        public override bool CanRead
        {
            get { return _basestream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _basestream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _basestream.CanWrite; }
        }

        public override void Flush()
        {
            _basestream.Flush();
        }

        public override long Length
        {
            get { return _basestream.Length; }
        }

        public override long Position
        {
            get
            {
                return _basestream.Position;
            }
            set
            {
                _basestream.Position = value;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _basestream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _basestream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _basestream.Write(buffer, offset, count);
        }
        #endregion
    }
}
