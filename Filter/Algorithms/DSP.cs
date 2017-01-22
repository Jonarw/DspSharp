using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Filter.Extensions;

namespace Filter.Algorithms
{
    /// <summary>
    ///     Static container class for acoustic algorithms.
    /// </summary>
    public sealed class Dsp
    {
        /// <summary>
        ///     Enumeration of all supported slope-generating methods.
        /// </summary>
        public enum SlopeModes
        {
            /// <summary>
            ///     The slope consists of a straight line in logarithmic scale.
            /// </summary>
            Straight,

            /// <summary>
            ///     The slope consists of a smooth raised-cosine line scale.
            /// </summary>
            Smooth
        }

        /// <summary>
        ///     Enumerates the available alignments for sin sweeps.
        /// </summary>
        public enum SweepAlignments
        {
            /// <summary>
            ///     No special alignment.
            /// </summary>
            None,

            /// <summary>
            ///     End at a zero.
            /// </summary>
            Zero,

            /// <summary>
            ///     End at a zero from a positive half wave.
            /// </summary>
            PositiveZero,

            /// <summary>
            ///     End at a zero from a negative half wave.
            /// </summary>
            NegativeZero,

            /// <summary>
            ///     End at 1.
            /// </summary>
            PositiveOne,

            /// <summary>
            ///     End at -1.
            /// </summary>
            NegativeOne
        }

        private static int WhiteNoiseSeedNumber { get; set; }

        /// <summary>
        ///     Interpolates a data series with x and y values to a new series with the specified x values.
        ///     Depending on the local point density of the original and new x values either spline interpolation, linear
        ///     interpolation or moving averaging is used to calculate the new y values.
        /// </summary>
        /// <param name="x">The original x values.</param>
        /// <param name="y">The original y values; must be the same length as <paramref name="x" />.</param>
        /// <param name="xtarget">The new x values.</param>
        /// <param name="logX">Determines whether the calculation should be performed with logarithmic scaling along the x axis.</param>
        /// <returns>The interpolated y values.</returns>
        public static IEnumerable<double> AdaptiveInterpolation(
            IReadOnlyList<double> x,
            IReadOnlyList<double> y,
            IReadOnlyList<double> xtarget,
            bool logX = true)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            if (xtarget == null)
            {
                throw new ArgumentNullException(nameof(xtarget));
            }

            if (x.Count != y.Count)
            {
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + " should be the same length.");
            }

