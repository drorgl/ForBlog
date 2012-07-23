/*
FrameworkSpinlock - Microsoft's Spinlock wrapper
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
    class FrameworkSpinlock : ILocker
    {
        private SpinLock _lock = new SpinLock();
        #region ILocker Members

        public void Enter()
        {
            bool locktaken = false;
            _lock.Enter(ref locktaken);
        }

        public void Exit()
        {
            _lock.Exit();
        }

        #endregion
    }
}
