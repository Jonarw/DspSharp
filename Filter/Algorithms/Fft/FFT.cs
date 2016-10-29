using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Filter.Extensions;

namespace Filter.Algorithms
{
    /// <summary>
    ///     Provides an easy-to-use static abstraction for the real-valued 1d transformation functions of the FFTW library. FFT
    ///     plans are automatically created and
    ///     destroyed. The accumulated wisdom is automatically saved and loaded.
    /// </summary>
    public sealed class Fft
    {
        public static IFftProvider FftProvider { get; set; }

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
            return Dsp.LinSeries(0, samplerate * 0.5, (n >> 1) + 1);
        }

        /// <summary>
        ///     Computes the next biggest power of 2 for a given input value.
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <returns>The result.</returns>
        public static int NextPowerOfTwo(int input)
        {
            return Convert.ToInt32(Math.Pow(2, Math.Ceiling(Math.Log(input, 2))));
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
            if (n == -1)
            {
                input = input.ToReadOnlyList();
                n = input.Count();
            }

            return FftProvider.RealFft(input.ToReadOnlyList(n), n);
        }

        /// <summary>
        ///     Computes the iFFT over the positive half of a hermitian-symmetric spectrum.
        /// </summary>
        /// <param name="input">The positive half of a hermitian-symmetric spectrum.</param>
        /// <returns>The computed time-domain values. Always has an even length.</returns>
        public static IReadOnlyList<double> RealIfft(IEnumerable<Complex> input)
        {
            return FftProvider.RealIfft(input.ToReadOnlyList());
        }

        /// <summary>
        ///     Computes an oversampled FFT over real-valued input data. Only the positive half of the hermitian symmetric fourier
        ///     spectrum is
        ///     returned.
        /// </summary>
        /// <param name="input">The real-valued input data.</param>
        /// <param name="oversampling">The oversampling factor</param>
        /// <param name="n">The desired fft length. If set, the <paramref name="input" /> is zero-padded to <paramref name="n" />.</param>
        /// <returns>The positive half of the hermitian-symmetric spectrum, including DC and Nyquist/2.</returns>
        public static IEnumerable<Complex> RealOversampledFft(IEnumerable<double> input, int oversampling, int n = -1)
        {
            var inputlist = input.ToReadOnlyList();

            if (n < 0)
            {
                n = inputlist.Count;
            }

            n *= oversampling;
            var fft = FftProvider.RealFft(inputlist, n);
            return fft.SparseSeries(oversampling);
        }
    }
}