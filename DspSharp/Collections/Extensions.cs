// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DspSharp.Collections
{
    public static class Extensions
    {
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T element, IEqualityComparer<T> comparer = null)
        {
            var i = 0;
            comparer = comparer ?? EqualityComparer<T>.Default;
            foreach (var currentElement in enumerable)
            {
                if (comparer.Equals(currentElement, element))
                    return i;

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