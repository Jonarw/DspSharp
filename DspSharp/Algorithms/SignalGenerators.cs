// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignalGenerators.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using DspSharp.Signal.Windows;

namespace DspSharp.Algorithms
{
    public static class SignalGenerators
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

        /// <summary>
        ///     Feedback taps for MLS orders 2 to 31.
        /// </summary>
        /// <remarks>Taps 5 through 18 are brute-force optimized to allow the maximum possible crest factor when filtering the sequence.</remarks>
        public static readonly IReadOnlyList<uint> MlsFeedbackTaps = new List<uint>
        {
            0, //dummy

            0b0000_0000_0000_0000_0000_0000_0000_0001u, //dummy
            0b0000_0000_0000_0000_0000_0000_0000_0011u, //2,1
            0b0000_0000_0000_0000_0000_0000_0000_0110u, //3,2
            0b0000_0000_0000_0000_0000_0000_0000_1100u, //4,3

            0b0000_0000_0000_0000_0000_0000_0001_0100u, //5,3
            0b0000_0000_0000_0000_0000_0000_0011_0110u, //6,5,3,2
            0b0000_0000_0000_0000_0000_0000_0101_1100u, //7,5,4,3
            0b0000_0000_0000_0000_0000_0000_1101_0100u, //8,7,5,3

            0b0000_0000_0000_0000_0000_0001_1011_1001u, //9,8,6,5,4,1
            0b0000_0000_0000_0000_0000_0011_1011_0001u, //10,9,8,6,5,1
            0b0000_0000_0000_0000_0000_0110_1010_0000u, //11,10,8,6
            0b0000_0000_0000_0000_0000_1110_0110_0010u, //12,11,10,7,6,2

            0b0000_0000_0000_0000_0001_0101_0101_1000u, //13,11,9,7,5,4
            0b0000_0000_0000_0000_0011_1001_0110_0000u, //14,13,12,9,7,6
            0b0000_0000_0000_0000_0100_1000_0011_0000u, //15,12,6,5
            0b0000_0000_0000_0000_1110_0010_1101_1000u, //16,15,14,10,8,7,5,4

            0b0000_0000_0000_0001_1101_1101_0001_1010u, //17,16,15,13,12,11,9,5,4,2
            0b0000_0000_0000_0010_0000_0100_0000_0000u, //18,17,16,13,12,11,9,8,7,5,4,2
            0b0000_0000_0000_0100_0000_0000_0010_0011u, //19,6,2,1
            0b0000_0000_0000_1001_0000_0000_0000_0000u, //20,17

            0b0000_0000_0001_0100_0000_0000_0000_0000u, //21,19
            0b0000_0000_0011_0000_0000_0000_0000_0000u, //22,21
            0b0000_0000_0100_0010_0000_0000_0000_0000u, //23,18
            0b0000_0000_1110_0001_0000_0000_0000_0000u, //24,23,22,17

            0b0000_0001_0010_0000_0000_0000_0000_0000u, //25,22
            0b0000_0010_0000_0000_0000_0000_0010_0011u, //26,6,2,1
            0b0000_0100_0000_0000_0000_0000_0001_0011u, //27,5,2,1
            0b0000_1001_0000_0000_0000_0000_0000_0000u, //28,25

            0b0001_0100_0000_0000_0000_0000_0000_0000u, //29,27
            0b0010_0000_0000_0000_0000_0000_0010_1001u, //30,6,4,1
            0b0100_1000_0000_0000_0000_0000_0000_0000u //31,28
            //0b1000_0000_0010_0000_0000_0000_0000_0011u //32,22,2,1 not working due to overflow
        }.AsReadOnly();

        private static int WhiteNoiseSeedNumber { get; set; }

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
        public static IEnumerable<double> AlignedLogSweep(
            double from,
            double to,
            double length,
            SweepAlignments alignment,
            double samplerate = 44100)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (from <= 0)
                throw new ArgumentOutOfRangeException(nameof(from));

            if (to <= 0)
                throw new ArgumentOutOfRangeException(nameof(to));

            if (samplerate <= 0)
                throw new ArgumentOutOfRangeException(nameof(samplerate));

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (to == from)
                throw new ArgumentException(nameof(to) + " and " + nameof(from) + " cannot be the same.");

            if (alignment == SweepAlignments.None)
                return LogSweep(from, to, length, samplerate);

            length = Convert.ToInt32(length * samplerate) / samplerate;

            var w1 = Math.Min(to, from) * 2 * Math.PI;
            var w2 = Math.Max(to, from) * 2 * Math.PI;
            var k = length * (w2 - w1) / (Math.PI * Math.Log(w2 / w1));

