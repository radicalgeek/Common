using System;
using System.Collections.Generic;
using System.Linq;

namespace RadicalGeek.Common.Collections
{
    public sealed class CacheList<TKey1, TKey2, TValue> : CacheList<Tuple<TKey1, TKey2>, TValue>
        where TKey1 : struct
        where TKey2 : struct
    {
    }

    public sealed class CacheList<TKey1, TKey2, TKey3, TValue> : CacheList<Tuple<TKey1, TKey2, TKey3>, TValue>
        where TKey1 : struct
        where TKey2 : struct
        where TKey3 : struct
    {
    }

    public class CacheList<TKey, TValue>
    {
        private readonly bool enableTouch;
        private Func<TKey, TValue> getter;
        private readonly List<CacheRecord> internalList = new List<CacheRecord>();

        private readonly int maxCacheSize = int.MaxValue;

        private readonly TimeSpan timeout = TimeSpan.MaxValue;

        public CacheList() { }

        public CacheList(Func<TKey, TValue> getter)
        {
            this.getter = getter;
        }

        public CacheList(Func<TKey, TValue> getter, TimeSpan timeout)
            : this(getter)
        {
            this.timeout = timeout;
        }

        public CacheList(Func<TKey, TValue> getter, TimeSpan timeout, bool enableTouch)
            : this(getter, timeout)
        {
            this.enableTouch = enableTouch;
        }

        public CacheList(Func<TKey, TValue> getter, TimeSpan timeout, bool enableTouch, int maxCacheSize)
            : this(getter, timeout, enableTouch)
        {
            this.maxCacheSize = maxCacheSize;
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (this)
                {
                    CacheRecord cacheRecord;
                    if (!ContainsKey(key))
                    {
                        if (internalList.Count == maxCacheSize)
                            internalList.Remove(internalList.OrderBy(cr => cr.LastTouched).First());
                        cacheRecord = new CacheRecord(key, getter, timeout, enableTouch);
                        internalList.Add(cacheRecord);
                    }
                    else
                    {
                        cacheRecord = internalList.FirstOrDefault(cr => cr.Key.Equals(key));
                    }
                    return cacheRecord != null ? cacheRecord.Value : default(TValue);
                }
            }
            protected set
            {
                lock (this)
                {
                    if (!ContainsKey(key))
                    {
                        if (internalList.Count == maxCacheSize)
                            internalList.Remove(internalList.OrderBy(cr => cr.LastTouched).First());
                        CacheRecord cacheRecord = new CacheRecord(key, getter, timeout, enableTouch);
                        cacheRecord.Value = value;
                        internalList.Add(cacheRecord);
                    }
                }
            }
        }

        protected bool ContainsKey(TKey key)
        {
            return internalList.Any(cr => cr.Key.Equals(key));
        }
        public void Clear()
        {
            internalList.Clear();
        }
        /// <summary>
        /// Updates the Getter method for undiscovered or not-yet-loaded items.
        /// </summary>
        /// <param name="func"></param>
        public void SetGetterMethod(Func<TKey, TValue> func)
        {
            getter = func;
        }
        #region Nested type: CacheRecord

        private sealed class CacheRecord
        {
            private readonly bool enableTouch;
            private readonly Func<TKey, TValue> getterFunction;

            private readonly TKey key;

            private readonly TimeSpan timeout = TimeSpan.MaxValue;
            private bool hasValue;
            private DateTime lastTouched = DateTime.Now;
            private TValue value;

            public CacheRecord(TKey key, Func<TKey, TValue> getterFunction, TimeSpan timeout, bool enableTouch)
            {
                this.key = key;
                this.getterFunction = getterFunction;
                this.timeout = timeout;
                this.enableTouch = enableTouch;
            }

            public DateTime LastTouched
            {
                get { return lastTouched; }
            }

            public TKey Key
            {
                get { return key; }
            }

            public TValue Value
            {
                get
                {
                    if ((!hasValue || DateTime.Now - LastTouched > timeout))
                    {
                        value = getterFunction(Key);
                        hasValue = true;
                    }
                    if (enableTouch) lastTouched = DateTime.Now;
                    return value;
                }
                set
                {
                    this.value = value;
                    hasValue = true;
                    if (enableTouch) lastTouched = DateTime.Now;
                }
            }
        }

        #endregion


    }
}
