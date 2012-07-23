using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LockingTests.Locks;
using System.Threading;

namespace LockingTests
{
    class Payload
    {
        private ILocker _locker;

        public Payload(ILocker locker)
        {
            _locker = locker;
        }

        volatile int concurrent = 0;

        public void ThreadX()
        {
            long res = 0;
            for (int i = 0; i < 10000; i++)
            {
                _locker.Enter();
                Interlocked.Increment(ref concurrent);
                for (int z = 0; z < 1000; z++)
                    res += res;
                if (concurrent > 1)
                    Console.WriteLine("{0} concurrency issues!", _locker.GetType().Name);

                Interlocked.Decrement(ref concurrent);
                _locker.Exit();
                
            }
        }
    }
}
