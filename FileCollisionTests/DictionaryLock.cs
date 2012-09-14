using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace FileCollisionTests
{
    /// <summary>
    /// Dictionary of locks on TKey
    /// </summary>
    /// <typeparam name="TKey">Type of key</typeparam>
    public class DictionaryLock<TKey>
    {
        /// <summary>
        /// Dictionary of locks container
        /// </summary>
        private ConcurrentDictionary<TKey, ReaderWriterLockSlim> _locks = new ConcurrentDictionary<TKey, ReaderWriterLockSlim>();

        /// <summary>
        /// _locks updating lock
        /// </summary>
        private SpinLock _operationlock = new SpinLock();

        /// <summary>
        /// Retrieves the ReaderWriterLock for a specific key
        /// </summary>
        private ReaderWriterLockSlim GetLock(TKey key)
        {
            //check if lock exist
            ReaderWriterLockSlim localock;
            if (_locks.TryGetValue(key, out localock))
            {
                return localock;
            }

            //it doesn't exist, lets create it

            bool lockTaken = false;
            _operationlock.Enter(ref lockTaken);

            //after acquired write lock, recheck its not in the dictionary if two writes were attempted for the same key
            if (!_locks.TryGetValue(key, out localock))
            {
                localock = new ReaderWriterLockSlim();
                _locks[key] = localock;
            }
            _operationlock.Exit();

            return localock;
        }


        /// <summary>
        /// Enter Reader lock on key
        /// </summary>
        public void EnterReader(TKey key)
        {
            var localock = GetLock(key);

            localock.EnterReadLock();
        }

        /// <summary>
        /// Enter Writer lock on key
        /// </summary>
        public void EnterWriter(TKey key)
        {
            var localock = GetLock(key);

            localock.EnterWriteLock();
        }

        /// <summary>
        /// Check Reader locked on key
        /// </summary>
        public bool IsReaderLocked(TKey key)
        {
            ReaderWriterLockSlim localock;
            if (_locks.TryGetValue(key, out localock))
                return localock.IsReadLockHeld;
            return false;
        }

        /// <summary>
        /// Check Writer locked on key
        /// </summary>
        public bool IsWriterLocked(TKey key)
        {
            ReaderWriterLockSlim localock;
            if (_locks.TryGetValue(key, out localock))
                return localock.IsWriteLockHeld;
            return false;
        }

        /// <summary>
        /// Exit Reader lock on key
        /// </summary>
        public void ExitReader(TKey key)
        {
            ReaderWriterLockSlim localock;
            if (_locks.TryGetValue(key, out localock))
                localock.ExitReadLock();
        }

        /// <summary>
        /// Exit Writer lock on key
        /// </summary>
        public void ExitWriter(TKey key)
        {
            ReaderWriterLockSlim localock;
            if (_locks.TryGetValue(key, out localock))
                localock.ExitWriteLock();
        }
    }
}
