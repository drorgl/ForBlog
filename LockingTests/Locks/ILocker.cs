using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockingTests.Locks
{
    /// <summary>
    /// Locker interface
    /// </summary>
    interface ILocker
    {
        void Enter();
        void Exit();
    }
}
