﻿/*
CompressedNetworkStream - Compressed Network Stream
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
2012-07-20 - Fixed idle high CPU usage
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.IO.Compression;

namespace ChatMessages
{
    /// <summary>
    /// Compressed Network Stream
    /// <para>can be used with any stream, but it uses blocks
    /// instead of a whole stream so its less efficient than using a GZipStream for example.
    /// </para>
    /// </summary>
    public class CompressedNetworkStream : Stream, IDisposable
    {
        /// <summary>
        /// Maximum buffer size, we're using 3 bytes for the message length, so if you're make this
        /// value different, need to go over the code and adjust the 4th byte and so on.
        /// </summary>
        private const int _maxbuffersize = 255 * 255 * 255;

        /// <summary>
        /// temporary buffer size, for copy operations between streams
        /// </summary>
        private const int _tempbuffersize = 4096;

        /// <summary>
        /// The base stream the compression works on
        /// </summary>
        private Stream _basestream;

        /// <summary>
        /// Read buffer, this buffer stores the uncompressed blocks after they have been processed by the ReadThread
        /// </summary>
        private MemoryBuffer _readbuffer = new MemoryBuffer(_maxbuffersize);

        /// <summary>
        /// A lock for the read buffer
        /// </summary>
        private SpinLock _readbufferlock = new SpinLock();

        /// <summary>
        /// Write buffer, this buffer stores the uncompressed bytes before they have been processed by the WriteThread
        /// </summary>
        private MemoryBuffer _writebuffer = new MemoryBuffer(_maxbuffersize);

        /// <summary>
        /// A lock for the write buffer
        /// </summary>
        private SpinLock _writebufferlock = new SpinLock();

        /// <summary>
        /// Flush flag
        /// </summary>
        private volatile bool _flush = false;

        /// <summary>
        /// Write thread, responsible for compressing
        /// </summary>
        private Thread _writethread;

        /// <summary>
        /// Read thread, responsible for decompressing
        /// </summary>
        private Thread _readthread;

        /// <summary>
        /// Initializes a new CompressedNetworkStream with a base stream
        /// </summary>
        /// <param name="baseStream">the network stream to perform compression on</param>
        public CompressedNetworkStream(Stream baseStream)
        {
            _basestream = baseStream;
            _writethread = new Thread(new ThreadStart(WriteThread));
            _writethread.Start();
            _readthread = new Thread(new ThreadStart(ReadThread));
            _readthread.Start();
        }

        /// <summary>
        /// Write Thread, responsible for compressing and sending the compressed blocks to the base stream
        /// </summary>
        private void WriteThread()
        {
            while (true)
            {
                //writebuffer is either 16mb or flush requested
                if (((_writebuffer.Length >= _maxbuffersize) || (_flush)) && (_writebuffer.Length > 0))
                {
                    _flush = false;
                    //compress the buffer, but also store the original buffer so later we can decide which one is smaller.

                    MemoryStream compressed = new MemoryStream();
                    MemoryStream uncompressed = new MemoryStream();

                    byte[] buffer = new byte[_tempbuffersize];
                    bool locktaken = false;
                    _writebufferlock.Enter(ref locktaken);
                    int readbytes = 0;
                    do
                    {
                        readbytes = _writebuffer.Read(buffer, 0, buffer.Length);
                        uncompressed.Write(buffer, 0, readbytes);
                    } while (readbytes > 0);
                    _writebufferlock.Exit();

                    //compress the block
                    uncompressed.Seek(0, SeekOrigin.Begin);
                    using (GZipStream gzcompression = new GZipStream(compressed, CompressionMode.Compress, true))
                    {
                        uncompressed.CopyTo(gzcompression);
                        gzcompression.Close();
                    }


                    //check if we rather use the compressed or the uncompressed byte stream
                    MemoryStream selectedstream = null;

                    //check if compressed buffer is bigger than uncompressed buffer.
                    if (compressed.Length > uncompressed.Length)
                        selectedstream = uncompressed;
                    else
                        selectedstream = compressed;

                    //write to writecompressed
                    //first, write 3 bytes of the length, so later the reading stream can determine the block size.
                    byte[] lenint = BitConverter.GetBytes((int)selectedstream.Length);
                    _basestream.Write(lenint, 0, 3);

                    //write which stream we selected, either the compressed or the uncompressed.
                    if (compressed.Length > uncompressed.Length)
                        _basestream.WriteByte(0);
                    else
                        _basestream.WriteByte(1);

                    //write writecompressed to underlying stream
                    selectedstream.Seek(0, SeekOrigin.Begin);
                    do
                    {
                        readbytes = selectedstream.Read(buffer, 0, buffer.Length);
                        _basestream.Write(buffer, 0, readbytes);
                    } while (readbytes > 0);
                    _basestream.Flush();
                    
                }
                else
                {
                    Thread.Sleep(1);
                }


                
               
            }
        }

        /// <summary>
        /// Read Thread, responsible for decompressing the block of bytes arriving from base stream
        /// </summary>
        private void ReadThread()
        {
            while (true)
            {
                //read the underlying stream 4 bytes to determine length
                byte[] buflen = new byte[4];
                int buflenbytes = buflen.Length;
                int readbytes = 0;
                while (buflenbytes > 0)
                {
                    readbytes = _basestream.Read(buflen, 4 - buflenbytes, buflenbytes);
                    buflenbytes -= readbytes;

                    if (readbytes == 0)
                    {
                        Thread.Sleep(1);
                    }
                }

                bool iscompressed = false;
                if (buflen[3] == 1)
                    iscompressed = true;
                buflen[3] = 0;

                //get the number of bytes in the block
                int bufferlength = BitConverter.ToInt32(buflen, 0);
                
                MemoryStream block = new MemoryStream();

                //read the block into the memorystream;
                readbytes = 0;
                byte[] buffer = new byte[_tempbuffersize];
                while (bufferlength > 0)
                {
                    readbytes = _basestream.Read(buffer, 0, (buffer.Length > bufferlength) ? bufferlength : buffer.Length);
                    block.Write(buffer, 0, readbytes);
                    bufferlength -= readbytes;
                }

                //if block is compressed, process it
                if (iscompressed)
                {
                    block.Seek(0, SeekOrigin.Begin);
                    MemoryStream uncompressed = new MemoryStream();
                    GZipStream decompress = new GZipStream(block, CompressionMode.Decompress);
                    decompress.CopyTo(uncompressed);
                    decompress.Flush();
                    block = uncompressed;
                }

                //write it to readbuffer
                readbytes = 0;
                block.Seek(0, SeekOrigin.Begin);
                bool locktaken = false;
                _readbufferlock.Enter(ref locktaken);
                do
                {
                    readbytes = block.Read(buffer, 0, buffer.Length);
                    _readbuffer.Write(buffer, 0, readbytes);
                } while (readbytes > 0);
                _readbufferlock.Exit();
            }

        }



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
            _flush = true;
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException("Can't seen an unseekable stream");
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException("Can't set length after compressed network stream has been created");
        }

        /// <summary>
        /// Reads a sequence of bytes from the uncompressed stream and removes the bytes read from the buffer
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            bool locktaken = false;
            _readbufferlock.Enter(ref locktaken);
            int readbuflen = _readbuffer.Read(buffer, offset, count);
            _readbufferlock.Exit();
            return readbuflen;
        }

        
        /// <summary>
        /// Writes a sequence of bytes to be compressed and passed to the base stream
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            bool locktaken = false;
            _writebufferlock.Enter(ref locktaken);
            _writebuffer.Write(buffer, offset, count);
            _writebufferlock.Exit();

        }

        /// <summary>
        /// Aborts read and write threads
        /// </summary>
        public void Dispose()
        {
            if (_writethread != null)
                _writethread.Abort();
            if (_readthread != null)
                _readthread.Abort();

            base.Dispose();
        }
    }
}
