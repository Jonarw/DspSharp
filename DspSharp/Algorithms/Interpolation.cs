// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interpolation.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DspSharp.Algorithms
{
    public static class Interpolation
    {
        public enum ExtrapolationMode
        {
            Hold,
            Zero,
            NaN
        }

        /// <summary>
        ///     Interpolates a data series with x and y values to a new series with the specified x values.
        ///     Depending on the local point density of the original and new x values either spline interpolation, linear
        ///     interpolation or moving averaging is used to calculate the new y values.
        /// </summary>
        /// <param name="x">The original x values.</param>
        /// <param name="y">The original y values; must be the same length as <paramref name="x" />.</param>
        /// <param name="xtarget">The new x values.</param>
        /// <param name="logX">Determines whether the calculation should be performed with logarithmic scaling along the x axis.</param>
        /// <param name="useSpline">
        ///     If true, spline interpolation is used when the local point density is low. Otherwise, linar
        ///     interpolation is used.
        /// </param>
        /// <param name="extrapolationMode">Determines the behaviour outside of the input value range.</param>
        /// <returns>The interpolated y values.</returns>
        public static IEnumerable<double> AdaptiveInterpolation(
            IReadOnlyList<double> x,
            IReadOnlyList<double> y,
            IReadOnlyList<double> xtarget,
            bool logX = true,
            bool useSpline = true,
            ExtrapolationMode extrapolationMode = ExtrapolationMode.Hold)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (y == null)
                throw new ArgumentNullException(nameof(y));

            if (xtarget == null)
                throw new ArgumentNullException(nameof(xtarget));

            if (x.Count != y.Count)
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + " should be the same length.");

            if (x.Count == 0)
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + "cannot be empty.");

            if (xtarget.Count == 0)
                yield break;

            IReadOnlyList<double> actualX;
            IReadOnlyList<double> actualTargetX;
            IReadOnlyList<double> spline = null;

            if (logX)
            {
                actualX = x.Log(10).Select(d => Math.Max(d, -1e9)).ToReadOnlyList();
                actualTargetX = xtarget.Log(10).Select(d => Math.Max(d, -1e9)).ToReadOnlyList();
            }
            else
            {
                actualX = x;
                actualTargetX = xtarget;
            }

            var xCurrent = 0;
            while (actualX[xCurrent + 1] < actualTargetX[0])
            {
                xCurrent += 1;
            }

            var xMax = actualX.Count - 1;
            while (actualX[xMax - 1] > actualTargetX.Last())
            {
                xMax -= 1;
            }

            for (var c = 0; c < actualTargetX.Count; c++)
            {
                if (actualTargetX[c] < actualX.First())
                {
                    if (extrapolationMode == ExtrapolationMode.Zero)
                        yield return 0;
                    else if (extrapolationMode == ExtrapolationMode.NaN)
                        yield return double.NaN;
                    else if (extrapolationMode == ExtrapolationMode.Hold)
                        yield return y[0];

                    continue;
                }

                if (actualTargetX[c] > actualX.Last())
                {
                    if (extrapolationMode == ExtrapolationMode.Zero)
                        yield return 0;
                    else if (extrapolationMode == ExtrapolationMode.NaN)
                        yield return double.NaN;
                    else if (extrapolationMode == ExtrapolationMode.Hold)
                        yield return y[y.Count - 1];

                    continue;
                }

                double xlim;
                if (c == actualTargetX.Count - 1)
                    xlim = actualTargetX[c];
                else
                    xlim = (actualTargetX[c + 1] + actualTargetX[c]) / 2;

                var pointCounter = 0;
                while (xCurrent < xMax && actualX[xCurrent] < xlim)
                {
                    pointCounter += 1;
                    xCurrent += 1;
                }

                if (useSpline && pointCounter < 2) // spline
                {
                    if (spline == null)
                        spline = CubicSpline.CubicSpline.Compute(actualX.ToArray(), y.ToArray(), actualTargetX.ToArray());

                    yield return spline[c];
                }
                else if (pointCounter < 3) // linear interpolation
                {
                    var tmp = (actualTargetX[c] - actualX[xCurrent - 1]) * y[xCurrent];
                    tmp += (actualX[xCurrent] - actualTargetX[c]) * y[xCurrent - 1];
                    tmp /= actualX[xCurrent] - actualX[xCurrent - 1];
                    yield return tmp;
                }
                else // average
                {
                    double tmp = 0;
                    for (var c2 = 1; c2 <= pointCounter; c2++)
                    {
                        tmp += y[xCurrent - c2];
                    }

                    tmp /= pointCounter;
                    yield return tmp;
                }
            }
        }

        /// <summary>
        ///     Resamples a frequency domain signal to a specified series of frequencies. Depending on the local point density of
        ///     the original
        ///     and new frequency values, either spline interpolation, linear interpolation or moving averaging is used to
        ///     calculate the new spectral amplitudes.
        /// </summary>
        /// <param name="x">The original frequencies.</param>
        /// <param name="y">The original spectral amplitudes. Must be the same length as <paramref name="x" />.</param>
        /// <param name="targetX">The target frequencies.</param>
        /// <param name="logX">Determines whether the calculation shall be performed in a logarithmic x space.</param>
        /// <returns>The resampled spectral amplitudes.</returns>
        public static IEnumerable<Complex> AdaptiveInterpolation(
            IReadOnlyList<double> x,
            IReadOnlyList<Complex> y,
            IReadOnlyList<double> targetX,
            bool logX = true)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (y == null)
                throw new ArgumentNullException(nameof(y));

            if (targetX == null)
                throw new ArgumentNullException(nameof(targetX));

            if (x.Count != y.Count)
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + " should be the same length.");

            if (x.Count == 0)
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + "cannot be empty.");

            var magnitude = y.Magitude().ToReadOnlyList();
            var phase = y.Phase().ToReadOnlyList();
            phase = FrequencyDomain.UnwrapPhase(phase).ToReadOnlyList();

            var smagnitude = AdaptiveInterpolation(x, magnitude, targetX, logX);
            var sphase = AdaptiveInterpolation(x, phase, targetX, logX);

            return FrequencyDomain.PolarToComplex(smagnitude, sphase);
        }

        /// <summary>
        ///     Performs an cubic spline interpolation of a complex-valued series.
        /// </summary>
        /// <param name="x">The x-values of the original series.</param>
        /// <param name="y">The complex y-values of the original series. Must be the same length as <paramref name="x" />.</param>
        /// <param name="targetX">The desired x-values for the new series.</param>
        /// <param name="logX">
        ///     If <c>true</c> (default), the target x-values are assumed to be an a logarithmic scale and the
        ///     interpolation is done on a logarithmic scale as well.
        /// </param>
        /// <returns>A new array of the same length as <paramref name="targetX" /> containing the result.</returns>
        public static IEnumerable<Complex> InterpolateComplex(
            IReadOnlyList<double> x,
            IReadOnlyList<Complex> y,
            IReadOnlyList<double> targetX,
            bool logX = true)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (y == null)
                throw new ArgumentNullException(nameof(y));

            if (targetX == null)
                throw new ArgumentNullException(nameof(targetX));

            if (x.Count != y.Count)
                throw new ArgumentException();

            IReadOnlyList<double> actualX;
            IReadOnlyList<double> actualTargetX;

            if (logX)
            {
                actualX = x.Log().ToReadOnlyList();
                actualTargetX = targetX.Log().ToReadOnlyList();
            }
            else
            {
                actualX = x.ToReadOnlyList();
                actualTargetX = targetX.ToReadOnlyList();
            }

            var magnitude = y.Magitude();
            var phase = y.Phase();
            phase = FrequencyDomain.UnwrapPhase(phase);

            var mspline = CubicSpline.CubicSpline.Compute(
                actualX.ToArray(),
                magnitude.ToArray(),
                actualTargetX.ToArray());
            var pspline = CubicSpline.CubicSpline.Compute(actualX.ToArray(), phase.ToArray(), actualTargetX.ToArray());

            return FrequencyDomain.PolarToComplex(mspline, pspline);
        }

        public static double LinearInterpolation(double xactual, double x1, double x2, double y1, double y2, bool logX = false)
        {
            if (logX)
            {
                xactual = Math.Log(xactual);
                x1 = Math.Log(x1);
                x2 = Math.Log(x2);
            }

            return (xactual - x1) / (x2 - x1) * y2 + (x2 - xactual) / (x2 - x1) * y1;
        }

        public static Complex LinearInterpolation(double xactual, double x1, double x2, Complex y1, Complex y2, bool logX = false)
        {
            if (logX)
            {
                xactual = Math.Log(xactual);
                x1 = Math.Log(x1);
                x2 = Math.Log(x2);
            }

            return (xactual - x1) / (x2 - x1) * y2 + (x2 - xactual) / (x2 - x1) * y1;
        }

        /// <summary>
        ///     Smooths the y-values of a set of xy-related data with a moving average filter.
        /// </summary>
        /// <param name="x">The x-values.</param>
        /// <param name="y">The y-values. Must be the same length as <paramref name="x" />.</param>
        /// <param name="resolution">The smoothing resultion in points per octave.</param>
        /// <param name="logX">If <c>true</c> (default), the x-values are assumed to be on a logarithmic scale.</param>
        /// <returns>
        ///     A sequence of the same length as <paramref name="x" /> and <paramref name="y" /> containing the
        ///     result.
        /// </returns>
        public static IEnumerable<double> Smooth(
            IReadOnlyList<double> x,
            IReadOnlyList<double> y,
            int resolution,
            bool logX = true)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (y == null)
                throw new ArgumentNullException(nameof(y));

            if (x.Count != y.Count)
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + " should be the same length.");

            if (x.Count == 0)
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + " cannot be empty.");

            if (resolution < 0)
                throw new ArgumentOutOfRangeException(nameof(resolution));

            if (resolution == 0)
            {
                foreach (var d in y)
                {
                    yield return d;
                }

                yield break;
            }

            double SmoothWindow(double logF, double logF0, double bw)
            {
                var argument = (logF - logF0) / bw * Math.PI;
                if (Math.Abs(argument) >= Math.PI)
                    return 0;

                return 0.5 * (1.0 + Math.Cos(argument));
            }

            double bandwidth;
            IReadOnlyList<double> actualX;

            if (logX)
            {
                actualX = x.Log(10).ToReadOnlyList();
                bandwidth = Math.Log(Math.Pow(2.0, 1.0 / resolution));
            }
            else
            {
                actualX = x;
                bandwidth = Math.Pow(2.0, 1.0 / resolution);
            }

            for (var fc = 0; fc < y.Count; fc++)
            {
                double factorSum = 0;
                double sum = 0;
                var fc2 = fc;
                double factor;
                while (fc2 >= 0)
                {
                    if (!(actualX[fc2] > actualX[fc] - bandwidth))
                        break;

                    factor = SmoothWindow(actualX[fc2], actualX[fc], bandwidth);
                    factorSum += factor;
                    sum += factor * y[fc2];
                    fc2 -= 1;
                }

                fc2 = fc + 1;
                while (fc2 < actualX.Count)
                {
                    if (!(actualX[fc2] < actualX[fc] + bandwidth))
                        break;

                    factor = SmoothWindow(actualX[fc2], actualX[fc], bandwidth);
                    factorSum += factor;
                    sum += factor * y[fc2];
                    fc2 += 1;
                }

                yield return sum / factorSum;
            }
        }
    }
}