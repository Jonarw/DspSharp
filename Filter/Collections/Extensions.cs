using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Filter.Collections
{
    public static class Extensions
    {
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T element, IEqualityComparer<T> comparer = null)
        {
            int i = 0;
            comparer = comparer ?? EqualityComparer<T>.Default;
            foreach (var currentElement in enumerable)
            {
                if (comparer.Equals(currentElement, element))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        public static IReadOnlyObservableList<T> ToReadOnlyObservableList<T>(this ObservableCollection<T> input)
        {
            return new ReadOnlyObservableList<T>(input);
        }

        public static IReadOnlyObservableList<T> ToReadOnlyObservableList<T>(this IObservableList<T> input)
        {
            return new ReadOnlyObservableList<T>((ObservableCollection<T>)input);
        }

        public static IReadOnlyObservableList<T> ToReadOnlyObservableList<T>(this ObservableList<T> input)
        {
            return new ReadOnlyObservableList<T>(input);
        }
    }
}