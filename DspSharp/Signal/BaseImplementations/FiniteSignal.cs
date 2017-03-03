// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FiniteSignal.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DspSharp.Algorithms;
using DspSharp.Series;
using DspSharp.Spectrum;
using PropertyTools.DataAnnotations;

namespace DspSharp.Signal
{
    /// <summary>
    ///     Represents a digital signal that is representable in time domain with a finite number of time samples.
    /// </summary>
    /// <seealso cref="SignalBase" />
    /// <seealso cref="IFiniteSignal" />
    public class FiniteSignal : SignalBase, IFiniteSignal
    {
        private IReadOnlyList<double> _signal;
        private IFftSpectrum _spectrum;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FiniteSignal" /> class.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="start">The start sample time of the signal.</param>
        public FiniteSignal(IReadOnlyList<double> signal, double sampleRate, int start = 0) : base(sampleRate)
        {
            this.Signal = signal;
            this.Start = start;
            this.Length = signal.Count;
            this.Stop = start + signal.Count;
            this.Frequencies = new FftSeries(sampleRate, this.Length);
            this.DisplayName = "finite signal";
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FiniteSignal" /> class.
        /// </summary>
        /// <param name="spectrum">The FFT spectrum of the signal.</param>
        /// <param name="start">The start sample time of the signal.</param>
        public FiniteSignal(IFftSpectrum spectrum, int start = 0) : base(spectrum.Frequencies.SampleRate)
        {
            this.Spectrum = spectrum;
            this.Start = start;
            this.Length = spectrum.Frequencies.N;
            this.Stop = this.Start + this.Length;
            this.DisplayName = "finite signal";
        }

        /// <summary>
        ///     Gets the FFT frequencies.
        /// </summary>
        public FftSeries Frequencies { get; }

        /// <summary>
        ///     Gets or sets the minimum length of the FFT used to compute the signal's spectrum.
        /// </summary>
        public int MinFftLength { get; set; } = 128;

        /// <summary>
        ///     Computes the spectrum.
        /// </summary>
        /// <param name="fftLength">Length of the FFT used to compute the spectrum.</param>
        /// <returns>
        ///     The spectrum.
        /// </returns>
        public IFftSpectrum GetSpectrum(int fftLength)
        {
            return new FftSpectrum(this.Signal, fftLength, this.SampleRate, this.Start);
        }

        /// <summary>
        ///     Gets the signal.
        /// </summary>
        IEnumerable<double> IEnumerableSignal.Signal => this.Signal;

        /// <summary>
        ///     Gets the sample at the specified time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>
        ///     The sample.
        /// </returns>
        public double GetSample(int time)
        {
            if ((time < this.Start) || (time >= this.Stop))
                return 0.0;

            return this.Signal[time - this.Start];
        }

        /// <summary>
        ///     Gets the signal in time domain.
        /// </summary>
        public IReadOnlyList<double> Signal
        {
            get
            {
                if (this._signal == null)
                    this._signal = this.Spectrum.GetTimeDomainSignal();

                return this._signal;
            }

            private set { this._signal = value; }
        }

        /// <summary>
        ///     Gets the spectrum of the signal.
        /// </summary>
        public IFftSpectrum Spectrum
        {
            get
            {
                if (this._spectrum == null)
                {
                    this._spectrum = new FftSpectrum(
                        this.Signal,
                        Math.Max(this.Length, this.MinFftLength),
                        this.SampleRate,
                        this.Start);
                }

                return this._spectrum;
            }

            private set { this._spectrum = value; }
        }

        /// <summary>
        ///     Gets a section of the signal in time domain.
        /// </summary>
        /// <param name="start">The start of the section.</param>
        /// <param name="length">The length of the section.</param>
        /// <returns>
        ///     The specified section.
        /// </returns>
        public override IEnumerable<double> GetWindowedSamples(int start, int length)
        {
            return this.Signal.GetPaddedRange(start - this.Start, length);
        }

        [Category("finite signal")]
        [DisplayName("start time")]
        public int Start { get; }

        [DisplayName("length")]
        public int Length { get; }

        [DisplayName("end time")]
        public int Stop { get; }
    }
}