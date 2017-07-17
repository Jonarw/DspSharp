using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DspSharp.Algorithms;

namespace DspSharp.Utilities.Collections
{
    public class SortedObservableList<TKey, TValue> : ISortedObservableList<TKey, TValue>, IReadOnlyList<TValue>, IList
    {
        public SortedObservableList(Func<TValue, TKey> keyFunction)
        {
            this.KeyFunction = keyFunction;
            this.InternalList = new SortedList<TKey, TValue>();
        }

        public SortedObservableList(IComparer<TKey> comparer, Func<TValue, TKey> keyFunction)
        {
            this.Comparer = comparer;
            this.KeyFunction = keyFunction;
            this.InternalList = new SortedList<TKey, TValue>(comparer);
        }

        private SortedList<TKey, TValue> InternalList { get; }

        void ICollection.CopyTo(Array array, int index)
        {
            this.CopyTo((TValue[])array, index);
        }

        bool ICollection.IsSynchronized => throw new NotSupportedException();

        object ICollection.SyncRoot => throw new NotSupportedException();

        public void Add(TValue item)
        {
            this.InternalList.Add(this.KeyFunction(item), item);

            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, this.InternalList.IndexOfValue(item)));
        }

        public void Clear()
        {
            this.InternalList.Clear();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(TValue item)
        {
            return this.InternalList.ContainsValue(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            this.InternalList.Values.CopyTo(array, arrayIndex);
        }

        public int Count => this.InternalList.Count;

        public bool IsReadOnly { get; } = false;

        public bool Remove(TValue item)
        {
            this.InternalList.Remove(this.KeyFunction(item));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return this.InternalList.Values.GetEnumerator();
        }

        int IList.Add(object value)
        {
            this.Add((TValue)value);
            return this.IndexOf((TValue)value);
        }

        bool IList.Contains(object value)
        {
            return this.Contains((TValue)value);
        }

        int IList.IndexOf(object value)
        {
            return this.IndexOf((TValue)value);
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        bool IList.IsFixedSize => false;

        object IList.this[int index]
        {
            get => this[index];
            set => throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            this.Remove((TValue)value);
        }

        public int IndexOf(TValue item)
        {
            return this.InternalList.IndexOfValue(item);
        }

        public void Insert(int index, TValue item)
        {
            throw new NotSupportedException();
        }

        TValue IList<TValue>.this[int index]
        {
            get => this.InternalList.Values[index];
            set
            {
                this.InternalList.Values[index] = value;
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public void RemoveAt(int index)
        {
            this.InternalList.Remove(this.KeyFunction(this[index]));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public void AddRange(IEnumerable<TValue> items)
        {
            var itemslist = items.ToList();
            var keyslist = itemslist.Select(this.KeyFunction).ToReadOnlyList();

            for (int i = 0; i < itemslist.Count; i++)
            {
                this.InternalList.Add(keyslist[i], itemslist[i]);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public event PropertyChangedEventHandler ItemPropertyChanged;

        public void MoveItem(TValue item, int newIndex)
        {
            throw new InvalidOperationException();
        }

        public void RemoveRange(IEnumerable<TValue> items)
        {
            var itemslist = items.ToList();
            foreach (var value in itemslist)
            {
                this.InternalList.Remove(this.KeyFunction(value));
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Reset(IEnumerable<TValue> items)
        {
            this.InternalList.Clear();

            var itemslist = items.ToList();
            var keyslist = itemslist.Select(this.KeyFunction).ToReadOnlyList();

            for (int i = 0; i < itemslist.Count; i++)
            {
                this.InternalList.Add(keyslist[i], itemslist[i]);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public TValue this[int index] => this.InternalList.Values[index];

        public IComparer<TKey> Comparer { get; }
        public Func<TValue, TKey> KeyFunction { get; }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in this.InternalList.Values.OfType<INotifyPropertyChanged>())
                {
                    item.PropertyChanged += this.OnItemPropertyChanged;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var item in e.OldItems.OfType<INotifyPropertyChanged>())
                {
                    item.PropertyChanged -= this.OnItemPropertyChanged;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var item in e.NewItems.OfType<INotifyPropertyChanged>())
                {
                    item.PropertyChanged += this.OnItemPropertyChanged;
                }
            }

            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Count)));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.CollectionChanged?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ItemPropertyChanged?.Invoke(sender, e);
        }
    }
}