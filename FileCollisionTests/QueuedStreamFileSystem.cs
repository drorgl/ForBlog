using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;

namespace FileCollisionTests
{
    /// <summary>
    /// Queued file access 
    /// </summary>
    public class QueuedStreamFileSystem : IDisposable
    {
        private class FileOperationItem
        {
            public enum OperationType
            {
                Save,
                Load
            }

            public OperationType Type { get; set; }

            public string Filename { get; set; }

            public Stream stream { get; set; }

            public ManualResetEventSlim resetevent { get; set; }
        }

        private ConcurrentQueue<FileOperationItem> _operations = new ConcurrentQueue<FileOperationItem>();
        private DictionaryLock<string> _filenamelock = new DictionaryLock<string>();

        public List<Thread> _backgroundworkers = new List<Thread>();

        /// <summary>
        /// Queue Processor
        /// </summary>
        private void ProcessQueue()
            {
                while (true)
                {
                    FileOperationItem foi;
                    if (_operations.TryDequeue(out foi))
                    {
                        
                        if (foi.Type == FileOperationItem.OperationType.Load)
                        {
                            if (_filenamelock.IsReaderLocked(foi.Filename))
                            {
                                _operations.Enqueue(foi);
                                continue;
                            }
                            _filenamelock.EnterReader(foi.Filename);
                            foi.stream = LockingFileSystem.LoadStream(foi.Filename);//, int.MaxValue, TimeSpan.FromMilliseconds(1));
                            _filenamelock.ExitReader(foi.Filename);
                            foi.resetevent.Set();
                        }
                        else if (foi.Type == FileOperationItem.OperationType.Save)
                        {
                            if (_filenamelock.IsWriterLocked(foi.Filename))
                            {
                                _operations.Enqueue(foi);
                                continue;
                            }
                            _filenamelock.EnterWriter(foi.Filename);
                            LockingFileSystem.SaveStream(foi.Filename, foi.stream);//, int.MaxValue, TimeSpan.FromMilliseconds(1));
                            _filenamelock.ExitWriter(foi.Filename);
                            foi.resetevent.Set();
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
        }

        public QueuedStreamFileSystem(int threadcount)
        {
            int workerscount = threadcount;
            for (var i = 0; i < workerscount; i++)
            {
                _backgroundworkers.Add (new Thread(new ThreadStart(ProcessQueue)));
            }
            for (var i = 0; i < workerscount; i++)
                _backgroundworkers[i].Start();
        }


        /// <summary>
        /// Save a stream to file
        /// </summary>
        public void SaveStream(string filename, Stream stream)
        {
            var manualresetevent = new ManualResetEventSlim(false);
            
            _operations.Enqueue(new FileOperationItem
            {
                Type = FileOperationItem.OperationType.Save,
                Filename = filename,
                stream = stream,
                resetevent = manualresetevent
            });

            manualresetevent.Wait();
        }

        /// <summary>
        /// Load a stream from file
        /// </summary>
        public Stream LoadStream(string filename)
        {
            var manualresetevent = new ManualResetEventSlim(false);

            var foi = new FileOperationItem
            {
                Type = FileOperationItem.OperationType.Load,
                Filename = filename,
                resetevent = manualresetevent
            };
            _operations.Enqueue(foi);

            manualresetevent.Wait();

            return foi.stream;
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (var t in _backgroundworkers)
                t.Abort();
        }

        #endregion
    }
}
