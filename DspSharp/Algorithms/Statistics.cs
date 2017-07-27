// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Statistics.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Algorithms
{
    public static class Statistics
    {
        /// <summary>
        ///     Enumerates the different normalisation modes for computing Variance and Standard Deviation.
        /// </summary>
        public enum NormalisationMode
        {
            /// <summary>
            ///     Population mode; normalisation by N.
            /// </summary>
            Population,

            /// <summary>
            ///     Sample mode; normalisation by N-1.
            /// </summary>
            Sample
        }

        //TODO: unit test
        public static double GetCrestFactor(this IReadOnlyList<double> values)
        {
            return 1 / values.Normalize().Rms();
        }

        /// <summary>
        ///     Calculates the mean of the specified sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <returns>The mean of the sequence.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static double Mean(this IEnumerable<double> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Average();
        }

        /// <summary>
        ///     Gets the median.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Cannot compute median for an empty set.</exception>
        //TODO: unit test
        public static double Median(this IEnumerable<double> source)
        {
            var enumerable = source.ToReadOnlyList();

            if (enumerable.Count == 0)
                throw new InvalidOperationException("Cannot compute median for an empty set.");

            var sortedList = enumerable.OrderBy(number => number);

            var itemIndex = sortedList.Count() / 2;

            if (sortedList.Count() % 2 == 0)
                return (sortedList.ElementAt(itemIndex) + sortedList.ElementAt(itemIndex - 1)) / 2;

            return sortedList.ElementAt(itemIndex);
        }

        /// <summary>
        ///     Gets the median.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="numbers">The numbers.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        //TODO: unit test
        public static double Median<T>(this IEnumerable<T> numbers, Func<T, double> selector)
        {
            return numbers.Select(selector).Median();
        }

        //TODO: unit test
        public static double Rms(this IEnumerable<double> values)
        {
            var valueslist = values.ToReadOnlyList();
            return Math.Sqrt(valueslist.Aggregate(0d, (d, d1) => d + Math.Pow(d1, 2)) / valueslist.Count);
        }

        /// <summary>
        ///     Calculates the standard deviation of a sequence.
        /// </summary>
        /// <param name="values">The sequence.</param>
        /// <param name="mode">The normalisation mode.</param>
        /// <returns>The standard deviation of the sequence.</returns>
        public static double StandardDeviation(this IEnumerable<double> values, NormalisationMode mode = NormalisationMode.Population)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var valueslist = values.ToReadOnlyList();
            return StandardDeviation(valueslist, valueslist.Average(), mode);
        }

        /// <summary>
        ///     Calculates the standard deviation of a sequence.
        /// </summary>
        /// <param name="values">The sequence.</param>
        /// <param name="mean">The mean of the sequence.</param>
        /// <param name="mode">The normalisation mode.</param>
        /// <returns>The standard deviation of the sequence.</returns>
        public static double StandardDeviation(this IEnumerable<double> values, double mean, NormalisationMode mode = NormalisationMode.Population)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var valueslist = values.ToReadOnlyList();
            return Math.Sqrt(valueslist.Variance(mean, mode));
        }

        /// <summary>
        ///     Calculates the variance of a sequence.
        /// </summary>
        /// <param name="values">The sequence.</param>
        /// <param name="mode">The normalisation mode.</param>
        /// <returns>The variance of the sequence.</returns>
        public static double Variance(this IEnumerable<double> values, NormalisationMode mode = NormalisationMode.Population)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var valueslist = values.ToReadOnlyList();
            return Variance(valueslist, valueslist.Average(), mode);
        }

        /// <summary>
        ///     Calculates the variance of a sequence.
        /// </summary>
        /// <param name="values">The sequence.</param>
        /// <param name="mean">The mean of the sequence.</param>
        /// <param name="mode">The normalisation mode.</param>
        /// <returns>The variance of the sequence.</returns>
        public static double Variance(this IEnumerable<double> values, double mean, NormalisationMode mode = NormalisationMode.Population)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var valueslist = values.ToReadOnlyList();

            if (valueslist.Count == 0)
                return 0;

            var variance = valueslist.Aggregate(0.0, (d, d1) => d + Math.Pow(d1 - mean, 2));

            if (mode == NormalisationMode.Population)
                return variance / valueslist.Count;
            if (mode == NormalisationMode.Sample)
                return variance / Math.Max(valueslist.Count - 1, 1);

            throw new ArgumentException();
        }
    }
}