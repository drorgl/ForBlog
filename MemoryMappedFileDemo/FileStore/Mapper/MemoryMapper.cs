﻿/*
MemoryMapper - A Simplified MemoryMappedFiles accessor
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


It contains easy stream/view disposal, file resize and recreate all accessors
It minimizes the Views and Streams accessors by increasing the requested range and returning virtual accessors
 
  
Change log:
2013-03-15 - Initial version
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.MemoryMappedFiles;
using FileStore.Mapper.Collections;
using System.IO;

namespace FileStore.Mapper
{
    /// <summary>
    /// Simplified MemoryMappedFiles Access
    /// </summary>
    public class MemoryMapper : IDisposable
    {
        /// <summary>
        /// Maximized Range for each Accessor Creation
        /// </summary>
        private long m_maximize_range = 1024 * 1024 * 1;

        /// <summary>
        /// Grow file size automatically by
        /// </summary>
        private long m_grow_by = 1024 * 1024 * 1;

        /// <summary>
        /// file name
        /// </summary>
        private string m_filename;

        /// <summary>
        /// All Streams this Mapper is responsible for
        /// </summary>
        private List<MemoryMappedStreamContainer> m_streams = new List<MemoryMappedStreamContainer>();


        /// <summary>
        /// All Views this Mapper is responsible for
        /// </summary>
        private List<MemoryMappedViewContainer> m_views = new List<MemoryMappedViewContainer>();

        /// <summary>
        /// MemoryMappedFile instance
        /// </summary>
        internal MemoryMappedFile m_MemoryMappedFile;

        /// <summary>
        /// Physical File Size
        /// </summary>
        private long m_filesize = 0;

        public MapperStatistics Statistics { get; private set; }

        /// <summary>
        /// Create a new instance of MemoryMapper
        /// </summary>
        /// <param name="filename"></param>
        public MemoryMapper(string filename)
        {
            Statistics = new MapperStatistics();
            m_filename = filename;
            ReadFileSize();
        }

        /// <summary>
        /// Create a new instance of MemoryMapper
        /// </summary>
        public MemoryMapper(string filename, long maximizeRange, long growBy)
        {
            Statistics = new MapperStatistics();
            m_filename = filename;
            this.m_maximize_range = maximizeRange;
            this.m_grow_by = growBy;
            ReadFileSize();
        }

        /// <summary>
        /// Maximized Range for each Accessor Creation
        /// </summary>
        public long MaximizeRange
        {
            get { return this.m_maximize_range; }
            set { this.m_maximize_range = value; }
        }

        /// <summary>
        /// Grow file size automatically by
        /// </summary>
        public long GrowBy
        {
            get { return this.m_grow_by; }
            set { this.m_grow_by = value; }
        }

        /// <summary>
        /// Reads the Physical File Size
        /// </summary>
        private void ReadFileSize()
        {
            if (File.Exists(m_filename))
            {
                FileInfo fi = new FileInfo(m_filename);
                m_filesize = fi.Length;
            }
            else
            {
                GrowFile(m_grow_by);
            }
            m_MemoryMappedFile = MemoryMappedFile.CreateFromFile(m_filename);
        }

        /// <summary>
        /// Grow File to newsize
        /// </summary>
        /// <param name="newsize">new size of file</param>
        private void GrowFile(long newsize)
        {
            if (newsize < m_filesize)
                throw new ArgumentOutOfRangeException("Can't reduce file size");

            using (var fs = new FileStream(m_filename, FileMode.OpenOrCreate))
            {
                fs.SetLength(newsize);
                fs.Close();
                m_filesize = newsize;
            }
        }

        /// <summary>
        /// Grow file size to newsize and rebuild internal structures
        /// </summary>
        /// <param name="newsize">new size of file</param>
        /// <remarks>
        ///     this will clear all the MemoryMappedFile related objects and recreate
        ///     a basic skeleton needed for late view/stream creation
        /// </remarks>
        private void Grow(long newsize)
        {
            //dispose of all streams
            foreach (var sm in m_streams)
                sm.ClearStream();

            //remove unused streams
            m_streams.RemoveAll(i => i.MappedStreams.Count() == 0);

            //dispose of all views
            foreach (var vw in m_views)
                vw.ClearView();

            //remove all unused views
            m_views.RemoveAll(i => i.MappedViews.Count() == 0);

            //dispose of memory mapped file
            if (m_MemoryMappedFile != null)
                m_MemoryMappedFile.Dispose();

            //resize file
            GrowFile(newsize);

            //create memory mapped file
            m_MemoryMappedFile = MemoryMappedFile.CreateFromFile(m_filename);

            Statistics.NumberOfResizes++;
        }

        /// <summary>
        /// Retrieves the Maximized Range for requested offset/length, if the file 
        /// is too small, it will grow it and then get the requested/maximized range
        /// </summary>
        /// <param name="offset">offset of range</param>
        /// <param name="length">length of range</param>
        /// <returns>Maximized Range</returns>
        private Range<Int64> GetMaximizedRange(long offset, long length)
        {
            //if length is smaller than file size, return maximized file size
            if ((offset + length) < m_filesize)
            {
                var min = offset - m_maximize_range;
                if (min < 0)
                    min = 0;

                var max = offset + length + m_maximize_range;
                if (max > m_filesize)
                    max = m_filesize;

                return new Range<Int64> { Minimum = min, Maximum = max };
            }

            //if length is bigger than file size, grow file by "grow by".
            Grow(m_filesize + m_grow_by);
            //recursive execute
            return GetMaximizedRange(offset, length);
        }


        /// <summary>
        /// Create a VirtualView
        /// </summary>
        /// <param name="offset">absolute offset</param>
        /// <param name="length">length of virtual view</param>
        /// <returns>VirtualView instance</returns>
        public VirtualView CreateView(long offset, long length)
        {
            var range = new Range<Int64> { Minimum = offset, Maximum = offset + length };
            //look for views in that size
            var view = GetViewByRange(range);
            if (view != null)
            {
                Statistics.NumberOfViewReuse++;

                //if found, create MappedViewContainer
                var mappedViewContainer = new VirtualViewContainer{ Container = view, Range = range };
                mappedViewContainer.MappedView = new VirtualView(mappedViewContainer, view.Range.Minimum, offset, length);
                view.MappedViews.Add(mappedViewContainer);

                //ClearUnusedMappings();

                return mappedViewContainer.MappedView;
            }

            //if not found, create view, create MappedViewContainer
            var maximized = GetMaximizedRange(offset, length);
            var container = new MemoryMappedViewContainer();
            container.Range = maximized;
            container.MemoryMapper = this;
            //container.View = m_mmf.CreateViewAccessor(maximized.Minimum, maximized.Maximum - maximized.Minimum, MemoryMappedFileAccess.ReadWrite);
            m_views.Add(container);

            Statistics.NumberOfViewCreates++;

            //recursively get view (no reason to implement logic twice, only for optimizations)
            return CreateView(offset, length);
        }

        /// <summary>
        /// Find Existing MemoryMappedView that has requested range
        /// </summary>
        /// <param name="range">Find any existing MemoryMappedViewContainer containing requested range</param>
        /// <returns>existing MemoryMappedViewContainer </returns>
        private MemoryMappedViewContainer GetViewByRange(Range<Int64> range)
        {
            foreach (var mv in m_views)
                if (range.IsInsideRange(mv.Range))
                    return mv;
            return null;
        }

        /// <summary>
        /// Create a VirtualStream
        /// </summary>
        /// <param name="offset">absolute offset</param>
        /// <param name="length">length of virtual stream</param>
        /// <returns>VirtualStream instance</returns>
        public VirtualStream CreateStream(long offset, long length)
        {
            var range = new Range<Int64> { Minimum = offset, Maximum = offset + length };
            //look for views in that size
            var stream = GetStreamByRange(range);
            if (stream != null)
            {
                Statistics.NumberOfStreamReuse++;

                //if found, create MappedViewContainer
                var mappedStreamContainer = new VirtualStreamContainer { Container = stream, Range = range };
                mappedStreamContainer.MappedStream = new VirtualStream(mappedStreamContainer,stream.Range.Minimum, offset, length);
                stream.MappedStreams.Add(mappedStreamContainer);

                //ClearUnusedMappings();

                return mappedStreamContainer.MappedStream;
            }

            //if not found, create view, create MappedViewContainer
            var maximized = GetMaximizedRange(offset, length);
            var container = new MemoryMappedStreamContainer();
            container.Range = maximized;
            container.MemoryMapper = this;
            //container.Stream = m_mmf.CreateViewStream(maximized.Minimum, maximized.Maximum - maximized.Minimum, MemoryMappedFileAccess.ReadWrite);
            m_streams.Add(container);

            Statistics.NumberOfStreamCreates++;

            //recursively get view (no reason to implement logic twice, only for optimizations)
            return CreateStream(offset, length);
        }

        /// <summary>
        /// Find Existing MemoryMappedStream that has requested range
        /// </summary>
        /// <param name="range">Find any existing MemoryMappedStreamContainer containing requested range</param>
        /// <returns>existing MemoryMappedStreamContainer</returns>
        private MemoryMappedStreamContainer GetStreamByRange(Range<Int64> range)
        {
            foreach (var ms in m_streams)
                if (range.IsInsideRange(ms.Range))
                    return ms;
            return null;
        }

        internal void ClearUnusedMappings()
        {
            //if ((m_streams.Select(i => i.Range.Maximum - i.Range.Minimum).Sum() + m_views.Select(i => i.Range.Maximum - i.Range.Minimum).Sum()) < MAXIMUM_UNUSED_MAPPINGS)
            //    return;

            //dispose of all unused streams
            foreach (var sm in m_streams)
            {
                if (sm.MappedStreams.Count() == 0)
                    sm.ClearStream();
            }

            //remove unused streams
            m_streams.RemoveAll(i => i.MappedStreams.Count() == 0);

            //dispose of all unused views
            foreach (var vw in m_views)
            {
                if (vw.MappedViews.Count() == 0)
                    vw.ClearView();
            }

            //remove all unused views
            m_views.RemoveAll(i => i.MappedViews.Count() == 0);

            Statistics.NumberOfMappingsCleanup++;
        }


        /// <summary>
        /// Flush all Streams/Views and free unused memory
        /// </summary>
        public void Flush()
        {
            foreach (var sm in m_streams)
                sm.Stream.Flush();

            //dispose of all views
            foreach (var vw in m_views)
                vw.View.Flush();

            Statistics.NumberOfForcedFlushes++;

            ClearUnusedMappings();
        }

        #region IDisposable Members

        public void Dispose()
        {
            //dispose of all streams
            foreach (var sm in m_streams)
                sm.Stream.Dispose();

            //dispose of all views
            foreach (var vw in m_views)
                vw.View.Dispose();

            //dispose of memory mapped file
            if (m_MemoryMappedFile != null)
                m_MemoryMappedFile.Dispose();

            m_MemoryMappedFile = null;
        }

        #endregion
    }
}
