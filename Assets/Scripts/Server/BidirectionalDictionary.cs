using System.Collections;
using System.Collections.Generic;

namespace Game.Server
{
    public class BidirectionalDictionary<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
    {
        private readonly Dictionary<T1, T2> _forward;
        private readonly Dictionary<T2, T1> _reverse;

        /// <summary>
        /// Constructor that uses the default comparers for the keys in each direction.
        /// </summary>
        public BidirectionalDictionary()
            : this(null, null)
        {
        }

        /// <summary>
        /// Constructor that defines the comparers to use when comparing keys in each direction.
        /// </summary>
        /// <param name="t1Comparer">Comparer for the keys of type T1.</param>
        /// <param name="t2Comparer">Comparer for the keys of type T2.</param>
        /// <remarks>Pass null to use the default comparer.</remarks>
        public BidirectionalDictionary(IEqualityComparer<T1> t1Comparer, IEqualityComparer<T2> t2Comparer)
        {
            _forward = new Dictionary<T1, T2>(t1Comparer);
            _reverse = new Dictionary<T2, T1>(t2Comparer);
            Forward = new Indexer<T1, T2>(_forward);
            Reverse = new Indexer<T2, T1>(_reverse);
        }

        public Indexer<T1, T2> Forward { get; }
        public Indexer<T2, T1> Reverse { get; }

        public Dictionary<T1, T2>.KeyCollection Keys => _forward.Keys;
        public Dictionary<T1, T2>.ValueCollection Values => _forward.Values;

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public void Remove(T1 t1)
        {
            T2 revKey = Forward[t1];
            _forward.Remove(t1);
            _reverse.Remove(revKey);
        }

        public void Remove(T2 t2)
        {
            T1 forwardKey = Reverse[t2];
            _reverse.Remove(t2);
            _forward.Remove(forwardKey);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator() => _forward.GetEnumerator();

        public bool ContainsKey(T1 t1) => _forward.ContainsKey(t1);
        public bool ContainsValue(T2 t2) => _reverse.ContainsKey(t2);

        public class Indexer<T3, T4>
        {
            private readonly Dictionary<T3, T4> _dictionary;

            public Indexer(Dictionary<T3, T4> dictionary) => _dictionary = dictionary;

            public T4 this[T3 index]
            {
                get => _dictionary[index];
                set => _dictionary[index] = value;
            }

            public bool Contains(T3 key) => _dictionary.ContainsKey(key);
        }
    }
}
