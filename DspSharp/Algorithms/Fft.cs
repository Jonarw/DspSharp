// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Fft.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DspSharp.Algorithms
{
    /// <summary>
    ///     Provides a static abstraction for FFT calculation.
    /// </summary>
    public static class Fft
    {
        /// <summary>
        ///     Gets or sets the FFT provider used for all FFT calculations. Defaults to a FftwProvider.
        /// </summary>
        public static IFftProvider FftProvider { get; set; }

        /// <summary>
        ///     Computes the FFT of complex-valued input data.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="n">The desired fft length. If set, the <paramref name="input" /> is zero-padded to <paramref name="n" />.</param>
        /// <returns>The FFT of the input.</returns>
        public static IReadOnlyList<Complex> ComplexFft(IEnumerable<Complex> input, int n = -1)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            IReadOnlyList<Complex> inputlist;

            if (n < 0)
            {
                inputlist = input.ToReadOnlyList();
                n = inputlist.Count;
            }
            else
                inputlist = input.ToReadOnlyList(n);

            if (n == 0)
                return Enumerable.Empty<Complex>().ToReadOnlyList();

            return FftProvider.ComplexFft(inputlist, n);
        }

        /// <summary>
        ///     Computes the IFFT of complex-valued input data.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <returns>The IFFT of the input.</returns>
        public static IReadOnlyList<Complex> ComplexIfft(IEnumerable<Complex> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var inputlist = input.ToReadOnlyList();

            if (inputlist.Count == 0)
                return Enumerable.Empty<Complex>().ToReadOnlyList();

            return FftProvider.ComplexIfft(inputlist);
        }

        /// <summary>
        ///     Calculates the even-spaced frequency points from 0 Hz (DC) to Nyquist frequency (SampleRate/2) representing the
        ///     positive part of the
        ///     frequency axis of a signal of length N with SampleRate in time domain.
        /// </summary>
        /// <param name="samplerate">The Samplerate of the signal in time domain</param>
        /// <param name="n">The Length of the signal in time domain</param>
        /// <returns>Array of frequencies</returns>
        public static IEnumerable<double> GetFrequencies(double samplerate, int n)
        {
            return SignalGenerators.LinSeries(0, samplerate * 0.5, (n >> 1) + 1);
        }

        /// <summary>
        ///     Gets the next biggest integer value that is a efficiently computable FFT length with the current FFT provider.
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <returns>The result.</returns>
        public static int GetOptimalFftLength(int input)
        {
            return FftProvider.GetOptimalFftLength(input);
        }

        /// <summary>
        ///     Computes the FFT over real-valued input data. Only the positive half of the hermitian symmetric fourier spectrum is
        ///     returned.
        /// </summary>
        /// <param name="input">The real-valued input data.</param>
        /// <param name="n">The desired fft length. If set, the <paramref name="input" /> is zero-padded to <paramref name="n" />.</param>
        /// <returns>The positive half of the hermitian-symmetric spectrum, including DC and Nyquist/2.</returns>
        public static IReadOnlyList<Complex> RealFft(IEnumerable<double> input, int n = -1)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            IReadOnlyList<double> inputlist;

            if (n < 0)
            {
                inputlist = input.ToReadOnlyList();
                n = inputlist.Count;
            }
            else
                inputlist = input.ToReadOnlyList(n);

            if (n == 0)
                return Enumerable.Empty<Complex>().ToReadOnlyList();

            return FftProvider.RealFft(inputlist, n);
        }

        /// <summary>
        ///     Computes the iFFT over the positive half of a hermitian-symmetric spectrum.
        /// </summary>
        /// <param name="input">The positive half of a hermitian-symmetric spectrum.</param>
        /// <returns>The computed time-domain values. Always has an even length.</returns>
        public static IReadOnlyList<double> RealIfft(IEnumerable<Complex> input, int n = -1)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var inputlist = input.ToReadOnlyList();

            if (inputlist.Count == 0)
                return Enumerable.Empty<double>().ToReadOnlyList();

            return FftProvider.RealIfft(inputlist, n);
        }
    }
}