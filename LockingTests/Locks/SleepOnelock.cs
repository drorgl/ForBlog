/*
Sleeplock - Spinlock using sleep(0)
Dror Gluska
	
  
Change log:
2012-07-21 - Initial version
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LockingTests.Locks
{
    /// <summary>
    /// Spinlock using sleep(0)
    /// </summary>
    class SleepZerolock : ILocker
    {
        private int _lock = 0;

        public void Enter()
        {
            while (Interlocked.CompareExchange(ref _lock, 1, 0) != 0)
            {
                Thread.Sleep(0);
            }
        }

        public void Exit()
        {
            _lock = 0;
        }
    }
}
