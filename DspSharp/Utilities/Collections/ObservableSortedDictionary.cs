using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace DspSharp.Utilities.Collections
{
    public class ObservableSortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>,
        INotifyCollectionChanged
    {
        private ObservableList<TValue> _valuesSt = new ObservableList<TValue>();
        private SortedDictionary<TKey, TValue> InternalDictionary { get; } = new SortedDictionary<TKey, TValue>();

        public void CopyTo(Array array, int index)
        {
            ((ICollection)this.InternalDictionary).CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.InternalDictionary.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)this.InternalDictionary).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)this.InternalDictionary).SyncRoot; }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)this.InternalDictionary).Add(item);
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Clear()
        {
            this.InternalDictionary.Clear();
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.InternalDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this.InternalDictionary.CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count
        {
            get { return this.InternalDictionary.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)this.InternalDictionary).IsReadOnly; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            var ret = ((ICollection<KeyValuePair<TKey, TValue>>)this.InternalDictionary).Remove(item);
            if (ret)
                this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            return ret;
        }

        public void Add(object key, object value)
        {
            ((IDictionary)this.InternalDictionary).Add(key, value);
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(object key)
        {
            return ((IDictionary)this.InternalDictionary).Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)this.InternalDictionary).GetEnumerator();
        }

        bool IDictionary.IsFixedSize
        {
            get { return ((IDictionary)this.InternalDictionary).IsFixedSize; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return ((IDictionary)this.InternalDictionary).IsReadOnly; }
        }

        object IDictionary.this[object key]
        {
            get { return ((IDictionary)this.InternalDictionary)[key]; }
            set { ((IDictionary)this.InternalDictionary)[key] = value; }
        }

        ICollection IDictionary.Keys => this.InternalDictionary.Keys;

        public void Remove(object key)
        {
            ((IDictionary)this.InternalDictionary).Remove(key);
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        ICollection IDictionary.Values
        {
            get { return this.InternalDictionary.Values; }
        }

        public void Add(TKey key, TValue value)
        {
            this.InternalDictionary.Add(key, value);
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool ContainsKey(TKey key)
        {
            return this.InternalDictionary.ContainsKey(key);
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get { return this.InternalDictionary[key]; }
            set { this.InternalDictionary[key] = value; }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => this.InternalDictionary.Keys;

        public bool Remove(TKey key)
        {
            var ret = this.InternalDictionary.Remove(key);
            if (ret)
                this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            return ret;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.InternalDictionary.TryGetValue(key, out value);
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values => this.InternalDictionary.Values;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.InternalDictionary).GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return this.InternalDictionary.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => this.InternalDictionary.Count;

        public TValue this[TKey key] => this.InternalDictionary[key];

        public IEnumerable<TKey> Keys => this.InternalDictionary.Keys;
        public IEnumerable<TValue> Values => this.InternalDictionary.Values;

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            foreach (var item in items)
            {
                this.InternalDictionary.Add(item.Key, item.Value);
            }

            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void RemoveRange(IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                this.InternalDictionary.Remove(key);
            }

            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Reset(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            this.InternalDictionary.Clear();

            foreach (var item in items)
            {
                this.InternalDictionary.Add(item.Key, item.Value);
            }

            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}