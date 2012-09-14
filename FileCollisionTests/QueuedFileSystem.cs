using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FileCollisionTests
{
    /// <summary>
    /// Queued File access based on QueuedExecution
    /// </summary>
    public class QueuedFileSystem : QueuedExecution
    {
        /// <summary>
        /// Internal structure for operations
        /// </summary>
        private class FileOperationItem : IItem
        {
            public enum OperationType
            {
                Save,
                Load
            }

            public OperationType Type { get; set; }

            public string Filename { get; set; }

            public Stream input { get; set; }
            public Stream output { get; set; }


        }

        /// <summary>
        /// File access collision avoidance
        /// </summary>
        private DictionaryLock<string> _filenamelock = new DictionaryLock<string>();

        public QueuedFileSystem(int workerthreadcount)
            : base(workerthreadcount)
        {
            
        }

        /// <summary>
        /// queue processor
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override QueuedExecution.ProcessResult ProcessQueue(IItem item)
        {
            FileOperationItem foi = (FileOperationItem)item;
            if (foi.Type == FileOperationItem.OperationType.Load)
            {
                if (_filenamelock.IsReaderLocked(foi.Filename))
                    return QueuedExecution.ProcessResult.FailRequeue;

                _filenamelock.EnterReader(foi.Filename);
                foi.output = LockingFileSystem.LoadStream(foi.Filename);//, int.MaxValue, TimeSpan.FromMilliseconds(1));
                _filenamelock.ExitReader(foi.Filename);
                
                return ProcessResult.Success;
            }
            else if (foi.Type == FileOperationItem.OperationType.Save)
            {
                _filenamelock.EnterWriter(foi.Filename);
                LockingFileSystem.SaveStream(foi.Filename, foi.input);//, int.MaxValue, TimeSpan.FromMilliseconds(1));
                _filenamelock.ExitWriter(foi.Filename);
                return ProcessResult.Success;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves a stream to file
        /// </summary>
        public void SaveStream(string filename, Stream stream)
        {
            var foi = new FileOperationItem
            {
                Type = FileOperationItem.OperationType.Save,
                Filename = filename,
                input = stream
            };
            base.Execute(foi);
        }

        /// <summary>
        /// Loads a stream from file
        /// </summary>
        public Stream LoadStream(string filename)
        {
            var foi = new FileOperationItem
            {
                Type = FileOperationItem.OperationType.Load,
                Filename = filename
            };
            
            base.Execute(foi);

            return foi.output;
        }

        #region IDisposable Members

        public new void Dispose()
        {
            base.Dispose(true, TimeSpan.FromSeconds(1));
        }

        #endregion

    }
}
