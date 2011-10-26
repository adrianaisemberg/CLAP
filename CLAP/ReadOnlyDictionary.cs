using System.Collections.Generic;

namespace CLAP
{
    public class ReadOnlyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        #region Properties

        protected Dictionary<TKey, TValue> Dict { get; set; }

        public int Count
        {
            get { return Dict.Count; }
        }

        public ICollection<TKey> Keys
        {
            get { return Dict.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return Dict.Values; }
        }

        #endregion Properties

        #region Methods

        public bool ContainsKey(TKey key)
        {
            return Dict.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dict.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                return Dict[key];
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dict.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Dict.GetEnumerator();
        }

        #endregion Methods
    }
}