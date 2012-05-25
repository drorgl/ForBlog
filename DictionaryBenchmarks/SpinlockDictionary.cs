
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DictionaryBenchmarks
{

    public class SpinlockDictionary<TId, TValue> : IDictionary<TId, TValue>
        where TValue : class {

        SpinLock spinlock = new SpinLock();

        Dictionary<TId, TValue> dict = new Dictionary<TId, TValue>();

            #region IDictionary<TId,TValue> Members


            public void Add(TId key, TValue value)
            {
                var lockTaken = false;
                spinlock.Enter(ref lockTaken);
                dict.Add(key, value);
                spinlock.Exit();
            }

            public bool ContainsKey(TId key)
            {
                var lockTaken = false;
                spinlock.Enter(ref lockTaken);
                var retval = dict.ContainsKey(key);
                spinlock.Exit();
                return retval;
            }

            public ICollection<TId> Keys
            {

                get { throw new NotImplementedException(); }
            }

            public bool Remove(TId key)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(TId key, out TValue value)
            {
                throw new NotImplementedException();
            }

            public ICollection<TValue> Values
            {
                get { throw new NotImplementedException(); }
            }

            public TValue this[TId key]
            {
                get
                {
                    var lockTaken = false;
                    spinlock.Enter(ref lockTaken);
                    var retval = dict[key];
                    spinlock.Exit();
                    
                    return retval;
                }
                set
                {
                    var lockTaken = false;
                    spinlock.Enter(ref lockTaken);
                    dict[key] = value;
                    spinlock.Exit();
                }
            }

            #endregion

            #region ICollection<KeyValuePair<TId,TValue>> Members

            public void Add(KeyValuePair<TId, TValue> item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<TId, TValue> item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<TId, TValue>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            public bool Remove(KeyValuePair<TId, TValue> item)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerable<KeyValuePair<TId,TValue>> Members

            public IEnumerator<KeyValuePair<TId, TValue>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

}
