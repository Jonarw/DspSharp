// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VectorConversions.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UTilities.Extensions;

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

            if (sequence is T[] array)
                return array;

            return sequence.ToArray();
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

            if (sequence is IList<T> ilist)
            {
                if (ilist.Count <= maximumLength)
                    return new ReadOnlyCollection<T>(ilist);

                return ilist.Take(maximumLength).ToReadOnlyList();
            }

            if (sequence is IReadOnlyList<T> irolist)
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