// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Statistics.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UTilities;

namespace DspSharp.Algorithms
{
    public static class Statistics
    {
        /// <summary>
        /// Enumerates the different normalisation modes for computing Variance and Standard Deviation.
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
        /// Calculates the arithmetic mean of the specified sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static double ArithmeticMean(this IEnumerable<double> input)
        {
            return input.Average();
        }

        /// <summary>
        /// Fits a line to a collection of (x,y) points.
        /// </summary>
        /// <param name="xVals">The x-axis values.</param>
        /// <param name="yVals">The y-axis values.</param>
        /// <param name="rSquared">The r^2 value of the line.</param>
        /// <param name="yIntercept">The y-intercept value of the line (i.e. y = ax + b, yIntercept is b).</param>
        /// <param name="slope">The slop of the line (i.e. y = ax + b, slope is a).</param>
        public static void LinearRegression(
            IReadOnlyList<double> xVals,
            IReadOnlyList<double> yVals,
            out double rSquared,
            out double yIntercept,
            out double slope)
        {
            if (xVals.Count != yVals.Count)
                throw new Exception("Input values should be with the same length.");

            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double sumCodeviates = 0;

            for (var i = 0; i < xVals.Count; i++)
            {
                var x = xVals[i];
                var y = yVals[i];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }

            var count = xVals.Count;
            var ssX = sumOfXSq - ((sumOfX * sumOfX) / count);

            var rNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            var rDenom = (count * sumOfXSq - (sumOfX * sumOfX)) * (count * sumOfYSq - (sumOfY * sumOfY));
            var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            var meanX = sumOfX / count;
            var meanY = sumOfY / count;
            var dblR = rNumerator / Math.Sqrt(rDenom);

            rSquared = dblR * dblR;
            yIntercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
        }

        /// <summary>
        /// Calculates the geometric mean of the specified sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static double GeometricMean(this IReadOnlyCollection<double> input)
        {
            return Math.Pow(10, input.Log(10).Sum() / input.Count);
        }

        /// <summary>
        /// Gets the inter-quartile range from a sequence.
        /// </summary>
        /// <param name="values">The sequence.</param>
        public static double InterQuartileRange(this IEnumerable<double> values)
        {
            var list = values.OrderBy(d => d).ToList();
            return list.InterQuartileRange();
        }

        /// <summary>
        /// Gets the inter-quartile range from a SORTED sequence.
        /// </summary>
        /// <param name="sortedValues">The SORTED sequence.</param>
        public static double InterQuartileRange(this IReadOnlyList<double> sortedValues)
        {
            var q1 = sortedValues.NthOrderStatistic(25);
            var q3 = sortedValues.NthOrderStatistic(75);
            return q3 - q1;
        }

        /// <summary>
        /// Gets the median from a SORTED sequence.
        /// </summary>
        /// <param name="sortedList">The SORTED sequence.</param>
        public static T Median<T>(this IReadOnlyList<T> sortedList) where T : IComparable<T>
        {
            return sortedList.NthOrderStatistic(50);
        }

        /// <summary>
        /// Gets the median from a sequence.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        public static double Median(this IEnumerable<double> sequence)
        {
            var list = sequence.OrderBy(d => d).ToList();
            return list.Median();
        }

        /// <summary>
        /// Gets the nth-order statistic from a SORTED sequence.
        /// </summary>
        /// <param name="sortedList">The SORTED sequence.</param>
        /// <param name="percentile">The percentile.</param>
        public static T NthOrderStatistic<T>(this IReadOnlyList<T> sortedList, double percentile) where T : IComparable<T>
        {
            if (percentile == 100)
                return sortedList[sortedList.Count - 1];

            return sortedList[(int)(percentile / 100 * sortedList.Count)];
        }

        //TODO: unit test
        public static double Rms(this IReadOnlyCollection<double> values)
        {
            return Math.Sqrt(values.Aggregate(0d, (d, d1) => d + Math.Pow(d1, 2)) / values.Count);
        }

        /// <summary>
        /// Calculates the standard deviation of a sequence.
        /// </summary>
        /// <param name="values">The sequence.</param>
        /// <param name="mode">The normalisation mode.</param>
        public static double StandardDeviation(this IReadOnlyCollection<double> values, NormalisationMode mode = NormalisationMode.Population)
        {
            return StandardDeviation(values, values.Average(), mode);
        }

        /// <summary>
        /// Calculates the standard deviation of a sequence.
        /// </summary>
        /// <param name="values">The sequence.</param>
        /// <param name="mean">The mean of the sequence.</param>
        /// <param name="mode">The normalisation mode.</param>
        public static double StandardDeviation(this IReadOnlyCollection<double> values, double mean, NormalisationMode mode = NormalisationMode.Population)
        {
            return Math.Sqrt(values.Variance(mean, mode));
        }

        /// <summary>
        /// Calculates the variance of a sequence.
        /// </summary>
        /// <param name="values">The sequence.</param>
        /// <param name="mode">The normalisation mode.</param>
        public static double Variance(this IReadOnlyCollection<double> values, NormalisationMode mode = NormalisationMode.Population)
        {
            return Variance(values, values.Average(), mode);
        }

        /// <summary>
        /// Calculates the variance of a sequence.
        /// </summary>
        /// <param name="values">The sequence.</param>
        /// <param name="mean">The mean of the sequence.</param>
        /// <param name="mode">The normalisation mode.</param>
        public static double Variance(this IReadOnlyCollection<double> values, double mean, NormalisationMode mode = NormalisationMode.Population)
        {
            if (values.Count == 0)
                return 0;

            var variance = values.Aggregate(0.0, (d, d1) => d + Math.Pow(d1 - mean, 2));

            var norm = mode switch
            {
                NormalisationMode.Population => values.Count,
                NormalisationMode.Sample => Math.Max(values.Count - 1, 1),
                _ => throw EnumOutOfRangeException.Create(mode),
            };

            return variance / norm;
        }
    }
}