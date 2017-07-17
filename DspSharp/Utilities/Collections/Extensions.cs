using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Utilities.Collections
{
    public static class Extensions
    {
        private static readonly IDictionary<IObservableList, IList<IObservableList>> OfTypeDictionary =
            new Dictionary<IObservableList, IList<IObservableList>>();

        public static int IndexOf<T>(this IEnumerable<T> enumerable, T element, IEqualityComparer<T> comparer = null)
        {
            int i = 0;
            comparer = comparer ?? EqualityComparer<T>.Default;
            foreach (var currentElement in enumerable)
            {
                if (comparer.Equals(currentElement, element))
                    return i;

                i++;
            }

            return -1;
        }

        public static IObservableList<T> OfTypeObservable<T>(this IObservableList source)
        {
            IList<IObservableList> typelist;
            if (OfTypeDictionary.ContainsKey(source))
                typelist = OfTypeDictionary[source];
            else
            {
                typelist = new List<IObservableList>();
                OfTypeDictionary.Add(source, typelist);
            }

            var list = typelist.OfType<IObservableList<T>>().FirstOrDefault();
            if (list != null)
                return list;

            var ret = new ObservableList<T>();
            source.CollectionChanged += (sender, e) => ret.Reset(source.OfType<T>());
            ret.Reset(source.OfType<T>());
            typelist.Add(ret);
            return ret;
        }

        public static IReadOnlyObservableList<T> ToReadOnlyObservableList<T>(this IObservableList<T> input)
        {
            return new ReadOnlyObservableList<T>(input);
        }
    }
}