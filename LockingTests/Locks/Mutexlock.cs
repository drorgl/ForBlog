/*
Mutexlock - Lock with Mutex
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
    class Mutexlock : ILocker
    {
        #region ILocker Members

        private Mutex _lock = new Mutex();

        public void Enter()
        {
            _lock.WaitOne();
        }

        public void Exit()
        {
            _lock.ReleaseMutex();
        }

        #endregion
    }
}
