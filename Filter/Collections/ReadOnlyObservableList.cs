using System.Collections.ObjectModel;
using UmtUtilities.Collections;

namespace Filter.Collections
{
    public class ReadOnlyObservableList<T> : ReadOnlyObservableCollection<T>, IReadOnlyObservableList<T>
    {
        public ReadOnlyObservableList(ObservableCollection<T> list) : base(list)
        {
        }
    }
}