using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace DspSharp.Collections
{
    /// <summary>
    ///     Represents an observable List.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.ObjectModel.ObservableCollection{T}" />
    /// <seealso cref="IObservableList{T}" />
    public class ObservableList<T> : ObservableCollection<T>, IObservableList<T>
    {
        private bool _suspendUpdates;

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
        ///     Gets or sets a value indicating whether changes to the collection should invoke the corresponding events.
        /// </summary>
        /// <value>
        ///     <c>true</c> if [suspend updates]; otherwise, <c>false</c>.
        /// </value>
        public bool SuspendUpdates
        {
            get { return this._suspendUpdates; }
            set
            {
                if ((value != this._suspendUpdates) && (value == false))
                {
                    this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Count)));
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }

                this._suspendUpdates = value;
            }
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

            foreach (var item in rangelist)
            {
                this.Items.Add(item);
            }

            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Count)));
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

            foreach (var item in rangelist)
            {
                this.Items.Remove(item);
            }

            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Count)));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        ///     Resets the the list with the specified new items.
        /// </summary>
        /// <param name="newItems">The new items.</param>
        public void Reset(IEnumerable<T> newItems)
        {
            this.Items.Clear();

            if (newItems != null)
                this.AddRange(newItems);
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged" /> event with the
        ///     provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!this.SuspendUpdates)
                base.OnCollectionChanged(e);
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.PropertyChanged" /> event with the
        ///     provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (!this.SuspendUpdates)
                base.OnPropertyChanged(e);
        }
    }
}