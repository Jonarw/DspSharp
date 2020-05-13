// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interpolation.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UTilities.Extensions;

namespace DspSharp.Algorithms
{
    public static class Interpolation
    {
        public enum ExtrapolationMode
        {
            Hold,
            Zero,
            NaN,
        }

        public static IEnumerable<double> LinearInterpolation(
            IReadOnlyList<double> x,
            IReadOnlyList<double> y,
            IReadOnlyList<double> xtarget,
            bool logX = true,
            ExtrapolationMode extrapolationMode = ExtrapolationMode.Hold)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (y == null)
                throw new ArgumentNullException(nameof(y));

            if (xtarget == null)
                throw new ArgumentNullException(nameof(xtarget));

            if (x.Count != y.Count)
                throw new ArgumentException($"{nameof(x)} and {nameof(y)} should be the same length.");

            if (x.Count == 0)
                throw new ArgumentException($"{nameof(x)} and {nameof(y)}cannot be empty.");

            if (xtarget.Count == 0)
                yield break;

            IReadOnlyList<double> actualX;
            IReadOnlyList<double> actualTargetX;

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

            int xc = 1;

            for (var c = 0; c < actualTargetX.Count; c++)
            {
                if (Extrapolate(actualX[0], actualX[x.Count - 1], actualTargetX[c], y[0], y[x.Count - 1], extrapolationMode, out var ret))
                {
                    yield return ret;
                    continue;
                }

                while (actualX[xc] < actualTargetX[c])
                    xc++;

                yield return LinearInterpolation(actualTargetX[c], actualX[xc - 1], actualX[xc], y[xc - 1], y[xc]);
            }

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
                throw new ArgumentException($"{nameof(x)} and {nameof(y)} should be the same length.");

            if (x.Count == 0)
                throw new ArgumentException($"{nameof(x)} and {nameof(y)}cannot be empty.");

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
                if (Extrapolate(actualX[0], actualX[x.Count - 1], actualTargetX[c], y[0], y[x.Count - 1], extrapolationMode, out var ret))
                {
                    yield return ret;
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
                    var tmp = LinearInterpolation(actualTargetX[c], actualX[xCurrent - 1], actualX[xCurrent], y[xCurrent - 1], y[xCurrent]);

                    //tmp = (actualTargetX[c] - actualX[xCurrent - 1]) * y[xCurrent];
                    //tmp += (actualX[xCurrent] - actualTargetX[c]) * y[xCurrent - 1];
                    //tmp /= actualX[xCurrent] - actualX[xCurrent - 1];
                    yield return tmp;
                }
                else // average
                {
                    double tmp = 0;
                    for (var c2 = 1; c2 <= pointCounter; c2++)
                        tmp += y[xCurrent - c2];

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
            bool logX = true,
            bool useSpline = true,
            ExtrapolationMode extrapolationMode = ExtrapolationMode.Hold)
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

            var smagnitude = AdaptiveInterpolation(x, magnitude, targetX, logX, useSpline, extrapolationMode);
            var sphase = AdaptiveInterpolation(x, phase, targetX, logX, useSpline, extrapolationMode);

            return FrequencyDomain.PolarToComplex(smagnitude, sphase);
        }

