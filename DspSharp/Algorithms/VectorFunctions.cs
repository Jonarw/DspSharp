// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VectorFunctions.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Algorithms
{
    public static class VectorFunctions
    {
        /// <summary>
        ///     Calculates the energy of a sequence by summing up its squared elements.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <returns></returns>
        public static double CalculateEnergy(this IEnumerable<double> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Aggregate(0.0, (d, d1) => d + d1 * d1);
        }

        /// <summary>
        ///     Computes the logarithm of a sequence element-wise.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="newBase">The base.</param>
        /// <returns></returns>
        public static IEnumerable<double> Log(this IEnumerable<double> input, double newBase = 10)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (newBase <= 0)
                throw new ArgumentOutOfRangeException(nameof(newBase));

            return input.Select(d => Math.Log(d, newBase));
        }

        /// <summary>
        ///     Returns the maximal element of the given sequence, based on
        ///     the given projection.
        /// </summary>
        /// <remarks>
        ///     If more than one element has the maximal projected value, the first
        ///     one encountered will be returned. This overload uses the default comparer
        ///     for the projected type. This operator uses immediate execution, but
        ///     only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> or <paramref name="selector" /> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source" /> is empty</exception>
        public static TSource MaxBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, null);
        }

        /// <summary>
        ///     Returns the maximal element of the given sequence, based on
        ///     the given projection and the specified comparer for projected values.
        /// </summary>
        /// <remarks>
        ///     If more than one element has the maximal projected value, the first
        ///     one encountered will be returned. This operator uses immediate execution, but
        ///     only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="source" />, <paramref name="selector" />
        ///     or <paramref name="comparer" /> is null
        /// </exception>
        /// <exception cref="InvalidOperationException"><paramref name="source" /> is empty</exception>
        public static TSource MaxBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector,
            IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            comparer = comparer ?? Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Sequence contains no elements");
                TSource max = sourceIterator.Current;
                TKey maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }

        /// <summary>
        ///     Finds the index with the maximum value in a sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <returns></returns>
        public static int MaxIndex<T>(this IEnumerable<T> sequence)
            where T : IComparable<T>
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            var sequencelist = sequence.ToReadOnlyList();
            if (sequencelist.Count == 0)
                return -1;

            var index = -1;
            var maxValue = sequencelist.First(); // Immediately overwritten anyway

            return sequencelist.Aggregate(
                0,
                (i, value) =>
                {
                    index++;
                    if (value.CompareTo(maxValue) > 0)
                    {
                        maxValue = value;
                        return index;
                    }

                    return i;
                });
        }

        /// <summary>
        ///     Finds the index with the minimum value in a sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <returns></returns>
        public static int MinIndex<T>(this IEnumerable<T> sequence)
            where T : IComparable<T>
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            var sequencelist = sequence.ToReadOnlyList();
            if (sequencelist.Count == 0)
                return -1;

            var index = -1;
            var minValue = sequencelist.First(); // Immediately overwritten anyway

            return sequencelist.Aggregate(
                0,
                (i, value) =>
                {
                    index++;
                    if (value.CompareTo(minValue) < 0)
                    {
                        minValue = value;
                        return index;
                    }

                    return i;
                });
        }

        public static double MinimumDifference(this IEnumerable<double> sequence1, IEnumerable<double> sequence2)
        {
            return sequence1.Zip(sequence2, (d, d1) => d - d1).Min();
        }

        public static int MinimumDifferenceIndex(this IEnumerable<double> sequence1, IEnumerable<double> sequence2)
        {
            return sequence1.Zip(sequence2, (d, d1) => d - d1).MinIndex();
        }

        public static bool IsStrictlyMonotonicIncreasing(this IEnumerable<double> sequence)
        {
            var prev = double.NegativeInfinity;
            foreach (var d in sequence)
            {
                if (d <= prev)
                    return false;

                prev = d;
            }

            return true;
        }

        /// <summary>
        ///     Raises the elements of a sequence to the specified power.
        /// </summary>
        /// <param name="power">The power.</param>
        /// <param name="input">The sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> Pow(this IEnumerable<double> input, double power)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(d => Math.Pow(power, d));
        }
    }
}