            if (x.Count == 0)
            {
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + "cannot be empty.");
            }

            if (xtarget.Count == 0)
            {
                yield break;
            }

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
                    yield return y[0];
                    continue;
                }

                if (actualTargetX[c] > actualX.Last())
                {
                    yield return y[y.Count - 1];
                    continue;
                }

                double xlim;
                if (c == actualTargetX.Count - 1)
                {
                    xlim = actualTargetX[c];
                }
                else
                {
                    xlim = (actualTargetX[c + 1] + actualTargetX[c]) / 2;
                }

                var pointCounter = 0;
                while ((xCurrent < xMax) && (actualX[xCurrent] < xlim))
                {
                    pointCounter += 1;
                    xCurrent += 1;
                }

                //if (pointCounter < 2) // spline
                //{
                //    if (spline == null)
                //    {
                //        spline = CubicSpline.Compute(actualX.ToArray(), y.ToArray(), actualTargetX.ToArray());
                //    }

                //    yield return spline[c];
                //}
                //else 
                if (pointCounter < 3) // linear interpolation
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
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            if (targetX == null)
            {
                throw new ArgumentNullException(nameof(targetX));
            }

            if (x.Count != y.Count)
            {
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + " should be the same length.");
            }

            if (x.Count == 0)
            {
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + "cannot be empty.");
            }

            var magnitude = y.Magitude().ToReadOnlyList();
            var phase = y.Phase().ToReadOnlyList();
            phase = UnwrapPhase(phase).ToReadOnlyList();

            var smagnitude = AdaptiveInterpolation(x, magnitude, targetX, logX);
            var sphase = AdaptiveInterpolation(x, phase, targetX, logX);

            return PolarToComplex(smagnitude, sphase);
        }

        /// <summary>
        ///     Computes a logarithmic sine sweep where the stop frequency is slightly altered so that the sweep stops exactly at a
        ///     zero-crossing.
        /// </summary>
        /// <param name="from">The start frequency.</param>
        /// <param name="to">The stop frequency.</param>
        /// <param name="length">The length.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="samplerate">The samplerate.</param>
        /// <returns></returns>
        public static IEnumerable<double> AlignedLogSweep(double from, double to, double length, SweepAlignments alignment, double samplerate = 44100)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (from <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(from));
            }

            if (to <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(to));
            }

            if (samplerate <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(samplerate));
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (to == from)
            {
                throw new ArgumentException(nameof(to) + " and " + nameof(from) + " cannot be the same.");
            }

            if (alignment == SweepAlignments.None)
            {
                return LogSweep(from, to, length, samplerate);
            }

            length = Convert.ToInt32(length * samplerate) / samplerate;

            var w1 = Math.Min(to, from) * 2 * Math.PI;
            var w2 = Math.Max(to, from) * 2 * Math.PI;
            var k = length * (w2 - w1) / (Math.PI * Math.Log(w2 / w1));

            if (alignment == SweepAlignments.Zero)
            {
                k = Convert.ToInt32(k);
            }
            else if (alignment == SweepAlignments.NegativeZero)
            {
                k = Convert.ToInt32(0.5 * k) * 2;
            }
            else if (alignment == SweepAlignments.PositiveZero)
            {
                k = Convert.ToInt32(0.5 * (k + 1)) * 2 - 1;
            }
            else if (alignment == SweepAlignments.NegativeOne)
            {
                k = Convert.ToInt32(0.5 * (k + 0.5)) * 2 - 0.5;
            }
            else if (alignment == SweepAlignments.PositiveOne)
            {
                k = Convert.ToInt32(0.5 * (k - 0.5)) * 2 + 0.5;
            }

            w2 = FindRoot(w2N => length * (w2N - w1) / (Math.PI * Math.Log(w2N / w1)) - k, w2, 1);

            var actualfrom = from < to ? from : w2 / (2 * Math.PI);
            var actualto = from < to ? w2 / (2 * Math.PI) : to;
            return LogSweep(actualfrom, actualto, length, samplerate);
        }

        /// <summary>
        ///     Converts a real-valued array to a zero-phase complex-valued array.
        /// </summary>
        /// <param name="amplitude">The amplitude array.</param>
        /// <returns>A new complex array of the same length as <paramref name="amplitude" /> containing the result.</returns>
        public static IEnumerable<Complex> AmplitudeToComplex(IEnumerable<double> amplitude)
        {
            if (amplitude == null)
            {
                throw new ArgumentNullException(nameof(amplitude));
            }

            return amplitude.Select(d => new Complex(d, 0.0));
        }

        /// <summary>
        ///     Applies a time delay to a complex frequency spectrum by representing the constant group delay in the complex phase
        ///     information.
        /// </summary>
        /// <param name="frequencies">The frequencies of the complex spectrum.</param>
        /// <param name="amplitudes">
        ///     The complex amplitudes of the spectrum.
        /// </param>
        /// <param name="delay">The delay to be applied to the spectrum. Can be negative.</param>
        /// <returns>
        ///     A new array containing the result. If <paramref name="frequencies" /> and <paramref name="amplitudes" /> are not
        ///     the same length, the longer one is truncated.
        /// </returns>
        public static IEnumerable<Complex> ApplyDelayToSpectrum(IEnumerable<double> frequencies, IEnumerable<Complex> amplitudes, double delay)
        {
            if (frequencies == null)
            {
                throw new ArgumentNullException(nameof(frequencies));
            }

            if (amplitudes == null)
            {
                throw new ArgumentNullException(nameof(amplitudes));
            }

            var factor = Complex.ImaginaryOne * 2 * Math.PI * delay;
            return frequencies.Zip(amplitudes, (f, a) => Complex.Exp(factor * f) * a);
        }

        /// <summary>
        ///     Experimental; approximates the spectrum of an infinite signal, tries to identify the nescessery analysis window
        ///     length.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <param name="energyRatio">The energy ratio.</param>
        /// <param name="initialLength">The initial length.</param>
        /// <param name="maximumLength">The maximum length.</param>
        /// <returns></returns>
        public static IReadOnlyList<Complex> ApproximateSpectrumOfInfiniteSignal(
            IEnumerable<double> signal,
            double energyRatio = 0.00001,
            int initialLength = 1024,
            int maximumLength = 524288)
        {
            var currentLength = initialLength / 2;

            // ReSharper disable PossibleMultipleEnumeration - unavoidable with infinite signal
            while (signal.Skip(currentLength).Take(currentLength).CalculateEnergy() / signal.Take(currentLength).CalculateEnergy() > energyRatio)
            {
                currentLength *= 2;
                if (currentLength > maximumLength)
                {
                    break;
                }
            }

            return Fft.RealFft(signal.Take(currentLength));
            // ReSharper restore PossibleMultipleEnumeration
        }

        /// <summary>
        ///     Calculates the group delay of a system for a given phase response.
        /// </summary>
        /// <param name="frequencies">
        ///     The frequencies the phase values correspond to.
        /// </param>
        /// <param name="phase">The phase values.</param>
        /// <returns>
        ///     An array containing the result (in seconds). If <paramref name="frequencies" /> and <paramref name="phase" /> are
        ///     not the same length, the longer one is truncated.
        /// </returns>
        public static IEnumerable<double> CalculateGroupDelay(IEnumerable<double> frequencies, IEnumerable<double> phase)
        {
            if (frequencies == null)
            {
                throw new ArgumentNullException(nameof(frequencies));
            }

            if (phase == null)
            {
                throw new ArgumentNullException(nameof(phase));
            }

            var phaselist = phase.ToReadOnlyList();
            var frequencylist = frequencies.ToReadOnlyList();

            var n = Math.Min(phaselist.Count, frequencylist.Count);

            if (n == 0)
            {
                yield break;
            }

            yield return (phaselist[0] - phaselist[1]) / (2 * Math.PI * (frequencylist[1] - frequencylist[0]));
            for (var c = 1; c < n - 1; c++)
            {
                yield return (phaselist[c - 1] - phaselist[c + 1]) / (2 * Math.PI * (frequencylist[c + 1] - frequencylist[c - 1]));
            }

            yield return (phaselist[phaselist.Count - 2] - phaselist[phaselist.Count - 1]) /
                         (2 * Math.PI * (frequencylist[frequencylist.Count - 1] - frequencylist[frequencylist.Count - 2]));
        }

        /// <summary>
        ///     Performs a circular shift on an array.
        /// </summary>
        /// <param name="input">The array to be circularly shifted.</param>
        /// <param name="offset">
        ///     The amount of samples the array should be shifted. Positive offsets are used for left-shifts while negative
        ///     offsets are used for right-shifts.
        /// </param>
        /// <returns>An array of the same length as <paramref name="input" /> containing the result.</returns>
        public static IEnumerable<T> CircularShift<T>(IReadOnlyList<T> input, int offset)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.Count == 0)
            {
                return Enumerable.Empty<T>();
            }

            offset = Mod(offset, input.Count);

            return input.Skip(offset).Concat(input.Take(offset));
        }

        /// <summary>
        ///     Converts a time to its equivalent in samples.
        /// </summary>
        /// <param name="delay">The time.</param>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="integer">If set to <c>true</c>, the provided time equals an integer number of samples.</param>
        /// <returns>The number of samples equivalent to the provided time.</returns>
        public static int ConvertTimeToSamples(double delay, double sampleRate, out bool integer)
        {
            if (sampleRate <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sampleRate));
            }

            var mod = Math.Abs(delay % (1 / sampleRate));

            if ((mod > 1e-13) && (mod < 1 / sampleRate - 1e-13))
            {
                integer = false;
            }
            else
            {
                integer = true;
            }

            return Convert.ToInt32(delay * sampleRate);
        }

        /// <summary>
        ///     Convolves the specified finite signals.
        /// </summary>
        /// <param name="signal1">The first signal.</param>
        /// <param name="signal2">The second signal.</param>
        /// <returns>The convolution of the two signals.</returns>
        public static IReadOnlyList<double> Convolve(IReadOnlyList<double> signal1, IReadOnlyList<double> signal2)
        {
            if (signal1 == null)
            {
                throw new ArgumentNullException(nameof(signal1));
            }

            if (signal2 == null)
            {
                throw new ArgumentNullException(nameof(signal2));
            }

            if ((signal1.Count == 0) || (signal2.Count == 0))
            {
                return new List<double>().AsReadOnly();
            }

            var l = signal1.Count + signal1.Count - 1;
            var n = Fft.NextPowerOfTwo(l);
            var spectrum1 = Fft.RealFft(signal1, n);
            var spectrum2 = Fft.RealFft(signal2, n);
            var spectrum = spectrum2.Multiply(spectrum1);
            return Fft.RealIfft(spectrum).Take(l).ToReadOnlyList();
        }

        /// <summary>
        ///     Convolves the specified signals.
        /// </summary>
        /// <param name="signal1">The first signal. Can be of infinite length.</param>
        /// <param name="signal2">The second signal.</param>
        /// <returns>The convolution of the two signals.</returns>
        public static IEnumerable<double> Convolve(IEnumerable<double> signal1, IReadOnlyList<double> signal2)
        {
            if (signal1 == null)
            {
                throw new ArgumentNullException(nameof(signal1));
            }

            if (signal2 == null)
            {
                throw new ArgumentNullException(nameof(signal2));
            }

            if (signal2.Count == 0)
            {
                yield break;
            }

            var e1 = signal1.GetEnumerator();

            var n = 2 * Fft.NextPowerOfTwo(signal2.Count);
            var blockSize = n - signal2.Count + 1;
            var sig2Fft = Fft.RealFft(signal2, n);
            IReadOnlyList<double> buffer = null;
            var sig1Buffer = new List<double>(blockSize);

            while (true)
            {
                var c = 0;
                while ((c < blockSize) && e1.MoveNext())
                {
                    sig1Buffer.Add(e1.Current);
                    c++;
                }

                IEnumerable<double> ret;
                IReadOnlyList<double> blockconv;
                if (c > 0)
                {
                    var sig1Fft = Fft.RealFft(sig1Buffer, n);
                    sig1Buffer.Clear();
                    var spec = sig1Fft.Multiply(sig2Fft);
                    blockconv = Fft.RealIfft(spec);
                    ret = blockconv;
                }
                else
                {
                    blockconv = Enumerable.Empty<double>().ToReadOnlyList();
                    ret = Enumerable.Empty<double>();
                }

                if (buffer != null)
                {
                    ret = ret.AddFull(buffer);
                }

                if (c < blockSize)
                {
                    foreach (var d in ret.Take(c + signal2.Count - 1))
                    {
                        yield return d;
                    }

                    yield break;
                }

                foreach (var d in ret.Take(blockSize))
                {
                    yield return d;
                }

                buffer = blockconv.GetRangeOptimized(blockSize, signal2.Count - 1).ToReadOnlyList();
            }
        }

        /// <summary>
        ///     Computes the cross-correlation of two finite signals.
        /// </summary>
        /// <param name="signal1">The first signal.</param>
        /// <param name="signal2">The second signal.</param>
        /// <returns>The result of the computation.</returns>
        public static IReadOnlyList<double> CrossCorrelate(IReadOnlyList<double> signal1, IReadOnlyList<double> signal2)
        {
            if (signal1 == null)
            {
                throw new ArgumentNullException(nameof(signal1));
            }

            if (signal2 == null)
            {
                throw new ArgumentNullException(nameof(signal2));
            }

            return Convolve(signal2.Reverse().ToReadOnlyList(), signal1);
        }

        /// <summary>
        ///     Computes the cross-correlation of two signals.
        /// </summary>
        /// <param name="signal1">The second signal (can be infinite).</param>
        /// <param name="signal2">The first signal.</param>
        /// <returns>The result of the computation.</returns>
        public static IEnumerable<double> CrossCorrelate(IEnumerable<double> signal1, IReadOnlyList<double> signal2)
        {
            if (signal2 == null)
            {
                throw new ArgumentNullException(nameof(signal2));
            }

            if (signal1 == null)
            {
                throw new ArgumentNullException(nameof(signal1));
            }

            return Convolve(signal1, signal2.Reverse().ToReadOnlyList());
        }

        /// <summary>
        ///     Converts a single value from dB to linear scale.
        /// </summary>
        /// <param name="dB">The value in dB.</param>
        /// <returns>The value in linear scale.</returns>
        public static double DbToLinear(double dB)
        {
            return Math.Pow(10, dB / 20);
        }

        /// <summary>
        ///     Converts an array from dB to linear scale.
        /// </summary>
        /// <param name="dB">The array in dB scale.</param>
        /// <returns>A new array of the same length as <paramref name="dB" /> containing the result.</returns>
        public static IEnumerable<double> DbToLinear(IEnumerable<double> dB)
        {
            if (dB == null)
            {
                throw new ArgumentNullException(nameof(dB));
            }

            return dB.Select(DbToLinear);
        }

        /// <summary>
        ///     Converts a singe value from degree to rad.
        /// </summary>
        /// <param name="deg">The value in degree.</param>
        /// <returns>The value in rad.</returns>
        public static double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }

        /// <summary>
        ///     Converts an array from degree to rad.
        /// </summary>
        /// <param name="deg">The array in degree.</param>
        /// <returns>A new array of the same length as <paramref name="deg" /> containing the result.</returns>
        public static IEnumerable<double> DegToRad(IEnumerable<double> deg)
        {
            if (deg == null)
            {
                throw new ArgumentNullException(nameof(deg));
            }

            return deg.Select(DegToRad);
        }

        /// <summary>
        ///     Uses a simple iterative algorithm to find the root of a (locally) monotonous function.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="startValue">The start value.</param>
        /// <param name="initialStepSize">The initial step size.</param>
        /// <param name="threshold">The threshold where the iteration stops.</param>
        /// <returns>The x coordinate of the root.</returns>
        public static double FindRoot(Func<double, double> function, double startValue, double initialStepSize, double threshold = 1e-16)
        {
            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            if (initialStepSize == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialStepSize));
            }

            if (threshold < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(threshold));
            }

            var e1 = function(startValue);
            var x = startValue + initialStepSize;
            var stepsize = initialStepSize;
            double e;

            int i = 0;

            while ((e = Math.Abs(function(x))) > threshold)
            {
                if (Math.Abs(e - e1) < threshold)
                {
                    break;
                }

                stepsize = stepsize * (e / (e1 - e));

                e1 = e;
                x += stepsize;

                if (i++ > 100)
                {
                    throw new Exception("Not converging.");
                }
            }

            return x;
        }

        /// <summary>
        ///     Generates a slope.
        /// </summary>
        /// <param name="x">The x values where the slope is evaluated.</param>
        /// <param name="startX">The start x value.</param>
        /// <param name="stopX">The stop x value.</param>
        /// <param name="startValue">The start slope value.</param>
        /// <param name="stopValue">The stop slope value.</param>
        /// <param name="mode">The slope mode.</param>
        /// <param name="logarithmicX">If set to <c>true</c> the generation is done on a logarithmic x scale.</param>
        /// <returns>The result.</returns>
        public static IEnumerable<double> GenerateSlope(
            IEnumerable<double> x,
            double startX,
            double stopX,
            double startValue,
            double stopValue,
            SlopeModes mode = SlopeModes.Smooth,
            bool logarithmicX = true)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            IReadOnlyList<double> actualX;
            double actualStartX, actualStopX;

            if (startX > stopX)
            {
                var tmp = stopX;
                stopX = startX;
                startX = tmp;
                tmp = stopValue;
                stopValue = startValue;
                startValue = tmp;
            }

            if (logarithmicX)
            {
                actualX = x.Log(10).ToReadOnlyList();
                actualStartX = Math.Log10(startX);
                actualStopX = Math.Log10(stopX);
            }
            else
            {
                actualX = x.ToReadOnlyList();
                actualStartX = startX;
                actualStopX = stopX;
            }

            if (actualX.Count == 0)
            {
                yield break;
            }

            Func<double, double> smoothSlope = input => -0.5 * (Math.Cos(Math.PI * input) - 1);

            var deltaF = actualStopX - actualStartX;
            var deltaV = stopValue - startValue;

            foreach (var f in actualX)
            {
                double actualGain;
                if (f <= actualStartX)
                {
                    actualGain = startValue;
                }
                else if (f >= actualStopX)
                {
                    actualGain = stopValue;
                }
                else
                {
                    var tmpgain = (f - actualStartX) / deltaF;
                    if (mode == SlopeModes.Smooth)
                    {
                        tmpgain = smoothSlope(tmpgain);
                    }

                    tmpgain = deltaV * tmpgain + startValue;
                    actualGain = tmpgain;
                }

                yield return actualGain;
            }
        }

        /// <summary>
        ///     Generates the positive half of the Sinc function.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        /// <param name="samplerate">The samplerate.</param>
        /// <returns></returns>
        public static IEnumerable<double> HalfSinc(double frequency, double samplerate)
        {
            if (frequency <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(frequency));
            }

            if (samplerate <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(samplerate));
            }

            yield return 1;

            double factor = 2 * Math.PI * frequency / samplerate;
            int c = 1;
            while (true)
            {
                var omega = c * factor;
                yield return Math.Sin(omega) / omega;
                c++;
            }
            // ReSharper disable once FunctionNeverReturns
            // Output is meant to be infinte
        }

        /// <summary>
        ///     Applies an IIR filter to the provided input signal.
        /// </summary>
        /// <param name="input">The input signal.</param>
        /// <param name="a">The denominator coefficients of the filter.</param>
        /// <param name="b">The numerator coefficients of the filter.</param>
        /// <param name="inputbuffer">The inputbuffer. If not provided, an empty buffer is created.</param>
        /// <param name="outputbuffer">The outputbuffer. If not provided, an empty buffer is created.</param>
        /// <param name="clip">
        ///     If set to true, the output signal is clipped to the length of the input signal. Otherwise, the
        ///     output signal will be infinitely long.
        /// </param>
        /// <returns>The filter output.</returns>
        public static IEnumerable<double> IirFilter(
            IEnumerable<double> input,
            IReadOnlyList<double> a,
            IReadOnlyList<double> b,
            CircularBuffer<double> inputbuffer = null,
            CircularBuffer<double> outputbuffer = null,
            bool clip = false)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            if (a.Count < b.Count)
            {
                a = a.ZeroPad(b.Count - a.Count).ToReadOnlyList();
            }
            else if (b.Count < a.Count)
            {
                b = b.ZeroPad(a.Count - b.Count).ToReadOnlyList();
            }

            if ((a.Count == 0) || (a[0] == 0))
            {
                throw new Exception("a0 cannot be 0.");
            }

            var n = a.Count - 1;

            if (n < 0)
            {
                yield break;
            }

            if ((inputbuffer == null) || (outputbuffer == null))
            {
                inputbuffer = new CircularBuffer<double>(n);
                outputbuffer = new CircularBuffer<double>(n);
            }
            else if ((inputbuffer.Length != n) || (outputbuffer.Length != n))
            {
                throw new ArgumentException();
            }

            var an = a.Multiply(1 / a[0]).ToReadOnlyList();
            var bn = b.Multiply(1 / a[0]).ToReadOnlyList();
            var e = input.GetEnumerator();

            while (e.MoveNext())
            {
                double currentY = e.Current * bn[0];

                for (int i = 1; i <= n; i++)
                {
                    currentY += inputbuffer.Peek(i) * bn[i];
                    currentY -= outputbuffer.Peek(i) * an[i];
                }

                inputbuffer.Store(e.Current);
                outputbuffer.Store(currentY);

                yield return currentY;
            }

            if (clip)
            {
                yield break;
            }

            for (int i2 = 0; i2 < inputbuffer.Length; i2++)
            {
                double currentY = 0.0;

                for (int i = 1; i <= n; i++)
                {
                    currentY += inputbuffer.Peek(i) * bn[i];
                    currentY -= outputbuffer.Peek(i) * an[i];
                }

                inputbuffer.Store(0.0);
                outputbuffer.Store(currentY);

                yield return currentY;
            }

            while (true)
            {
                double currentY = 0.0;
                for (int i = 1; i <= n; i++)
                {
                    currentY -= outputbuffer.Peek(i) * an[i];
                }

                outputbuffer.Store(currentY);

                yield return currentY;
            }
        }

        /// <summary>
        ///     Calculates the frequency response of an IIR filter.
        /// </summary>
        /// <param name="a">The denominator coefficients.</param>
        /// <param name="b">The numerator coefficents.</param>
        /// <param name="frequencies">The frequencies.</param>
        /// <param name="samplerate">The samplerate.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static IEnumerable<Complex> IirFrequencyResponse(
            IReadOnlyList<double> a,
            IReadOnlyList<double> b,
            IReadOnlyList<double> frequencies,
            double samplerate)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            if (frequencies == null)
            {
                throw new ArgumentNullException(nameof(frequencies));
            }

            if (samplerate <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(samplerate));
            }

            if (a.Count < b.Count)
            {
                a = a.ZeroPad(b.Count - a.Count).ToReadOnlyList();
            }
            else if (b.Count < a.Count)
            {
                b = b.ZeroPad(a.Count - b.Count).ToReadOnlyList();
            }

            if ((a.Count == 0) || (a[0] == 0))
            {
                throw new Exception("a0 cannot be 0.");
            }

            var n = a.Count;
            double factor = 2 * Math.PI / samplerate;

            foreach (double d in frequencies)
            {
                var w = d * factor;
                Complex nom = 0;
                Complex den = 0;
                for (var c1 = 0; c1 < n; c1++)
                {
                    nom += b[c1] * Complex.Exp(-(n - c1) * Complex.ImaginaryOne * w);
                    den += a[c1] * Complex.Exp(-(n - c1) * Complex.ImaginaryOne * w);
                }

                yield return nom / den;
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
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            if (targetX == null)
            {
                throw new ArgumentNullException(nameof(targetX));
            }

            if (x.Count != y.Count)
            {
                throw new ArgumentException();
            }

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
            phase = UnwrapPhase(phase);

            var mspline = CubicSpline.Compute(actualX.ToArray(), magnitude.ToArray(), actualTargetX.ToArray());
            var pspline = CubicSpline.Compute(actualX.ToArray(), phase.ToArray(), actualTargetX.ToArray());

            return PolarToComplex(mspline, pspline);
        }

        /// <summary>
        ///     Approximates the Lambert W function.
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/Lambert_W_function#Numerical_evaluation</remarks>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static double LambertW(double input)
        {
            if (input < -1 / Math.E)
            {
                throw new ArgumentOutOfRangeException(nameof(input));
            }

            double wj = 0;
            double pwj;
            int i = 0;

            double ewj;

            do
            {
                ewj = Math.Exp(wj);
                pwj = wj;
                wj = wj - (wj * ewj - input) / (ewj * (wj + 1) - (wj + 2) * (wj * ewj - input) / (2 * wj + 2));
                i++;
                if (i > 1000)
                {
                    throw new Exception("Not converging...");
                }
            }
            while (Math.Abs(wj - pwj) > 1e-15);

            ewj = Math.Exp(wj);
            wj = wj - (wj * ewj - input) / (ewj * (wj + 1) - (wj + 2) * (wj * ewj - input) / (2 * wj + 2));

            return wj;
        }

        /// <summary>
        ///     Converts a single value from linear scale to dB.
        /// </summary>
        /// <param name="linear">The value in linear scale.</param>
        /// <param name="minValue">The minimum return value.</param>
        /// <returns>The value in dB.</returns>
        public static double LinearToDb(double linear, double minValue = double.NegativeInfinity)
        {
            if (linear <= 0)
            {
                return minValue;
            }

            return Math.Max(20 * Math.Log10(linear), minValue);
        }

        /// <summary>
        ///     Converts an array from linear scale to dB.
        /// </summary>
        /// <param name="linear">The array in linear scale.</param>
        /// <param name="minValue">The minimum return value.</param>
        /// <returns>A new array of the same length as <paramref name="linear" /> containing the result.</returns>
        public static IEnumerable<double> LinearToDb(IEnumerable<double> linear, double minValue = double.NegativeInfinity)
        {
            if (linear == null)
            {
                throw new ArgumentNullException(nameof(linear));
            }

            return linear.Select(d => LinearToDb(d, minValue));
        }

        /// <summary>
        ///     Generates a linear series of values between two points with a specified number of steps.
        /// </summary>
        /// <param name="from">The starting point of the series.</param>
        /// <param name="to">The stopping point of the series.</param>
        /// <param name="length">The number of steps (including starting and stopping points).</param>
        /// <returns>An array of length <paramref name="length" /> containing the result.</returns>
        public static IEnumerable<double> LinSeries(double from, double to, int length)
        {
            if (length < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            var d = (to - from) / (length - 1);
            return Enumerable.Range(0, length).Select(i => i * d + from);
        }

        /// <summary>
        ///     Generates a logarithmic value series between a start and a stop value with a specified number of steps.
        /// </summary>
        /// <param name="from">The start value.</param>
        /// <param name="to">The stop value.</param>
        /// <param name="steps">The number of steps (including start and stop values).</param>
        /// <returns>A new array of length <paramref name="steps" /> containing the result.</returns>
        public static IEnumerable<double> LogSeries(double from, double to, int steps)
        {
            if (steps < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(steps));
            }

            if (from <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(@from));
            }

            if (to <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(to));
            }

            var startValueLog = Math.Log(from);
            var stopValueLog = Math.Log(to);
            var stepSizeLog = (stopValueLog - startValueLog) / (steps - 1);

            return Enumerable.Range(0, steps).Select(i => Math.Exp(startValueLog + i * stepSizeLog));
        }

        /// <summary>
        ///     Computes a logarithmic sine sweep using the direct analytic approach proposed by Farina.
        /// </summary>
        /// <remarks>Angelo Farina - Simultaneous Measurement of Impulse Response and Distortion With a Swept-Sine Technique, 2000</remarks>
        /// <param name="from">The start frequency of the sweep in Hz.</param>
        /// <param name="to">The stop frequency of the sweep in Hz.</param>
        /// <param name="length">The length oft the sweep in seconds.</param>
        /// <param name="samplerate">The samplerate of the sweep.</param>
        public static IEnumerable<double> LogSweep(double from, double to, double length, double samplerate = 44100)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (samplerate <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(samplerate));
            }

            if (from <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(from));
            }

            if (to <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(to));
            }

            if (to == from)
            {
                throw new ArgumentException();
            }

            var w1 = Math.Min(from, to) * 2 * Math.PI;
            var w2 = Math.Max(from, to) * 2 * Math.PI;
            var steps = (int)(length * samplerate);

            var factor1 = w1 * length / Math.Log(w2 / w1);
            var factor2 = Math.Log(w2 / w1) / length;

            if (to > from)
            {
                for (int i = 0; i < steps; i++)
                {
                    yield return Math.Sin(factor1 * (Math.Exp(i / samplerate * factor2) - 1));
                }
            }
            else
            {
                for (int i = steps - 1; i >= 0; i--)
                {
                    yield return Math.Sin(factor1 * (Math.Exp(i / samplerate * factor2) - 1));
                }
            }
        }

        /// <summary>
        ///     Computes a logarithmic sweep using an alternative stepped algorithm [deprecated, untested, likely to be removed].
        /// </summary>
        /// <remarks>https://blogs.msdn.microsoft.com/matthew_van_eerde/2009/08/07/how-to-calculate-a-sine-sweep-the-right-way/</remarks>
        /// <param name="from">The start frequency of the sweep in Hz.</param>
        /// <param name="to">The stop frequency of the sweep in Hz.</param>
        /// <param name="length">The length oft the sweep in seconds.</param>
        /// <param name="samplerate">The samplerate of the sweep.</param>
        /// <param name="oversampling">
        ///     The oversampling used to calculate the sweep. Increases the accuracy of the phase
        ///     calculation, especially at higher frequencies.
        /// </param>
        public static IEnumerable<double> LogSweepAlternative(double from, double to, double length, double samplerate = 44100, int oversampling = 10)
        {
            var logAngularFrom = Math.Log(from * 2 * Math.PI / (samplerate * oversampling));
            var logAngularTo = Math.Log(to * 2 * Math.PI / (samplerate * oversampling));

            var steps = (int)(length * samplerate);
            var oversampledSteps = steps * oversampling;
            var logStep = (logAngularTo - logAngularFrom) / oversampledSteps;

            var logCurrentFrequency = logAngularFrom;
            var currentPhase = 0.0;

            for (var c = 0; c < oversampledSteps; c++)
            {
                if (c % oversampling == 0)
                {
                    yield return Math.Sin(currentPhase);
                }

                logCurrentFrequency += logStep;
                var currentFrequency = Math.Pow(Math.E, logCurrentFrequency);
                currentPhase += currentFrequency;
            }
        }

        /// <summary>
        ///     Finds the minimum distance between two neighbouring points of an array.
        /// </summary>
        /// <param name="input">The array.</param>
        /// <returns>The result.</returns>
        public static double MinimumDistance(IEnumerable<double> input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var inputlist = input.ToReadOnlyList();

            var ret = double.PositiveInfinity;
            for (var c = 1; c < inputlist.Count; c++)
            {
                ret = Math.Min(ret, inputlist[c] - inputlist[c - 1]);
            }

            return ret;
        }

        /// <summary>
        ///     Calculates the modulus (remainder of a division).
        /// </summary>
        /// <param name="x">The dividend.</param>
        /// <param name="m">The divisor.</param>
        /// <returns>The remainder of the division.</returns>
        public static int Mod(int x, int m)
        {
            if (x == 0)
            {
                return 0;
            }

            if (m == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(m));
            }

            return (x % m + m) % m;
        }

        /// <summary>
        ///     Calculates the modulus (remainder of a division).
        /// </summary>
        /// <param name="x">The dividend.</param>
        /// <param name="m">The divisor.</param>
        /// <returns>The remainder of the division.</returns>
        public static double Mod(double x, double m)
        {
            if (x == 0)
            {
                return 0;
            }

            if (m == 0)
            {
                return double.NaN;
            }

            return (x % m + m) % m;
        }

        /// <summary>
        ///     Calculates the modified bessel function of the first kind for a single value.
        /// </summary>
        /// <param name="x">The value.</param>
        /// <returns>The result.</returns>
        //TODO: Find better algorithm
        public static double ModBessel0(double x)
        {
            if (x < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (x < 4.9)
            {
                return 1 + Math.Pow(x, 2) / 4 + Math.Pow(x, 4) / 64 + Math.Pow(x, 6) / 2304 + Math.Pow(x, 8) / 147456 + Math.Pow(x, 10) / 14745600;
            }

            if (x > 5.1)
            {
                return Math.Pow(Math.E, x) / Math.Sqrt(2 * Math.PI * x) *
                       (1 + 1 / (8 * x) + 9 / (128 * Math.Pow(x, 2)) + 225 / (3072 * Math.Pow(x, 3)) + 11025 / (98304 * Math.Pow(x, 4)) +
                        893025 / (3932160 * x));
            }

            var t1 = 1 + Math.Pow(x, 2) / 4 + Math.Pow(x, 4) / 64 + Math.Pow(x, 6) / 2304 + Math.Pow(x, 8) / 147456 + Math.Pow(x, 10) / 14745600;
            var t2 = Math.Pow(Math.E, x) / Math.Sqrt(2 * Math.PI * x) *
                     (1 + 1 / (8 * x) + 9 / (128 * Math.Pow(x, 2)) + 225 / (3072 * Math.Pow(x, 3)) + 11025 / (98304 * Math.Pow(x, 4)) +
                      893025 / (3932160 * x));
            return t1 * (5.1 - x) / 0.2 + t2 * (x - 4.9) / 0.2;
        }

        /// <summary>
        ///     Converts two individual arrays containing magnitude and phase information to one complex array.
        /// </summary>
        /// <param name="amplitude">The amplitude data.</param>
        /// <param name="phase">The phase data. Has to be the same length as <paramref name="amplitude" />.</param>
        /// <returns>
        ///     A new complex array of the same length as <paramref name="amplitude" /> and <paramref name="phase" />
        ///     containing the result.
        /// </returns>
        public static IEnumerable<Complex> PolarToComplex(IEnumerable<double> amplitude, IEnumerable<double> phase)
        {
            if (amplitude == null)
            {
                throw new ArgumentNullException(nameof(amplitude));
            }

            if (phase == null)
            {
                throw new ArgumentNullException(nameof(phase));
            }

            return amplitude.Zip(phase, Complex.FromPolarCoordinates);
        }

        /// <summary>
        ///     Converts a single value from rad to degree.
        /// </summary>
        /// <param name="rad">The value in rad.</param>
        /// <returns>The value in degree.</returns>
        public static double RadToDeg(double rad)
        {
            return rad * 180 / Math.PI;
        }

        /// <summary>
        ///     Converts a sequence from rad to deg.
        /// </summary>
        /// <param name="rad">The array in rad.</param>
        /// <returns>A new sequence of the same length as <paramref name="rad" /> containing the result.</returns>
        public static IEnumerable<double> RadToDeg(IEnumerable<double> rad)
        {
            if (rad == null)
            {
                throw new ArgumentNullException(nameof(rad));
            }

            return rad.Select(RadToDeg);
        }

        /// <summary>
        ///     Performs a linear right-shift on a sequence, filling the beginning with zeros.
        /// </summary>
        /// <param name="input">The array to be shifted.</param>
        /// <param name="offset">The amount of samples the array should be shifted. If negative, the beginning of the sequence is truncated.</param>
        /// <returns>A sequence containing the result.</returns>
        public static IEnumerable<double> RightShift(IEnumerable<double> input, int offset)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return offset > 0 ? input.LeadingZeros(offset) : input.Skip(-offset);
        }

        /// <summary>
        ///     Calculates the sinc = sin(pi * x) / (pi * x) of a single value.
        /// </summary>
        /// <param name="x">The value.</param>
        /// <returns>The result.</returns>
        public static double Sinc(double x)
        {
            if (x == 0)
            {
                return 1;
            }

            return Math.Sin(Math.PI * x) / (Math.PI * x);
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
        public static IEnumerable<double> Smooth(IReadOnlyList<double> x, IReadOnlyList<double> y, int resolution, bool logX = true)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            if (x.Count != y.Count)
            {
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + " should be the same length.");
            }

            if (x.Count == 0)
            {
                throw new ArgumentException(nameof(x) + " and " + nameof(y) + " cannot be empty.");
            }

            if (resolution < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(resolution));
            }

            if (resolution == 0)
            {
                foreach (var d in y)
                {
                    yield return d;
                }

                yield break;
            }

            Func<double, double, double, double> smoothWindow = (logF, logF0, bw) => 
            {
                var argument = (logF - logF0) / bw * Math.PI;
                if (Math.Abs(argument) >= Math.PI)
                {
                    return 0;
                }

                return 0.5 * (1.0 + Math.Cos(argument));
            };

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
                    {
                        break;
                    }

                    factor = smoothWindow(actualX[fc2], actualX[fc], bandwidth);
                    factorSum += factor;
                    sum += factor * y[fc2];
                    fc2 -= 1;
                }

                fc2 = fc + 1;
                while (fc2 < actualX.Count)
                {
                    if (!(actualX[fc2] < actualX[fc] + bandwidth))
                    {
                        break;
                    }

                    factor = smoothWindow(actualX[fc2], actualX[fc], bandwidth);
                    factorSum += factor;
                    sum += factor * y[fc2];
                    fc2 += 1;
                }

                yield return sum / factorSum;
            }
        }

        /// <summary>
        ///     Unwraps phase information.
        /// </summary>
        /// <param name="phase">The phase sequence.</param>
        /// <param name="useDeg">If true, the phase unit is assumed to be degree, otherwise rad (default).</param>
        /// <returns>A new sequence of the same length as <paramref name="phase" /> containing the result.</returns>
        public static IEnumerable<double> UnwrapPhase(IEnumerable<double> phase, bool useDeg = false)
        {
            if (phase == null)
            {
                throw new ArgumentNullException(nameof(phase));
            }

            double fullPeriod;
            double halfPeriod;
            if (useDeg)
            {
                fullPeriod = 360;
                halfPeriod = 180;
            }
            else
            {
                fullPeriod = 2 * Math.PI;
                halfPeriod = Math.PI;
            }

            double offset = 0;

            var e = phase.GetEnumerator();
            if (!e.MoveNext())
            {
                yield break;
            }

            var previousPhase = e.Current;
            yield return e.Current;

            while (e.MoveNext())
            {
                if (previousPhase - e.Current > halfPeriod)
                {
                    offset += fullPeriod;
                }
                else if (previousPhase - e.Current < -halfPeriod)
                {
                    offset -= fullPeriod;
                }

                previousPhase = e.Current;
                yield return e.Current + offset;
            }
        }

        /// <summary>
        ///     Calculates an infinite white noise sequence.
        /// </summary>
        /// <returns></returns>
        /// <remarks>http://dspguru.com/dsp/howtos/how-to-generate-white-gaussian-noise</remarks>
        public static IEnumerable<double> WhiteNoise()
        {
            // this is to prevent multiple consecutive calls to this function getting the same seed
            var seed = unchecked((int)(DateTime.Now.Ticks + WhiteNoiseSeedNumber++));
            var rnd = new Random(seed);

            while (true)
            {
                double v1, v2, s;
                do
                {
                    v1 = 2 * rnd.NextDouble() - 1;
                    v2 = 2 * rnd.NextDouble() - 1;
                    s = v1 * v1 + v2 * v2;
                }
                while (s >= 1);

                yield return Math.Sqrt(-2 * Math.Log(s) / s) * v1;
                yield return Math.Sqrt(-2 * Math.Log(s) / s) * v2;
            }
            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        ///     Generates a sinc pulse, multiplied by a symmetrical rectangle window to make its length finite.
        /// </summary>
        /// <param name="frequency">The frequency of the sinc pulse.</param>
        /// <param name="samplerate">The samplerate at which the sinc pulse should be generated.</param>
        /// <param name="length">The length of the resulting sinc pulse.</param>
        /// <param name="start">The start time (in samples).</param>
        /// <returns>An array of length <paramref name="length" /> containing the result.</returns>
        public static IEnumerable<double> WindowedSinc(double frequency, double samplerate, int length, int start = 0)
        {
            if (frequency <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(frequency));
            }

            if (samplerate <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(samplerate));
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            double factor = 2 * Math.PI * frequency / samplerate;
            return Enumerable.Range(start, length).Select(
                i =>
                {
                    if (i == 0)
                    {
                        return 1;
                    }

                    var omega = i * factor;
                    return Math.Sin(omega) / omega;
                });
        }

        /// <summary>
        ///     Wraps phase data from an array, so that all resulting values are in the range -180° to +180° (or -pi to +pi).
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <param name="useDeg">If true, the angular unit is assumed to be degree, otherwise radians (default).</param>
        /// <returns></returns>
        public static IEnumerable<double> WrapPhase(IEnumerable<double> input, bool useDeg = false)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            double fullPeriod;
            double halfPeriod;

            if (useDeg)
            {
                fullPeriod = 360;
                halfPeriod = 180;
            }
            else
            {
                fullPeriod = 2 * Math.PI;
                halfPeriod = Math.PI;
            }

            return input.Select(
                d =>
                {
                    var tmp = d % fullPeriod;
                    if (tmp > halfPeriod)
                    {
                        tmp -= fullPeriod;
                    }
                    else if (tmp < -halfPeriod)
                    {
                        tmp += fullPeriod;
                    }

                    return tmp;
                });
        }
    }
}