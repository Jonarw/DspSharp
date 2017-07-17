using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace DspSharp.Utilities.Collections
{
    /// <summary>
    ///     Represents an observable List.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.ObjectModel.ObservableCollection{T}" />
    /// <seealso cref="IObservableList{T}" />
    public class ObservableList<T> : ObservableCollection<T>, IObservableList<T>, IObservableList, IReadOnlyObservableList<T>
    {
        /// <summary>
        ///     Initializes a new empty instance of the <see cref="ObservableList{T}" /> class.
        /// </summary>
        public ObservableList()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableList{T}" /> class.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        public ObservableList(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableList{T}" /> class.
        /// </summary>
        /// <param name="list">The list from which the elements are copied.</param>
        public ObservableList(List<T> list)
            : base(list)
        {
        }

        /// <summary>
        ///     Adds the range of specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var rangelist = items.ToList();

            foreach (T item in rangelist)
            {
                this.Items.Add(item);
            }

            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Count)));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, rangelist));
        }

        public void MoveItem(T item, int newIndex)
        {
            if (newIndex > this.Items.Count - 1 || newIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(newIndex));

            var index = this.IndexOf(item);
            if (index == newIndex)
                return;

            this.Items.Remove(item);
            this.Items.Insert(newIndex, item);
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        ///     Removes the range of specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var rangelist = items.ToList();

            foreach (T item in rangelist)
            {
                this.Items.Remove(item);
            }

            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Count)));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, rangelist));
        }

        /// <summary>
        ///     Resets the the list with the specified new items.
        /// </summary>
        /// <param name="newItems">The new items.</param>
        public void Reset(IEnumerable<T> newItems)
        {
            this.Items.Clear();

            if (newItems != null)
            {
                foreach (T item in newItems)
                {
                    this.Items.Add(item);
                }
            }

            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Count)));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public event PropertyChangedEventHandler ItemPropertyChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in this.Items.OfType<INotifyPropertyChanged>())
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

            base.OnCollectionChanged(e);
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ItemPropertyChanged?.Invoke(sender, e);
        }
    }
}