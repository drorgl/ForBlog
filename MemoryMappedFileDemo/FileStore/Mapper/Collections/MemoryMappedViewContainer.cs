﻿/*
MemoryMappedViewContainer - Contains a list of MemoryMappedViews for each View + Range
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
    /// Contains a list of MemoryMappedViews for each View + Range
    /// </summary>
    class MemoryMappedViewContainer
    {
        /// <summary>
        /// Accessor for MemoryMappedFile object, for late view creation
        /// </summary>
        internal MemoryMapper MemoryMapper { get; set; }

        /// <summary>
        /// The Range of the Accessor
        /// </summary>
        public Range<Int64> Range { get; set; }

        /// <summary>
        /// Lazy view variable
        /// </summary>
        private MemoryMappedViewAccessor m_view = null;

        /// <summary>
        /// The MemoryMappedFile View
        /// </summary>
        public MemoryMappedViewAccessor View
        {
            get
            {
                if (m_view == null)
                {
                    if (MemoryMapper == null)
                        throw new ApplicationException("MemoryMappedFile was not supplied to the container");

                    //attempt to create view 4 times before failing, clear unused memory between calls
                    for (var i = 0; i <= 3; i++)
                    {
                        m_view = CreateView((i == 3));
                        if (m_view == null)
                        {
                            this.MemoryMapper.ClearUnusedMappings();
                        }
                        else
                        {
                            break;
                        }
                       
                    }
                }
                return m_view;
            }
        }

        /// <summary>
        /// Attempts to create a view
        /// </summary>
        private MemoryMappedViewAccessor CreateView(bool throwException)
        {
            try
            {
                return MemoryMapper.m_MemoryMappedFile.CreateViewAccessor(this.Range.Minimum, this.Range.Maximum - this.Range.Minimum);
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
        /// List of VirtualViews
        /// </summary>
        internal List<VirtualViewContainer> MappedViews { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        internal MemoryMappedViewContainer()
        {
            this.MappedViews = new List<VirtualViewContainer>();
        }

        /// <summary>
        /// Internal method for clearing the view from memory
        /// </summary>
        internal void ClearView()
        {
            if (m_view != null)
            {
                m_view.Dispose();
                m_view = null;
            }

        }
    }
}
