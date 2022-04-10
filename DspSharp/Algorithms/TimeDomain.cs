// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeDomain.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Buffers;
using DspSharp.Extensions;
using DspSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Algorithms
{
    public static class TimeDomain
    {
        /// <summary>
        /// Computes the circular convolution of the specified signals.
        /// </summary>
        /// <param name="signal1">The first signal.</param>
        /// <param name="signal2">The second signal.</param>
        /// <remarks>This uses a FFT-based fast convolution.</remarks>
        public static IReadOnlyList<double> CircularConvolve(
            IReadOnlyList<double> signal1,
            IReadOnlyList<double> signal2)
        {
            if ((signal1.Count == 0) || (signal2.Count == 0))
                return Array.Empty<double>();

            var n = Math.Max(signal1.Count, signal2.Count);
            var spectrum1 = Fft.RealFft(signal1, n);
            var spectrum2 = Fft.RealFft(signal2, n);
            var spectrum = spectrum2.Multiply(spectrum1);
            return Fft.RealIfft(spectrum, n);
        }

        /// <summary>
        /// Computes the cross-correlation of two finite signals.
        /// </summary>
        /// <param name="signal1">The first signal.</param>
        /// <param name="signal2">The second signal.</param>
        public static IReadOnlyList<double> CircularCrossCorrelate(
            IReadOnlyList<double> signal1,
            IReadOnlyList<double> signal2)
        {
            return CircularConvolve(signal2.Reverse().ToList(), signal1);
        }

        /// <summary>
        /// Convolves the specified signals.
        /// </summary>
        /// <param name="signal1">The first signal.</param>
        /// <param name="signal2">The second signal.</param>
        /// <remarks>This uses a FFT-based fast convolution. The whole convoluation is evaluated in a single FFT.</remarks>
        public static IReadOnlyList<double> Convolve(IReadOnlyCollection<double> signal1, IReadOnlyCollection<double> signal2)
        {
            if ((signal1.Count == 0) || (signal2.Count == 0))
                return Array.Empty<double>();

            var l = signal1.Count + signal2.Count - 1;
            var spectrum1 = Fft.RealFft(signal1, l);
            var spectrum2 = Fft.RealFft(signal2, l);
            var spectrum = spectrum2.Multiply(spectrum1);
            return Fft.RealIfft(spectrum, l);
        }

        /// <summary>
        /// Convolves the specified signals.
        /// </summary>
        /// <param name="signal1">The first signal. Can be of infinite length.</param>
        /// <param name="signal2">The second signal.</param>
        /// <returns>The convolution of the two signals.</returns>
        /// <remarks>This uses a FFT-based fast convolution with overlap&add. Blocks are computed as they are requested.</remarks>
        public static IEnumerable<double> Convolve(IEnumerable<double> signal1, IReadOnlyCollection<double> signal2)
        {
            if (signal2.Count == 0)
                yield break;

            using var e1 = signal1.GetEnumerator();

            var blockSize = signal2.Count;
            var n = blockSize * 2;
            var sig2Fft = Fft.RealFft(signal2, n);

            var sig1Buffer = new List<double>(signal2.Count);
            var previousBlock = new double[n];

            while (true)
            {
                var c = 0;
                while ((c < blockSize) && e1.MoveNext())
                {
                    sig1Buffer.Add(e1.Current);
                    c++;
                }

                var sig1Fft = Fft.RealFft(sig1Buffer, n);
                sig1Buffer.Clear();

                var spec = sig1Fft.Multiply(sig2Fft);
                var currentBlock = Fft.RealIfft(spec, n);

                for (var i = 0; i < blockSize; i++)
                    yield return currentBlock[i] + previousBlock[i + blockSize];

                if (c < blockSize)
                {
                    for (var i = blockSize; i < blockSize + c - 1; i++)
                        yield return currentBlock[i];

                    yield break;
                }

                previousBlock = currentBlock;
            }
        }

        /// <summary>
        /// Computes the cross-correlation of two finite signals.
        /// </summary>
        /// <param name="signal1">The first signal.</param>
        /// <param name="signal2">The second signal.</param>
        public static IReadOnlyList<double> CrossCorrelate(IReadOnlyList<double> signal1, IReadOnlyList<double> signal2)
        {
            return Convolve(signal2.ReverseIndexed(), signal1);
        }

        /// <summary>
        /// Computes the cross-correlation of two signals.
        /// </summary>
        /// <param name="signal1">The second signal (can be infinite).</param>
        /// <param name="signal2">The first signal.</param>
        public static IEnumerable<double> CrossCorrelate(IEnumerable<double> signal1, IReadOnlyList<double> signal2)
        {
            return Convolve(signal1, signal2.ReverseIndexed());
        }

        /// <summary>
        /// Applies an IIR filter to the provided input signal.
        /// </summary>
        /// <param name="input">The input signal.</param>
        /// <param name="a">The denominator coefficients of the filter.</param>
        /// <param name="b">The numerator coefficients of the filter.</param>
        /// <param name="inputbuffer">The inputbuffer. If not provided, an empty buffer is created.</param>
        /// <param name="outputbuffer">The outputbuffer. If not provided, an empty buffer is created.</param>
        /// <returns>The filter output. CAUTION: This will be infinitely long.</returns>
        public static IEnumerable<double> IirFilter(
            IEnumerable<double> input,
            IReadOnlyList<double> a,
            IReadOnlyList<double> b,
            CircularBuffer<double> inputbuffer = null,
            CircularBuffer<double> outputbuffer = null)
        {
            if (a.Count < b.Count)
                a = a.PadEnd(b.Count - a.Count).ToList();
            else if (b.Count < a.Count)
                b = b.PadEnd(a.Count - b.Count).ToList();

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
            else if ((inputbuffer.Length != n) || (outputbuffer.Length != n))
            {
                throw new ArgumentException();
            }

            var an = a.Multiply(1 / a[0]).ToList();
            var bn = b.Multiply(1 / a[0]).ToList();

            // process all input elements
            foreach (var item in input)
            {
                var currentY = item * bn[0];

                for (var i = 1; i <= n; i++)
                {
                    currentY += inputbuffer.Peek(-i) * bn[i];
                    currentY -= outputbuffer.Peek(-i) * an[i];
                }

                inputbuffer.Store(item);
                outputbuffer.Store(currentY);

                yield return currentY;
            }

            // continue until input buffer is empty
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

            // infinite tail
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

        //TODO: unit test
        /// <summary>
        /// Normalizes a signal by dividing all elements by the absolute maximum of the signal.
        /// </summary>
        public static ILazyReadOnlyList<double> Normalize(this IReadOnlyList<double> signal)
        {
            var max = signal.AbsMax();
            return signal.SelectIndexed(d => d / max);
        }
    }
}