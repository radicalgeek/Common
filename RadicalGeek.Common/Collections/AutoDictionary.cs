using System.Collections.Generic;

namespace RadicalGeek.Common.Collections
{
    public class AutoDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public new TValue this[TKey key]
        {
            set
            {
                if (!ContainsKey(key))
                    Add(key, value);
                else
                    base[key] = value;
            }
            get {
                return ContainsKey(key) ? base[key] : default(TValue);
            }
        }
    }
}
