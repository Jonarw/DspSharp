// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeDomain.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DspSharp.Buffers;
using DspSharp.Signal.Windows;

namespace DspSharp.Algorithms
{
    public static class TimeDomain
    {
        //TODO: unit test
        public static IEnumerable<double> Normalize(this IReadOnlyList<double> signal)
        {
            var max = Math.Max(signal.Max(), -signal.Min());
            return signal.Divide(max);
        }

        /// <summary>
        ///     Convolves the specified finite signals.
        /// </summary>
        /// <param name="signal1">The first signal.</param>
        /// <param name="signal2">The second signal.</param>
        /// <returns>The convolution of the two signals.</returns>
        public static IReadOnlyList<double> CircularConvolve(
            IReadOnlyList<double> signal1,
            IReadOnlyList<double> signal2)
        {
            if (signal1 == null)
                throw new ArgumentNullException(nameof(signal1));

            if (signal2 == null)
                throw new ArgumentNullException(nameof(signal2));

            if ((signal1.Count == 0) || (signal2.Count == 0))
                return new List<double>().AsReadOnly();

            var n = Math.Max(signal1.Count, signal2.Count);
            var spectrum1 = Fft.RealFft(signal1, n);
            var spectrum2 = Fft.RealFft(signal2, n);
            var spectrum = spectrum2.Multiply(spectrum1);
            return Fft.RealIfft(spectrum, n).ToReadOnlyList();
        }

        /// <summary>
        ///     Computes the cross-correlation of two finite signals.
        /// </summary>
        /// <param name="signal1">The first signal.</param>
        /// <param name="signal2">The second signal.</param>
        /// <returns>The result of the computation.</returns>
        public static IReadOnlyList<double> CircularCrossCorrelate(
            IReadOnlyList<double> signal1,
            IReadOnlyList<double> signal2)
        {
            if (signal1 == null)
                throw new ArgumentNullException(nameof(signal1));

            if (signal2 == null)
                throw new ArgumentNullException(nameof(signal2));

            return CircularConvolve(signal2.Reverse().ToReadOnlyList(), signal1);
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
                throw new ArgumentOutOfRangeException(nameof(sampleRate));

            var mod = Math.Abs(delay % (1 / sampleRate));

            if ((mod > 1e-13) && (mod < 1 / sampleRate - 1e-13))
                integer = false;
            else
                integer = true;

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
                throw new ArgumentNullException(nameof(signal1));

            if (signal2 == null)
                throw new ArgumentNullException(nameof(signal2));

            if ((signal1.Count == 0) || (signal2.Count == 0))
                return new List<double>().AsReadOnly();

            var l = signal1.Count + signal2.Count - 1;
            var n = Fft.GetOptimalFftLength(l);
            var spectrum1 = Fft.RealFft(signal1, n);
            var spectrum2 = Fft.RealFft(signal2, n);
            var spectrum = spectrum2.Multiply(spectrum1);
            return Fft.RealIfft(spectrum, n).Take(l).ToReadOnlyList();
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
                throw new ArgumentNullException(nameof(signal1));

            if (signal2 == null)
                throw new ArgumentNullException(nameof(signal2));

            if (signal2.Count == 0)
                yield break;

            using (var e1 = signal1.GetEnumerator())
            {
                var n = 2 * Fft.GetOptimalFftLength(signal2.Count);
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
                        blockconv = Fft.RealIfft(spec, n);
                        ret = blockconv;
                    }
                    else
                    {
                        blockconv = Enumerable.Empty<double>().ToReadOnlyList();
                        ret = Enumerable.Empty<double>();
                    }

                    if (buffer != null)
                        ret = ret.AddFull(buffer);

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
                throw new ArgumentNullException(nameof(signal1));

            if (signal2 == null)
                throw new ArgumentNullException(nameof(signal2));

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
                throw new ArgumentNullException(nameof(signal2));

            if (signal1 == null)
                throw new ArgumentNullException(nameof(signal1));

            return Convolve(signal1, signal2.Reverse().ToReadOnlyList());
        }

        //TODO: unit test
        public static IReadOnlyList<double> FrequencyWindowedBandpass(
            IReadOnlyList<double> input,
            double samplerate,
            double fc1,
            double fc2,
            double windowBwL,
            double windowBwH,
            WindowTypes windowType)
        {
            return Fft.RealIfft(FrequencyWindowedBandpassSpectrum(input, samplerate, fc1, fc2, windowBwL, windowBwH, windowType), input.Count);
        }

        public static IReadOnlyList<Complex> FrequencyWindowedBandpassSpectrum(
            IReadOnlyList<double> input,
            double samplerate,
            double fc1,
            double fc2,
            double windowBwL,
            double windowBwH,
            WindowTypes windowType)
        {
            var fftinput = Fft.RealFft(input);
            var frequencies = Fft.GetFrequencies(samplerate, input.Count).ToReadOnlyList();
            var winfunc = Window.GetWindowFunction(windowType);

            var spec = fftinput.Zip(
                frequencies,
                (c, f) =>
                {
                    if ((f <= fc1) || (f >= fc2))
                        return 0;

                    if ((f >= fc1 + windowBwL) && (f <= fc2 - windowBwH))
                        return c;

                    if (f < fc1 + windowBwL)
                        return c * winfunc((f - fc1) / windowBwL);

                    return c * winfunc((fc2 - f) / windowBwH);
                });

            return spec.ToReadOnlyList();
        }

