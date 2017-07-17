// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VectorConversions.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DspSharp.Algorithms
{
    public static class VectorConversions
    {
        /// <summary>
        ///     Converts the specified sequence to an Array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static T[] ToArrayOptimized<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            var array = sequence as T[];

            if (array != null)
                return array;

            return sequence.ToArray();
        }

        /// <summary>
        ///     Creates an enumerable containing a single element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumerable<T>(this T element)
        {
            return Enumerable.Repeat(element, 1);
        }

        /// <summary>
        ///     Returns a readonly list containing the specified sequence, evaluating it if necessary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            var ilist = sequence as IList<T>;
            if (ilist != null)
                return new ReadOnlyCollection<T>(ilist);

            var irlist = sequence as IReadOnlyList<T>;
            if (irlist != null)
                return irlist;

            return new ReadOnlyCollection<T>(sequence.ToList());
        }

        /// <summary>
        ///     Returns a readonly list containing the specified sequence, evaluating it if necessary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <param name="maximumLength">The maximum evaluation length.</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> sequence, int maximumLength)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            var ilist = sequence as IList<T>;
            if (ilist != null)
            {
                if (ilist.Count <= maximumLength)
                    return new ReadOnlyCollection<T>(ilist);

                return ilist.Take(maximumLength).ToReadOnlyList();
            }

            var irolist = sequence as IReadOnlyList<T>;
            if (irolist != null)
            {
                if (irolist.Count <= maximumLength)
                    return irolist;

                return irolist.Take(maximumLength).ToReadOnlyList();
            }

            var i = 0;
            var ret = new List<T>();
            using (var e = sequence.GetEnumerator())
            {
                while (e.MoveNext() && (i < maximumLength))
                {
                    ret.Add(e.Current);
                    i++;
                }
            }

            return ret.AsReadOnly();
        }
    }
}