        //TODO: Unit test
        public static double GetValueAt(IReadOnlyList<double> x, IReadOnlyList<double> y, double xTarget, bool logX = false, ExtrapolationMode mode = ExtrapolationMode.Hold)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));
            if (x.Count != y.Count)
                throw new ArgumentException();

            int i;
            for (i = 0; i < x.Count - 1 && xTarget > x[i]; i++)
            {
            }

            if (x[i] == xTarget)
                return y[i];

            if (i == 0 || i == x.Count - 1)
            {
                switch (mode)
                {
                    case ExtrapolationMode.Hold:
                        return y[i];
                    case ExtrapolationMode.Zero:
                        return 0;
                    case ExtrapolationMode.NaN:
                        return double.NaN;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }

            return LinearInterpolation(xTarget, x[i - 1], x[i], y[i - 1], y[i], logX);
        }

        public static Complex GetValueAt(IReadOnlyList<double> x, IReadOnlyList<Complex> y, double xTarget, bool logX = false)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));
            if (x.Count != y.Count)
                throw new ArgumentException();

            int i;
            for (i = 0; i < x.Count && xTarget < x[i]; i++)
            {
            }

            if (i == 0 || i == x.Count - 1)
                return x[i];

            return LinearInterpolation(xTarget, x[i - 1], x[i], y[i - 1], y[i], logX);
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
        public static IEnumerable<double> HannSmooth(
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
                    yield return d;

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
        public static IEnumerable<double> MovingAverage(
            IReadOnlyList<double> x,
            IReadOnlyList<double> y,
            double pointsPerOctave)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (y == null)
                throw new ArgumentNullException(nameof(y));

            if (x.Count != y.Count)
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + " should be the same length.");

            if (x.Count == 0)
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + " cannot be empty.");

            if (pointsPerOctave == 0)
            {
                foreach (var d in y)
                    yield return d;

                yield break;
            }

            var xlog = x.Log(10).ToReadOnlyList();
            var halfbandwidth = .5 * Math.Log10(Math.Pow(2.0, 1 / pointsPerOctave));

            for (var fc = 0; fc < y.Count; fc++)
            {
                var pointCounter = 0;
                double sum = 0;
                var fc2 = fc;

                var lowerthreshold = xlog[fc] - halfbandwidth;
                var upperthreshold = xlog[fc] + halfbandwidth;

                while (fc2 >= 0 && xlog[fc2] > lowerthreshold)
                {
                    pointCounter++;
                    sum += y[fc2];
                    fc2--;
                }

                fc2 = fc + 1;
                while (fc2 < xlog.Count && xlog[fc2] < upperthreshold)
                {
                    pointCounter++;
                    sum += y[fc2];
                    fc2++;
                }

                yield return sum / pointCounter;
            }
        }

        private static bool Extrapolate(double lowerThreshold, double upperThreshold, double actual, double minvalue, double maxvalue, ExtrapolationMode mode, out double value)
        {
            if (actual > lowerThreshold && actual < upperThreshold)
            {
                value = double.NaN;
                return false;
            }

            if (actual == lowerThreshold)
                value = minvalue;
            else if (actual == upperThreshold)
                value = maxvalue;
            else
            {
                switch (mode)
                {
                case ExtrapolationMode.Zero:
                    value = 0;
                    break;
                case ExtrapolationMode.NaN:
                    value = double.NaN;
                    break;
                case ExtrapolationMode.Hold:
                    value = actual < lowerThreshold ? minvalue : maxvalue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }

            return true;
        }

        private static int GetIndexOfNextY(IReadOnlyList<double> x, double value)
        {
            var currentX = x.Count / 2;
            var delta = (int)Math.Floor(x.Count / 4d);

            while (delta > 1)
            {
                if (x[currentX] < value)
                    currentX += delta;
                else
                    currentX -= delta;

                delta = (int)Math.Floor(delta / 2d);
            }

            while (currentX < x.Count && x[currentX] <= value)
                currentX++;

            while (currentX > 0 && x[currentX - 1] > value)
                currentX--;

            return currentX;
        }

        public static IEnumerable<double> Smooth(
            IReadOnlyList<double> x,
            IReadOnlyList<double> y,
            IReadOnlyList<double> xTarget,
            double bandwidth,
            ExtrapolationMode extrapolationMode = ExtrapolationMode.Hold,
            Func<double, double> smoothWindow = null)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (y == null)
                throw new ArgumentNullException(nameof(y));

            if (xTarget == null)
                throw new ArgumentNullException(nameof(xTarget));

            if (x.Count != y.Count)
                throw new ArgumentException($"{nameof(x)} and {nameof(y)} should be the same length.");

            if (x.Count == 0)
                throw new ArgumentException($"{nameof(x)} and {nameof(y)} cannot be empty.");

            if (bandwidth < 0)
                throw new ArgumentOutOfRangeException(nameof(bandwidth));

            if (xTarget.Count == 0)
                yield break;

            if (smoothWindow == null)
                smoothWindow = d => 1;

            var logX = x.Log(10).Select(d => Math.Max(d, -1e9)).ToReadOnlyList();
            var logTargetX = xTarget.Log(10).Select(d => Math.Max(d, -1e9)).ToReadOnlyList();
            var logDeltaX = Math.Log(2, 10) * bandwidth * 2;
            var logHalfDeltaX = .5 * logDeltaX;

            var xCurrent = 0;
            while (logX[xCurrent + 1] < logTargetX[0])
            {
                xCurrent += 1;
            }

            var xMax = logX.Count - 1;
            while (logX[xMax - 1] > logTargetX.Last())
            {
                xMax -= 1;
            }

            foreach (var logCurrentX in logTargetX)
            {
                if (Extrapolate(logX[0], logX[x.Count - 1], logCurrentX, y[0], y[x.Count - 1], extrapolationMode, out var ret ))
                {
                    yield return ret;
                    continue;
                }

                var logLowerThresholdX = logCurrentX - logHalfDeltaX;
                var logUpperThresholdX = logCurrentX + logHalfDeltaX;

                var lowerThresholdIndex = GetIndexOfNextY(logX, logLowerThresholdX);
                var upperThresholdIndex = GetIndexOfNextY(logX, logUpperThresholdX);

                if (upperThresholdIndex == lowerThresholdIndex)
                {
                    yield return LinearInterpolation(logCurrentX, logX[lowerThresholdIndex - 1], logX[lowerThresholdIndex], y[lowerThresholdIndex - 1], y[lowerThresholdIndex], false);
                    continue;
                }

                if (upperThresholdIndex == lowerThresholdIndex + 1)
                {
                    var currentNextIndex = GetIndexOfNextY(logX, logCurrentX);
                    yield return LinearInterpolation(logCurrentX, logX[currentNextIndex - 1], logX[currentNextIndex], y[currentNextIndex - 1], y[currentNextIndex], false);
                    continue;
                }

                var normalization = 0d;
                var sum = 0d;
                for (int i = lowerThresholdIndex; i < upperThresholdIndex; i++)
                {
                    var windowInput = Math.Abs((logX[i] - logCurrentX) / logHalfDeltaX);
                    var factor = smoothWindow(1 - windowInput);

                    normalization += factor;
                    sum += factor * y[i];
                }

                if (normalization == 0)
                {
                    throw new Exception();
                }

                yield return sum / normalization;
            }
        }
    }
}