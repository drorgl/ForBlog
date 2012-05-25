/// Thread safe IDictionary implementation.
/// Lorenz Cuno Klopfenstein <lck@klopfenstein.net>
/// http://lorenz.klopfenstein.net
///
/// This appears to work correctly, it has not been proven to work, use at own risk. :)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Babil.Inject {

    public class SafeDictionary<TId, TValue> : IDictionary<TId, TValue>
        where TValue : class {

        ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        Dictionary<TId, TValue> _dict = new Dictionary<TId, TValue>();

        #region IDictionary<TId,TValue> Members

        public void Add(TId key, TValue value) {
            DoWithWriterLock(() => {
                _dict.Add(key, value);
            });
        }

        public bool ContainsKey(TId key) {
            bool ret = false;

            DoWithReaderLock(() => {
                ret = _dict.ContainsKey(key);
            });

            return ret;
        }

        public ICollection<TId> Keys {
            get {
                TId[] ret = null;

                DoWithReaderLock(() => {
                    ret = (from ele in _dict
                           select ele.Key).ToArray();
                });

                return ret;
            }
        }

        public bool Remove(TId key) {
            bool ret = false;

            DoWithWriterLock(() => {
                ret = _dict.Remove(key);
            });

            return ret;
        }

        public bool TryGetValue(TId key, out TValue value) {
            TValue outVal = null;
            bool ret = false;

            DoWithReaderLock(() => {
                ret = _dict.TryGetValue(key, out outVal);
            });

            value = outVal;
            return ret;
        }

        public ICollection<TValue> Values {
            get {
                TValue[] ret = null;

                DoWithWriterLock(() => {
                    ret = new TValue[_dict.Count];

                    int i = 0;
                    foreach (var couple in _dict) {
                        ret[i] = couple.Value;
                        ++i;
                    }
                });

                return ret;
            }
        }

        public TValue this[TId key] {
            get {
                TValue ret = null;

                DoWithReaderLock(() => {
                    ret = _dict[key];
                });

                return ret;
            }
            set {
                DoWithWriterLock(() => {
                    _dict[key] = value;
                });
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TId,TValue>> Members

        public void Add(KeyValuePair<TId, TValue> item) {
            Add(item.Key, item.Value);
        }

        public void Clear() {
            DoWithWriterLock(() => {
                _dict.Clear();
            });
        }

        public bool Contains(KeyValuePair<TId, TValue> item) {
            bool ret = false;

            DoWithReaderLock(() => {
                ret = _dict.Contains(item);
            });

            return ret;
        }

        public void CopyTo(KeyValuePair<TId, TValue>[] array, int arrayIndex) {
            DoWithReaderLock(() => {
                int arrLen = array.Length - arrayIndex;
                if (arrLen < _dict.Count)
                    throw new ArgumentException("Array too short.");

                int i = arrayIndex;
                foreach (var couple in _dict) {
                    array[i] = couple;
                    ++arrayIndex;
                }
            });
        }

        public int Count {
            get {
                int ret = 0;

                DoWithReaderLock(() => {
                    ret = _dict.Count;
                });

                return ret;
            }
        }

        public bool IsReadOnly {
            get {
                return false;
            }
        }

        public bool Remove(KeyValuePair<TId, TValue> item) {
            bool ret = false;

            DoWithWriterLock(() => {
                ret = _dict.Remove(item.Key);
            });

            return ret;
        }

        #endregion

        #region IEnumerable<KeyValuePair<TId,TValue>> Members

        public IEnumerator<KeyValuePair<TId, TValue>> GetEnumerator() {
            _lock.EnterReadLock();
            return new LockedEnumerator<TId, TValue>(_lock, _dict.GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            _lock.EnterReadLock();
            return new LockedEnumerator<TId, TValue>(_lock, _dict.GetEnumerator());
        }

        #endregion

        /// <summary>
        /// Add a value with a certain key only if no other value with the same key is already present.
        /// </summary>
        public void AddIfDoesntContain(TId key, TValue value) {
            DoWithWriterLock(() => {
                if (!_dict.ContainsKey(key))
                    _dict.Add(key, value);
            });
        }

        /// <summary>
        /// Returns a dictionary with an exclusive write lock on the original collection.
        /// Can be used to get quick and coherent access to the dictionary, without fine grained locking on each method.
        /// </summary>
        public LockedDictionary<TId, TValue> GetExclusiveDictionary() {
            _lock.EnterWriteLock();
            return new LockedDictionary<TId, TValue>(_dict, _lock);
        }

        /// <summary>
        /// Allows to manipulate the dictionary directly after having acquired an exclusive write lock.
        /// </summary>
        public void ManipulateWithWriteLock(Action<IDictionary<TId, TValue>> operation) {
            DoWithWriterLock(() => {
                operation(_dict);
            });
        }

        /// <summary>
        /// Allows to manipulate the dictionary directly after having acquired a read lock.
        /// Be extra careful to NOT change the dictionary while accessing it.
        /// </summary>
        public void ManipulateWithReadLock(Action<IDictionary<TId, TValue>> operation) {
            DoWithReaderLock(() => {
                operation(_dict);
            });
        }

        #region Helpers

        private void DoWithReaderLock(Action action) {
            _lock.EnterReadLock();
            try {
                action();
            }
            finally {
                _lock.ExitReadLock();
            }
        }

        private void DoWithWriterLock(Action action) {
            _lock.EnterWriteLock();
            try {
                action();
            }
            finally {
                _lock.ExitWriteLock();
            }
        }

        #endregion
    }

    /// <summary>
    /// Simple wrapper around a dictionary that releases a write lock when disposed.
    /// </summary>
    public class LockedDictionary<TId, TValue> : IDictionary<TId, TValue>, IDisposable {

        IDictionary<TId, TValue> _dict;
        ReaderWriterLockSlim _lock;

        public LockedDictionary(IDictionary<TId, TValue> dict, ReaderWriterLockSlim lck) {
            _dict = dict;
            _lock = lck;
        }
        
        #region IDictionary<TId,TValue> Members

        public void Add(TId key, TValue value) {
            _dict.Add(key, value);
        }

        public bool ContainsKey(TId key) {
            return _dict.ContainsKey(key);
        }

        public ICollection<TId> Keys {
            get { return _dict.Keys; }
        }

        public bool Remove(TId key) {
            return _dict.Remove(key);
        }

        public bool TryGetValue(TId key, out TValue value) {
            return _dict.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values {
            get { return _dict.Values; }
        }

        public TValue this[TId key] {
            get {
                return _dict[key];
            }
            set {
                _dict[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TId,TValue>> Members

        public void Add(KeyValuePair<TId, TValue> item) {
            _dict.Add(item);
        }

        public void Clear() {
            _dict.Clear();
        }

        public bool Contains(KeyValuePair<TId, TValue> item) {
            return _dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<TId, TValue>[] array, int arrayIndex) {
            _dict.CopyTo(array, arrayIndex);
        }

        public int Count {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly {
            get { return _dict.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TId, TValue> item) {
            return _dict.Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TId,TValue>> Members

        public IEnumerator<KeyValuePair<TId, TValue>> GetEnumerator() {
            return _dict.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _dict.GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            _lock.ExitWriteLock();
        }

        #endregion
    }

    /// <summary>
    /// Wrapper around an IEnumerator that releases a lock when disposed.
    /// </summary>
    class LockedEnumerator<TId, TValue> : IEnumerator<KeyValuePair<TId, TValue>>, System.Collections.IEnumerator {

        public LockedEnumerator(ReaderWriterLockSlim lockedLock, IEnumerator<KeyValuePair<TId, TValue>> enumerator) {
            _lock = lockedLock;
            _enumerator = enumerator;
        }

        ReaderWriterLockSlim _lock;
        IEnumerator<KeyValuePair<TId, TValue>> _enumerator;

        #region IEnumerator<TValue> Members

        public KeyValuePair<TId, TValue> Current {
            get {
                return _enumerator.Current;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            _enumerator.Dispose();
            _lock.ExitReadLock();
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current {
            get {
                return _enumerator.Current;
            }
        }

        public bool MoveNext() {
            return _enumerator.MoveNext();
        }

        public void Reset() {
            _enumerator.Reset();
        }

        #endregion

        #region IEnumerator Members


        bool System.Collections.IEnumerator.MoveNext() {
            return _enumerator.MoveNext();
        }

        void System.Collections.IEnumerator.Reset() {
            _enumerator.Reset();
        }

        #endregion
    }

}
