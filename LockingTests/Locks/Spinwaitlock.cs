/*
Spinwaitlock - Spinlock using SpinWait for waiting
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
    /// Spinlock using SpinWait for waiting
    /// </summary>
    class Spinwaitlock : ILocker
    {
        private int _lock = 0;

        public void Enter()
        {
            int i = 1;
            while (Interlocked.CompareExchange(ref _lock, 1, 0) != 0)
            {

                Thread.SpinWait(++i);
            }
        }

        public void Exit()
        {
            _lock = 0;
        }
    }
}
