using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DspSharp.Utilities.Collections
{
    public interface IObservableList : IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }

    /// <summary>
    ///     Describes an observable list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Generic.IList{T}" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    /// <seealso cref="System.Collections.Specialized.INotifyCollectionChanged" />
    public interface IObservableList<T> : IList<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        /// <summary>
        ///     Adds the range of specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        void AddRange(IEnumerable<T> items);

        /// <summary>
        ///     Occurs when a property of an item in the collection has changed.
        /// </summary>
        event PropertyChangedEventHandler ItemPropertyChanged;

        /// <summary>
        ///     Moves an item to the specified index.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="newIndex">The new index of the item after moving.</param>
        void MoveItem(T item, int newIndex);

        /// <summary>
        ///     Removes the range of specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        void RemoveRange(IEnumerable<T> items);

        /// <summary>
        ///     Resets the the list with the specified new items.
        /// </summary>
        /// <param name="newItems">The new items.</param>
        void Reset(IEnumerable<T> newItems);
    }
}