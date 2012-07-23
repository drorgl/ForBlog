/*
Complexlock - Complex spin lock
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
    /// Complex spin lock
    /// </summary>
    class Complexlock : ILocker
    {
        private int _lock = 0;

        public void Enter()
        {
            int i = 1;
            while (Interlocked.CompareExchange(ref _lock, 1, 0) != 0)
            {
                if (i == 1)
                    Thread.SpinWait(1);
                if (i > 5)
                    Thread.Yield();
                if (i > 10)
                    Thread.Sleep(0);
                if (i > 15)
                    Thread.Sleep(1);
                i++;
            }
        }

        public void Exit()
        {
            _lock = 0;
        }
    }
}
