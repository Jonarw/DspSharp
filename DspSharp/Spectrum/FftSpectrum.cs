// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FftSpectrum.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Numerics;
using DspSharp.Algorithms;
using DspSharp.Series;

namespace DspSharp.Spectrum
{
    /// <summary>
    ///     Represents the FFT spectrum of a finite digital signal.
    /// </summary>
    /// <seealso cref="Spectrum" />
    /// <seealso cref="IFftSpectrum" />
    public class FftSpectrum : Spectrum, IFftSpectrum
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FftSpectrum" /> class.
        /// </summary>
        /// <param name="frequencies">The frequencies.</param>
        /// <param name="values">The values.</param>
        public FftSpectrum(FftSeries frequencies, IReadOnlyList<Complex> values) : base(frequencies, values)
        {
            this.Frequencies = frequencies;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FftSpectrum" /> class.
        /// </summary>
        /// <param name="timeSignal">The time domain signal.</param>
        /// <param name="fftLength">Length of the FFT.</param>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="start">The start sample time of the signal.</param>
        public FftSpectrum(IEnumerable<double> timeSignal, int fftLength, double sampleRate, int start = 0)
            : this(
                new FftSeries(sampleRate, fftLength),
                Fft.RealFft(timeSignal.ToReadOnlyList().CircularShift(start), fftLength))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FftSpectrum" /> class.
        /// </summary>
        /// <param name="timeSignal">The finite time domain signal.</param>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="start">The start.</param>
        public FftSpectrum(IReadOnlyList<double> timeSignal, double sampleRate, int start = 0)
            : this(new FftSeries(sampleRate, timeSignal.Count), Fft.RealFft(timeSignal.CircularShift(start)))
        {
            this.TimeDomainSignal = timeSignal.CircularShift(start).ToReadOnlyList();
        }

        private IReadOnlyList<double> TimeDomainSignal { get; set; }

        /// <summary>
        ///     Gets the frequencies where the spectrum is defined.
        /// </summary>
        public new FftSeries Frequencies { get; }

        /// <summary>
        ///     Gets the time domain signal.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<double> GetTimeDomainSignal()
        {
            return this.TimeDomainSignal ?? (this.TimeDomainSignal = Fft.RealIfft(this.Values));
        }
    }
}