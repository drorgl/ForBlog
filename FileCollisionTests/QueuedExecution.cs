using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace FileCollisionTests
{
    
    /// <summary>
    /// Abstract Queued Execution
    /// <para>Provides infrastructure for executing IItems with ProcessQueue override
    /// in a number of threads in defined in the constructor</para>
    /// </summary>
    public abstract class QueuedExecution : IDisposable
    {
        /// <summary>
        /// Process Result, returned by ProcessQueue
        /// </summary>
        protected enum ProcessResult
        {
            Success,
            FailThrow,
            FailRequeue
        }

        /// <summary>
        /// Item interface
        /// </summary>
        protected interface IItem {}

        /// <summary>
        /// Queue Item container
        /// </summary>
        private class QueueItem
        {
            /// <summary>
            /// IItem
            /// </summary>
            public IItem Item { get; set; }

            /// <summary>
            /// Result of ProcessQueue
            /// </summary>
            public ProcessResult ProcessResult { get; set; }

            /// <summary>
            /// ManualResetEvent for pinging back the waiting call
            /// </summary>
            public ManualResetEventSlim resetEvent { get; set; } 
        }

        /// <summary>
        /// Queue containing all the items for execution
        /// </summary>
        private ConcurrentQueue<QueueItem> _queue = new ConcurrentQueue<QueueItem>();

        /// <summary>
        /// Process Queue method, should be overriden in inheriting class
        /// </summary>
        /// <param name="item">item to be executed against</param>
        /// <returns>success/fail/requeue</returns>
        protected abstract ProcessResult ProcessQueue(IItem item);

        /// <summary>
        /// Number of threads to process queue
        /// </summary>
        private int _threadcount = 1;

        /// <summary>
        /// Threads array
        /// </summary>
        private Thread[] _threads;

        /// <summary>
        /// flag, should abort all executing threads
        /// </summary>
        private bool _threadaborted = false;

        /// <summary>
        /// Initializes the threads for execution
        /// </summary>
        private void Initialize()
        {
            _threads = new Thread[_threadcount];
            for (var i = 0; i < _threadcount; i++)
                _threads[i] = new Thread(new ThreadStart(() =>
                    {
                        do
                        {
                            QueueItem item;
                            if (_queue.TryDequeue(out item))
                            {
                                item.ProcessResult = ProcessQueue(item.Item);

                                if (item.ProcessResult == ProcessResult.FailRequeue)
                                {
                                    _queue.Enqueue(item);
                                    continue;
                                }

                                item.resetEvent.Set();
                            }
                            else
                            {
                                Thread.Sleep(1);
                            }
                        } while (!_threadaborted);
                    }));
            for (var i = 0; i < _threadcount; i++)
                _threads[i].Start();
        }

        protected QueuedExecution(int threads)
        {
            _threadcount = threads;
            Initialize();
        }

        /// <summary>
        /// Execute call in queue, block until processed
        /// </summary>
        /// <param name="item"></param>
        protected void Execute(IItem item)
        {
            var resetevent = new ManualResetEventSlim();
            var qi = new QueueItem
                {
                    Item = item,
                    resetEvent = resetevent
                };
            _queue.Enqueue(qi);
            resetevent.Wait();

            if (qi.ProcessResult == ProcessResult.FailThrow)
                throw new Exception("execution failed");
        }

        #region IDisposable Members

        /// <summary>
        /// cleanup
        /// </summary>
        /// <param name="waitForFinish">should wait for process to finish 
        /// currently executing request or abort immediately</param>
        /// <param name="wait">time to wait for abort to finish</param>
        public void Dispose(bool waitForFinish,TimeSpan wait)
        {
            _threadaborted = true;
             bool allaborted = true;
            if (waitForFinish)
            {
                //wait for timeout, check if threads aborted gracefully in that time
                while ((DateTime.Now + wait) > DateTime.Now)
                {
                    allaborted = true;
                    foreach (var t in _threads)
                    {
                        if (t.IsAlive == true)
                        {
                            allaborted = false;
                            break;
                        }
                    }
                    if (allaborted == true)
                        break;

                    Thread.Sleep(1);
                }
            }

            //if not all threads were aborted, abort them
            if (allaborted == false)
            {
                foreach (var t in _threads)
                    if (t.IsAlive)
                        t.Abort();
            }
        }

        public void Dispose()
        {
            Dispose(false,TimeSpan.MinValue);
        }

        #endregion
    }
}
