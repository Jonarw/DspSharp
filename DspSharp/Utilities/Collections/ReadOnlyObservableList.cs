using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DspSharp.Utilities.Collections
{
    public class ReadOnlyObservableList<T> : IReadOnlyObservableList<T>
    {
        public ReadOnlyObservableList(IObservableList<T> list)
        {
            this.List = list;
        }

        private IObservableList<T> List { get; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.List).GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.List.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => this.List.CollectionChanged += value;
            remove => this.List.CollectionChanged -= value;
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => this.List.PropertyChanged += value;
            remove => this.List.PropertyChanged -= value;
        }

        public int Count => this.List.Count;

        public T this[int index] => this.List[index];

        public event PropertyChangedEventHandler ItemPropertyChanged
        {
            add => this.List.ItemPropertyChanged += value;
            remove => this.List.ItemPropertyChanged -= value;
        }
    }
}