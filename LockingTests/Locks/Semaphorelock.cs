/*
Semaphorelock - Lock with Semaphore
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
    class Semaphorelock : ILocker
    {
        private Semaphore _lock = new Semaphore(1, 2);

        #region ILocker Members

        public void Enter()
        {
            _lock.WaitOne();
        }

        public void Exit()
        {
            _lock.Release();
        }

        #endregion
    }
}
