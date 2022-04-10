// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VectorFunctions.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Extensions;
using DspSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Algorithms
{
    public static class VectorFunctions
    {
        /// <summary>
        /// Calculates the mean square of a sequence.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        public static double MeanSquare(this IEnumerable<double> input)
        {
            return input.Aggregate(0.0, (d, d1) => d + d1 * d1);
        }

        /// <summary>
        /// Computes the logarithm of a sequence element-wise.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="newBase">The logarithm base.</param>
        public static IEnumerable<double> Log(this IEnumerable<double> input, double newBase = 10)
        {
            if (newBase <= 0)
                throw new ArgumentOutOfRangeException(nameof(newBase));

            return input.Select(d => Math.Log(d, newBase));
        }

        /// <summary>
        /// Computes the logarithm of a sequence element-wise.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="newBase">The logarithm base.</param>
        public static ILazyReadOnlyCollection<double> Log(this IReadOnlyCollection<double> input, double newBase = 10)
        {
            if (newBase <= 0)
                throw new ArgumentOutOfRangeException(nameof(newBase));

            return input.SelectWithCount(d => Math.Log(d, newBase));
        }

        /// <summary>
        /// Computes the logarithm of a sequence element-wise.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="newBase">The logarithm base.</param>
        public static ILazyReadOnlyList<double> Log(this IReadOnlyList<double> input, double newBase = 10)
        {
            if (newBase <= 0)
                throw new ArgumentOutOfRangeException(nameof(newBase));

            return input.SelectIndexed(d => Math.Log(d, newBase));
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on the given projection.
        /// </summary>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The maximal element, according to the projection.</returns>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, null);
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on the given projection and the specified comparer for projected values.
        /// </summary>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            comparer ??= Comparer<TKey>.Default;

            using var sourceIterator = source.GetEnumerator();

            if (!sourceIterator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            var max = sourceIterator.Current;
            var maxKey = selector(max);
            while (sourceIterator.MoveNext())
            {
                var candidate = sourceIterator.Current;
                var candidateProjected = selector(candidate);
                if (comparer.Compare(candidateProjected, maxKey) > 0)
                {
                    max = candidate;
                    maxKey = candidateProjected;
                }
            }

            return max;
        }

        /// <summary>
        /// Returns the index of the element with the maximum absolute value of a sequence.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        public static int AbsMaxIndex(this IEnumerable<double> sequence)
        {
            var currentMax = 0d;
            var currentIndex = -1;
            var i = 0;
            foreach (var item in sequence)
            {
                var abs = Math.Abs(item);
                if (abs > currentMax)
                {
                    currentMax = abs;
                    currentIndex = i;
                }

                i++;
            }

            return currentIndex;
        }

        /// <summary>
        /// Returns the index of the element with the minimum absolute value of a sequence.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        public static int AbsMinIndex(this IEnumerable<double> sequence)
        {
            var currentMin = double.PositiveInfinity;
            var currentIndex = -1;
            var i = 0;
            foreach (var item in sequence)
            {
                var abs = Math.Abs(item);
                if (abs < currentMin)
                {
                    currentMin = abs;
                    currentIndex = i;
                }

                i++;
            }

            return currentIndex;
        }

        /// <summary>
        /// Returns the minimum absolute value of a sequence.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        public static double AbsMin(this IEnumerable<double> sequence)
        {
            return sequence.Min(Math.Abs);
        }

        /// <summary>
        /// Returns the maximum absolute value of a sequence.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        public static double AbsMax(this IEnumerable<double> sequence)
        {
            return sequence.Max(Math.Abs);
        }

        /// <summary>
        /// Returns the index of the element with the minimum value of a sequence.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        public static int MinIndex(this IEnumerable<double> sequence)
        {
            var currentMin = double.PositiveInfinity;
            var currentIndex = 0;
            var i = 0;
            foreach (var item in sequence)
            {
                if (!double.IsNaN(item) && item.CompareTo(currentMin) < 0)
                {
                    currentMin = item;
                    currentIndex = i;
                }

                i++;
            }

            return currentIndex;
        }

        /// <summary>
        /// Returns the minimum difference of two elements with the same index of two sequences.
        /// </summary>
        /// <param name="sequence1">The first sequence.</param>
        /// <param name="sequence2">The second sequence.</param>
        /// <remarks>The sequences must be the same length.</remarks>
        public static double MinimumDifference(this IEnumerable<double> sequence1, IEnumerable<double> sequence2)
        {
            return sequence1
                .ZipExact(sequence2, (d, d1) => d - d1)
                .Min();
        }

        /// <summary>
        /// Returns the minimum difference of two elements with the same index of two sequences.
        /// </summary>
        /// <param name="sequence1">The first sequence.</param>
        /// <param name="sequence2">The second sequence.</param>
        /// <remarks>The sequences must be the same length.</remarks>
        public static int MinimumDifferenceIndex(this IEnumerable<double> sequence1, IEnumerable<double> sequence2)
        {
            return sequence1
                .ZipExact(sequence2, (d, d1) => d - d1)
                .MinIndex();
        }

        /// <summary>
        /// Returns a value indicating whether a sequence is strictly monotonic increasing, i.e. if each value is greater than the previous one.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
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
        /// Returns a value indicating whether a sequence is strictly monotonic decreasing, i.e. if each value is greater than the previous one.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        public static bool IsStrictlyMonotonicDecreasing(this IEnumerable<double> sequence)
        {
            var prev = double.PositiveInfinity;
            foreach (var d in sequence)
            {
                if (d >= prev)
                    return false;

                prev = d;
            }

            return true;
        }

        /// <summary>
        /// Raises the elements of a sequence to the specified power.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="power">The power.</param>
        public static IEnumerable<double> Pow(this IEnumerable<double> input, double power)
        {
            return input.Select(d => Math.Pow(power, d));
        }

        /// <summary>
        /// Raises the elements of a sequence to the specified power.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="power">The power.</param>
        public static ILazyReadOnlyCollection<double> Pow(this IReadOnlyCollection<double> input, double power)
        {
            return input.SelectWithCount(d => Math.Pow(power, d));
        }

        /// <summary>
        /// Raises the elements of a sequence to the specified power.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="power">The power.</param>
        public static ILazyReadOnlyList<double> Pow(this IReadOnlyList<double> input, double power)
        {
            return input.SelectIndexed(d => Math.Pow(power, d));
        }
    }
}