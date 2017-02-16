using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace UmtUtilities.Collections
{
    public interface IReadOnlyObservableList<out T> : IReadOnlyList<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
    }
}