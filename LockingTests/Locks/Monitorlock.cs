/*
Monitorlock - Lock with Monitor
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
    /// Lock with Monitor
    /// </summary>
    class Monitorlock : ILocker
    {
        #region ILocker Members

        private object _lock = new object();

        public void Enter()
        {
            Monitor.Enter(_lock);
        }

        public void Exit()
        {
            Monitor.Exit(_lock);
        }

        #endregion
    }
}
