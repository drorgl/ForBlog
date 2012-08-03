/*
Limiter - Limits calls per interval and concurrency
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
2012-08-03 - Initial version
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GoogleAnalyticsDemo
{
    /// <summary>
    /// Class which allowes you control over how many calls per interval and how many concurrent calls
    /// <para>Works like a regular lock.</para>
    /// </summary>
    public class Limiter
    {
        /// <summary>
        /// Holds the latest datetime in which the requests per interval is valid
        /// </summary>
        private DateTime _last;

        /// <summary>
        /// Interval in which we're limiting the calls.
        /// </summary>
        private TimeSpan _interval;

        /// <summary>
        /// current calls per interval, this is zeroes each interval.
        /// </summary>
        private volatile int _callsperinterval;

        /// <summary>
        /// Maximum calls per interval
        /// </summary>
        private volatile int _maxcallsperinterval;


        /// <summary>
        /// Concurrent requests counter
        /// </summary>
        private volatile int _concurrentcalls;

        /// <summary>
        /// Maximum concurrent calls counter
        /// </summary>
        private volatile int _maxconcurrentcalls;

        /// <summary>
        /// Lock for calls counters.
        /// </summary>
        private SpinLock _callslock = new SpinLock();

        /// <summary>
        /// Initializes a new instance of the Limiter class
        /// </summary>
        /// <param name="interval">time span for maxRequestsPerInterval</param>
        /// <param name="maxCallsPerInterval">maximum calls per interval</param>
        /// <param name="maxConcurrent">maximum concurrent calls</param>
        public Limiter(TimeSpan interval, int maxCallsPerInterval, int maxConcurrent)
        {
            _interval = interval;
            _last = DateTime.Now + _interval;

            _callsperinterval = 0;
            _maxcallsperinterval = maxCallsPerInterval;
            _concurrentcalls = 0;
            _maxconcurrentcalls = maxConcurrent;
        }


        /// <summary>
        /// Acquire a call, blocks if not available until available.
        /// </summary>
        public void Enter()
        {
            
            while (true)
            {
                var locktaken = false;
                _callslock.Enter(ref locktaken);

                if (_last < DateTime.Now)
                {
                    _last = DateTime.Now + _interval;
                    _callsperinterval = 0;
                }

                //check interval
                //check concurrent
                if ((_concurrentcalls < _maxconcurrentcalls) && (_callsperinterval < _maxcallsperinterval))
                {
                    Interlocked.Increment(ref _concurrentcalls);
                    Interlocked.Increment(ref _callsperinterval);
                    _callslock.Exit();
                    break;
                }
                _callslock.Exit();
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Releases a call
        /// </summary>
        public void Exit()
        {
            //reduce concurrent
            var locktaken = false;
            _callslock.Enter(ref locktaken);
            Interlocked.Decrement(ref _concurrentcalls);
            _callslock.Exit();
        }
    }
}