            if (alignment == SweepAlignments.Zero)
                k = Convert.ToInt32(k);
            else if (alignment == SweepAlignments.NegativeZero)
                k = Convert.ToInt32(0.5 * k) * 2;
            else if (alignment == SweepAlignments.PositiveZero)
                k = Convert.ToInt32(0.5 * (k + 1)) * 2 - 1;
            else if (alignment == SweepAlignments.NegativeOne)
                k = Convert.ToInt32(0.5 * (k + 0.5)) * 2 - 0.5;
            else if (alignment == SweepAlignments.PositiveOne)
                k = Convert.ToInt32(0.5 * (k - 0.5)) * 2 + 0.5;

            w2 = Mathematic.FindRoot(w2N => length * (w2N - w1) / (Math.PI * Math.Log(w2N / w1)) - k, w2, 1);

            var actualfrom = from < to ? from : w2 / (2 * Math.PI);
            var actualto = from < to ? w2 / (2 * Math.PI) : to;
            return LogSweep(actualfrom, actualto, length, samplerate);
        }

        /// <summary>
        ///     Generates a dirac pulse.
        /// </summary>
        /// <param name="count">The length of the dirac.</param>
        /// <returns></returns>
        public static IReadOnlyList<double> GetDirac(int count)
        {
            var ret = new double[count - 1];
            return 1d.ToEnumerable().Concat(ret).ToReadOnlyList();
        }

        /// <summary>
        ///     Generates a sequence of zeros.
        /// </summary>
        /// <param name="count">The number of zeros.</param>
        /// <returns></returns>
        public static IReadOnlyList<double> GetZeros(int count)
        {
            var ret = new double[count];
            return ret.ToReadOnlyList();
        }

        public static int GetMlsLength(int order)
        {
            return (1 << order) - 1;
        }

        public static int GetIrsLength(int order)
        {
            return (1 << (order + 1)) - 2;
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
                throw new ArgumentOutOfRangeException(nameof(frequency));

            if (samplerate <= 0)
                throw new ArgumentOutOfRangeException(nameof(samplerate));

            yield return 1;

            var factor = 2 * Math.PI * frequency / samplerate;
            var c = 1;
            while (true)
            {
                var omega = c * factor;
                yield return Math.Sin(omega) / omega;
                c++;
            }
            // ReSharper disable once FunctionNeverReturns
            // Output is meant to be infinte
        }

        //TODO: unit test
        /// <summary>
        ///     Generates an inverse repeated MLS sequence of the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>The IRS sequence.</returns>
        public static IEnumerable<double> Irs(int order)
        {
            if (order < 2 || order > MlsFeedbackTaps.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(order));

            var mls = Mls(order).ToReadOnlyList();

            using (var e = mls.GetEnumerator())
            {
                while (true)
                {
                    e.MoveNext();
                    yield return e.Current;

                    if (e.MoveNext())
                        yield return -e.Current;
                    else
                        break;
                }
            }

            using (var e = mls.GetEnumerator())
            {
                while (true)
                {
                    e.MoveNext();
                    yield return -e.Current;

                    if (e.MoveNext())
                        yield return e.Current;
                    else
                        break;
                }
            }
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
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));

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
                throw new ArgumentOutOfRangeException(nameof(steps));

            if (from <= 0)
                throw new ArgumentOutOfRangeException(nameof(from));

            if (to <= 0)
                throw new ArgumentOutOfRangeException(nameof(to));

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
                throw new ArgumentOutOfRangeException(nameof(length));

            if (samplerate <= 0)
                throw new ArgumentOutOfRangeException(nameof(samplerate));

            if (from <= 0)
                throw new ArgumentOutOfRangeException(nameof(from));

            if (to <= 0)
                throw new ArgumentOutOfRangeException(nameof(to));

            if (to == from)
                throw new ArgumentException();

            var w1 = Math.Min(from, to) * 2 * Math.PI;
            var w2 = Math.Max(from, to) * 2 * Math.PI;
            var steps = (int)(length * samplerate);

            var factor1 = w1 * length / Math.Log(w2 / w1);
            var factor2 = Math.Log(w2 / w1) / length;

            if (to > from)
            {
                for (var i = 0; i < steps; i++)
                {
                    yield return Math.Sin(factor1 * (Math.Exp(i / samplerate * factor2) - 1));
                }
            }
            else
            {
                for (var i = steps - 1; i >= 0; i--)
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
        public static IEnumerable<double> LogSweepAlternative(
            double from,
            double to,
            double length,
            double samplerate = 44100,
            int oversampling = 10)
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
                    yield return Math.Sin(currentPhase);

                logCurrentFrequency += logStep;
                var currentFrequency = Math.Pow(Math.E, logCurrentFrequency);
                currentPhase += currentFrequency;
            }
        }

        public static void LogSweepAndInverse(
            double from,
            double to,
            int length,
            double samplerate,
            out IReadOnlyList<double> sweep,
            out IReadOnlyList<double> inverse)
        {
            var sw = AlignedLogSweep(from, to, length, SweepAlignments.Zero, samplerate).ToReadOnlyList();
            var win = Window.CreateWindow(WindowTypes.Hann, WindowModes.Symmetric, sw.Count, .1);

            sweep = sw.Multiply(win).ToReadOnlyList();
            var c = sweep.Count;

            //var fftsw = Fft.RealFft(sweep.Reverse());
            inverse = sweep.Reverse()
                .Select(
                    (d, i) => d * Math.Pow(to / from, -(double)i / c))
                .ToReadOnlyList();

            //var frequencies = Fft.GetFrequencies(samplerate, sweep.Count);
            //var fftinv = fftsw.Zip(frequencies,
            //    (c, f) =>
            //    {
            //        return c;

            //        if (f < from)
            //        {
            //            return c;
            //        }

            //        if (f > to)
            //        {
            //            return c * to / from;
            //        }

            //        return c * f / from;
            //    });

            //inverse = Fft.RealIfft(fftinv);
        }

        public static IEnumerable<double> Mls(uint taps)
        {
            const uint startState = 1 << 1;
            var state = startState;

            do
            {
                var lsb = 1 & state;
                state >>= 1;

                if (lsb > 0)
                {
                    state ^= taps;
                    yield return 1;
                }
                else
                    yield return -1;
            }
            while (state != startState);
        }

        /// <summary>
        ///     Generates a maximum length sequence of the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>The maximum length sequence.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static IEnumerable<double> Mls(int order)
        {
            if (order < 2 || order > MlsFeedbackTaps.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(order));

            return Mls(MlsFeedbackTaps[order]);
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
        public static IEnumerable<double> Slope(
            IEnumerable<double> x,
            double startX,
            double stopX,
            double startValue,
            double stopValue,
            SlopeModes mode = SlopeModes.Smooth,
            bool logarithmicX = true)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

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
                yield break;

            double SmoothSlope(double input) => -0.5 * (Math.Cos(Math.PI * input) - 1);

            var deltaF = actualStopX - actualStartX;
            var deltaV = stopValue - startValue;

            foreach (var f in actualX)
            {
                double actualGain;
                if (f <= actualStartX)
                    actualGain = startValue;
                else
                {
                    if (f >= actualStopX)
                        actualGain = stopValue;
                    else
                    {
                        var tmpgain = (f - actualStartX) / deltaF;
                        if (mode == SlopeModes.Smooth)
                            tmpgain = SmoothSlope(tmpgain);

                        tmpgain = deltaV * tmpgain + startValue;
                        actualGain = tmpgain;
                    }
                }

                yield return actualGain;
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
            // ReSharper disable once IteratorNeverReturns
        }

        //TODO: unit test
        public static IReadOnlyList<double> FadedLogSweep(
            double from,
            double to,
            double length,
            int fadeIn,
            int fadeOut,
            WindowTypes fadeWindowType,
            double samplerate = 44100)
        {
            var ret = LogSweep(from, to, length, samplerate).ToList();
            var startWin = Window.GetAntiCausalHalfWindow(fadeWindowType, fadeIn).ToReadOnlyList();
            var endWin = Window.GetCausalHalfWindow(fadeWindowType, fadeOut).ToReadOnlyList();

            fadeIn = Math.Min(fadeIn, ret.Count);
            fadeOut = Math.Min(fadeOut, ret.Count);

            for (int i = 0; i < fadeIn; i++)
            {
                ret[i] *= startWin[i];
            }

            for (int i = 0; i < fadeOut; i++)
            {
                ret[ret.Count - fadeOut + i] *= endWin[i];
            }

            return ret.ToReadOnlyList();
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
                throw new ArgumentOutOfRangeException(nameof(frequency));

            if (samplerate <= 0)
                throw new ArgumentOutOfRangeException(nameof(samplerate));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var factor = 2 * Math.PI * frequency / samplerate;
            return Enumerable.Range(start, length)
                .Select(
                    i =>
                    {
                        if (i == 0)
                            return 1;

                        var omega = i * factor;
                        return Math.Sin(omega) / omega;
                    });
        }
    }
}