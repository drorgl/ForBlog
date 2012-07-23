/*
SemaphoreSlimlock - Lock with SemaphoreSlim
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
    class SemaphoreSlimlock : ILocker
    {
        private SemaphoreSlim _lock = new SemaphoreSlim(1, 2);

        #region ILocker Members

        public void Enter()
        {
            _lock.Wait();
        }

        public void Exit()
        {
            _lock.Release();
        }

        #endregion
    }
}
