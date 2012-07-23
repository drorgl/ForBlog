/*
Yieldlock - Spinlock yields CPU when attempting to lock
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
    /// Spinlock yields CPU when attempting to lock
    /// </summary>
    class Yieldlock : ILocker
    {
        private int _lock = 0;

        public void Enter()
        {
            while (Interlocked.CompareExchange(ref _lock, 1, 0) != 0)
            {
                Thread.Yield();
            }
        }

        public void Exit()
        {
            _lock = 0;
        }
    }
}
