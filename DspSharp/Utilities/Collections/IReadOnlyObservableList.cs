using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DspSharp.Utilities.Collections
{
    public interface IReadOnlyObservableList<out T> : IReadOnlyList<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        /// <summary>
        ///     Occurs when a property of an item in the collection has changed.
        /// </summary>
        event PropertyChangedEventHandler ItemPropertyChanged;
    }
}