        //TODO: unit test
        public static IReadOnlyList<double> FrequencyWindowedInversion(
            IReadOnlyList<double> input,
            double samplerate,
            double fc1,
            double fc2,
            double windowBwL,
            double windowBwH,
            WindowTypes windowType)
        {
            return
                Fft.RealIfft(
                    FrequencyWindowedInversionSpectrum(
                        input,
                        samplerate,
                        fc1,
                        fc2,
                        windowBwL,
                        windowBwH,
                        windowType), input.Count);
        }

        //TODO: unit test
        public static IReadOnlyList<double> FrequencyWindowedInversionAlt(
            IReadOnlyList<double> input,
            double samplerate,
            double fc1,
            double fc2,
            double windowBwL,
            double windowBwH,
            WindowTypes windowType)
        {
            return
                Fft.RealIfft(
                    FrequencyWindowedInversionSpectrumAlt(
                        input,
                        samplerate,
                        fc1,
                        fc2,
                        windowBwL,
                        windowBwH,
                        windowType), input.Count);
        }

        //TODO: unit test
        public static IReadOnlyList<Complex> FrequencyWindowedInversionSpectrum(
            IReadOnlyList<double> input,
            double samplerate,
            double fc1,
            double fc2,
            double windowBwL,
            double windowBwH,
            WindowTypes windowType)
        {
            var fftinput = Fft.RealFft(input);
            var frequencies = Fft.GetFrequencies(samplerate, input.Count).ToReadOnlyList();
            var winfunc = Window.GetWindowFunction(windowType);

            var spec = fftinput.Zip(
                frequencies,
                (c, f) =>
                {
                    if (c.Magnitude < 1e-10)
                        return 0;

                    if ((f <= fc1) || (f >= fc2))
                        return 0;

                    if ((f >= fc1 + windowBwL) && (f <= fc2 - windowBwH))
                        return 1 / c;

                    if (f < fc1 + windowBwL)
                        return 1 / c * winfunc((f - fc1) / windowBwL);

                    return 1 / c * winfunc((fc2 - f) / windowBwH);
                });

            return spec.ToReadOnlyList();
        }

        //TODO: unit test
        public static IReadOnlyList<Complex> FrequencyWindowedInversionSpectrumAlt(
            IReadOnlyList<double> input,
            double samplerate,
            double fc1,
            double fc2,
            double windowBwL,
            double windowBwH,
            WindowTypes windowType)
        {
            var fftinput = Fft.RealFft(input);

            var startbin = Convert.ToInt32(Math.Ceiling(fc1 / samplerate * fftinput.Count * 2));
            var stopbin = Convert.ToInt32(Math.Floor(fc2 / samplerate * fftinput.Count * 2));
            var ret = new Complex[fftinput.Count];

            for (int i = startbin; i <= stopbin; i++)
            {
                ret[i] = 1 / fftinput[i];
            }

            for (int i = 0; i < startbin; i++)
            {
                ret[i] = 0;
            }

            for (int i = stopbin + 1; i < fftinput.Count; i++)
            {
                ret[i] = 0;
            }

            return ret.ToReadOnlyList();
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
                throw new ArgumentNullException(nameof(input));

            if (a == null)
                throw new ArgumentNullException(nameof(a));

            if (b == null)
                throw new ArgumentNullException(nameof(b));

            if (a.Count < b.Count)
                a = a.PadRight(b.Count - a.Count).ToReadOnlyList();
            else
            {
                if (b.Count < a.Count)
                    b = b.PadRight(a.Count - b.Count).ToReadOnlyList();
            }

            if ((a.Count == 0) || (a[0] == 0))
                throw new Exception("a0 cannot be 0.");

            var n = a.Count - 1;

            if (n < 0)
                yield break;

            if ((inputbuffer == null) || (outputbuffer == null))
            {
                inputbuffer = new CircularBuffer<double>(n);
                outputbuffer = new CircularBuffer<double>(n);
            }
            else
            {
                if ((inputbuffer.Length != n) || (outputbuffer.Length != n))
                    throw new ArgumentException();
            }

            var an = a.Multiply(1 / a[0]).ToReadOnlyList();
            var bn = b.Multiply(1 / a[0]).ToReadOnlyList();
            using (var e = input.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    var currentY = e.Current * bn[0];

                    for (var i = 1; i <= n; i++)
                    {
                        currentY += inputbuffer.Peek(-i) * bn[i];
                        currentY -= outputbuffer.Peek(-i) * an[i];
                    }

                    inputbuffer.Store(e.Current);
                    outputbuffer.Store(currentY);

                    yield return currentY;
                }
            }

            if (clip)
                yield break;

            for (var i2 = 0; i2 < inputbuffer.Length; i2++)
            {
                var currentY = 0.0;

                for (var i = 1; i <= n; i++)
                {
                    currentY += inputbuffer.Peek(-i) * bn[i];
                    currentY -= outputbuffer.Peek(-i) * an[i];
                }

                inputbuffer.Store(0.0);
                outputbuffer.Store(currentY);

                yield return currentY;
            }

            while (true)
            {
                var currentY = 0.0;
                for (var i = 1; i <= n; i++)
                {
                    currentY -= outputbuffer.Peek(-i) * an[i];
                }

                outputbuffer.Store(currentY);

                yield return currentY;
            }
        }
    }
}