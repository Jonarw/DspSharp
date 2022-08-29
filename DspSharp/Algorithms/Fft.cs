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
    /// Provides a static abstraction for FFT calculation.
    /// </summary>
    public static class Fft
    {
        /// <summary>
        /// Gets or sets the FFT provider used for all FFT calculations.
        /// </summary>
        public static IFftProvider FftProvider { get; set; }

        /// <summary>
        /// Computes the FFT of complex-valued input data.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="n">The desired fft length. If set, the <paramref name="input" /> is zero-padded to <paramref name="n" />.</param>
        /// <returns>The FFT of the input.</returns>
        public static Complex[] ComplexFft(IReadOnlyCollection<Complex> input, int n = -1)
        {
            if (n == 0)
                return Array.Empty<Complex>();

            var fftList = n < 0
                ? input
                : input.PadToLength(n);

            return FftProvider.ComplexFft(fftList.ToList());
        }

        /// <summary>
        /// Computes the IFFT of complex-valued input data.
        /// </summary>
        /// <param name="input">The input data.</param>
        public static Complex[] ComplexIfft(IReadOnlyCollection<Complex> input)
        {
            if (input.Count == 0)
                return Array.Empty<Complex>();

            return FftProvider.ComplexIfft(input.ToList());
        }

        /// <summary>
        /// Calculates the even-spaced frequency points from 0 Hz (DC) to Nyquist frequency (SampleRate/2) representing the
        /// positive part of the frequency axis of a signal of length N with SampleRate in time domain.
        /// </summary>
        /// <param name="samplerate">The Samplerate of the signal in time domain</param>
        /// <param name="n">The Length of the signal in time domain</param>
        public static IReadOnlyCollection<double> GetFrequencies(double samplerate, int n)
        {
            return SignalGenerators.LinSeries(0, samplerate * 0.5, (n >> 1) + 1);
        }

        /// <summary>
        /// Computes the FFT over real-valued input data. Only the positive half of the hermitian symmetric fourier spectrum is returned.
        /// </summary>
        /// <param name="input">The real-valued input data.</param>
        /// <param name="n">The desired fft length. If set, the <paramref name="input" /> is zero-padded to <paramref name="n" />.</param>
        /// <returns>The positive half of the hermitian-symmetric spectrum, including DC and Nyquist/2.</returns>
        public static Complex[] RealFft(IReadOnlyCollection<double> input, int n = -1)
        {
            if (n == 0)
                return Array.Empty<Complex>();

            var fftList = n < 0
                ? input
                : input.PadToLength(n);

            return FftProvider.RealFft(fftList.ToList());
        }

        /// <summary>
        /// Computes the iFFT over the positive half of a hermitian-symmetric spectrum.
        /// </summary>
        /// <param name="input">The positive half of a hermitian-symmetric spectrum.</param>
        /// <param name="isEven">A value indicating whether the time domain signal corresponding to the spectrum is even-length or not.</param>
        public static double[] RealIfft(IReadOnlyCollection<Complex> input, bool isEven)
        {
            if (input.Count == 0)
                return Array.Empty<double>();

            return FftProvider.RealIfft(input.ToList(), isEven);
        }

        /// <summary>
        /// Computes the iFFT over the positive half of a hermitian-symmetric spectrum.
        /// </summary>
        /// <param name="input">The positive half of a hermitian-symmetric spectrum.</param>
        /// <param name="timeDomainLength">The length of the signal in time domain.</param>
        public static double[] RealIfft(IReadOnlyCollection<Complex> input, int timeDomainLength)
        {
            var expectedLength = (input.Count - 1) * 2;
            if (timeDomainLength < expectedLength || timeDomainLength > expectedLength + 1)
                throw new ArgumentOutOfRangeException(nameof(timeDomainLength));

            return RealIfft(input, timeDomainLength % 2 == 0);
        }
    }
}