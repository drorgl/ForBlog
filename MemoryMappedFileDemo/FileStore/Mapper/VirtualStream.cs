﻿/*
VirtualStream - Memory Mapped Virtual Stream
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
using System.IO;
using System.Threading;
using FileStore.Mapper.Collections;

namespace FileStore.Mapper
{
    /// <summary>
    /// Memory Mapped Virtual Stream
    /// </summary>
    public class VirtualStream : Stream, IDisposable
    {
        private VirtualStreamContainer m_stream;
        private long m_stream_offset;
        private long m_offset;
        private long m_length;

        internal VirtualStream(VirtualStreamContainer stream, long stream_offset, long offset, long length)
        {
            m_stream = stream;
            m_offset = offset;
            m_length = length;
            m_stream_offset = stream_offset;
            m_stream.Container.Stream.Position = m_offset - m_stream_offset;
        }

        public override void Close()
        {
            //ignore close request as the underlying stream might still be active for other requests.
        }

        public override bool CanRead
        {
            get 
            {
                return m_stream.Container.Stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return m_stream.Container.Stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get { return m_stream.Container.Stream.CanWrite; }
        }

        public override void Flush()
        {
            m_stream.Container.Stream.Flush();
        }

        public override long Length
        {
            get { return m_length; }
        }

        public override long Position
        {
            get
            {
                return m_stream.Container.Stream.Position - m_offset - m_stream_offset;
            }
            set
            {
                //check position is inside the range
                if ((value-m_stream_offset) > m_length)
                    throw new ArgumentOutOfRangeException();

                m_stream.Container.Stream.Position = value + m_offset + m_stream_offset;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var calculatedcount = count;

            if (this.Position + count > m_length)
                calculatedcount = (int)(m_length - this.Position);

            return m_stream.Container.Stream.Read(buffer, offset, calculatedcount);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.Position + count > m_length)
                throw new ArgumentOutOfRangeException();

            m_stream.Container.Stream.Write(buffer, offset , count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = offset;
                    break;
                case SeekOrigin.Current:
                    this.Position += offset;
                    break;
                case SeekOrigin.End:
                    this.Position = m_length - offset;
                    break;
            }
            return this.Position;
        }

        public override void SetLength(long value)
        {
            //can't really set a length on a virtual stream, should be part of the GetVirtualStream request
            throw new NotImplementedException();
        }

        public new void Dispose()
        {
            //remove this stream from its container.
            m_stream.Container.MappedStreams.Remove(m_stream);
            m_stream = null;
        }
       
    }
}
