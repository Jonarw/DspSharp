// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interpolation.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Exceptions;
using DspSharp.Extensions;
using DspSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DspSharp.Algorithms
{
    public abstract class Interpolator
    {
        public ExtrapolationMode ExtrapolationMode { get; set; }

        public bool LogarithmicX { get; set; }

        //TODO: Unit test, implement binary search
        public static double GetValueAt(IReadOnlyList<double> x, IReadOnlyList<double> y, double xTarget, ExtrapolationMode mode = ExtrapolationMode.Hold)
        {
            if (x.Count != y.Count)
                throw new ArgumentException($"{nameof(x)} and {nameof(y)} must be the same length.");

            int i;
            for (i = 0; i < x.Count - 1 && xTarget > x[i]; i++)
            {
            }

            if (x[i] == xTarget)
                return y[i];

            if (i == 0 || i == x.Count - 1)
            {
                return mode switch
                {
                    ExtrapolationMode.Hold => y[i],
                    ExtrapolationMode.Zero => 0,
                    ExtrapolationMode.NaN => double.NaN,
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
                };
            }

            return LinearInterpolation(xTarget, x[i - 1], x[i], y[i - 1], y[i]);
        }

        public static double LinearInterpolation(double xactual, double x1, double x2, double y1, double y2)
        {
            return ((xactual - x1) / (x2 - x1) * y2) + ((x2 - xactual) / (x2 - x1) * y1);
        }

        public static Complex LinearInterpolation(double xactual, double x1, double x2, Complex y1, Complex y2)
        {
            return ((xactual - x1) / (x2 - x1) * y2) + ((x2 - xactual) / (x2 - x1) * y1);
        }

        /// <summary>
        /// Interpolates the given combination of X and complex Y values to a list of new X values.
        /// </summary>
        /// <param name="x">The list of X values. Must not be empty and monotonically increasing.</param>
        /// <param name="y">The list of complex Y values. Must have the same length as <paramref name="x"/>.</param>
        /// <param name="targetX">The list of target X values. Must be monotonically increasing.</param>
        /// <returns>The collection of interpolated values. This contains the interpolated values for each element of <paramref name="targetX"/>.</returns>
        /// <remarks>
        /// Magnitude and Phase of the complex <paramref name="y"/> are interpolated separately.
        /// </remarks>
        public ILazyReadOnlyCollection<Complex> Interpolate(IReadOnlyList<double> x, IReadOnlyList<Complex> y, IReadOnlyList<double> targetX)
        {
            var magnitude = y.SelectIndexed(c => c.Magnitude);
            var phase = y.SelectIndexed(c => c.Phase);

            var interpolatedMagnitude = this.Interpolate(x, magnitude, targetX);
            var interpolatedPhase = this.Interpolate(x, phase, targetX);

            return ComplexVectors.FromMagnitudeAndPhase(interpolatedMagnitude, interpolatedPhase);
        }

        /// <summary>
        /// Interpolates the given combination of X and Y values to a list of new X values.
        /// </summary>
        /// <param name="x">The list of X values. Must not be empty and monotonically increasing.</param>
        /// <param name="y">The list of Y values. Must have the same length as <paramref name="x"/>.</param>
        /// <param name="targetX">The list of target X values. Must be monotonically increasing.</param>
        /// <returns>The collection of interpolated values. This contains the interpolated values for each element of <paramref name="targetX"/>.</returns>
        public ILazyReadOnlyCollection<double> Interpolate(IReadOnlyList<double> x, IReadOnlyList<double> y, IReadOnlyList<double> targetX)
        {
            if (x.Count != y.Count)
                throw new LengthMismatchException(nameof(x), nameof(y));

            if (x.Count == 0)
                throw new ArgumentException($"{nameof(x)} and {nameof(y)} cannot be empty.");

            return InterpolateIterator().WithCount(targetX.Count);

            IEnumerable<double> InterpolateIterator()
            {
                if (targetX.Count == 0)
                    yield break;

                if (this.LogarithmicX)
                {
                    x = x.SelectIndexed(Math.Log);
                    targetX = targetX.SelectIndexed(Math.Log);
                }

                var minX = x[0];

                var i = 0;
                var extrapolationStartValue = this.ExtrapolationMode switch
                {
                    ExtrapolationMode.Hold => y[0],
                    ExtrapolationMode.Zero => 0,
                    _ => double.NaN,
                };

                while (targetX[i] < minX)
                {
                    yield return extrapolationStartValue;
                    i++;
                }

                var maxX = x[^1];
                var i2 = targetX.Count - 1;
                while (targetX[i2] > maxX)
                {
                    i2--;
                }

                i2++;

                if (i2 > i)
                {
                    var range = targetX.Range(i, i2 - i);
                    foreach (var item in this.InterpolateOverride(x, y, range))
                    {
                        yield return item;
                    }
                }

                var extrapolationEndValue = this.ExtrapolationMode switch
                {
                    ExtrapolationMode.Hold => y[^1],
                    ExtrapolationMode.Zero => 0,
                    _ => double.NaN,
                };

                for (; i2 < targetX.Count; i2++)
                {
                    yield return extrapolationEndValue;
                }
            }
        }

        /// <summary>
        /// Implements the implementation algorithm of this <see cref="Interpolator"/> instance.
        /// </summary>
        /// <param name="x">The list of X values. This has at least two values.</param>
        /// <param name="y">The list of Y values. This has the same number of values as <paramref name="x"/>.</param>
        /// <param name="targetX">The list of target X values. This has at least one value. All values are inside the range of X values defined by <paramref name="x"/>.</param>
        /// <returns>The interpolated sequence.</returns>
        protected abstract IEnumerable<double> InterpolateOverride(IReadOnlyList<double> x, IReadOnlyList<double> y, IReadOnlyList<double> targetX);
    }
}