// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdealLowpass.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Numerics;
using DspSharp.Algorithms;
using DspSharp.Series;
using DspSharp.Spectrum;
using PropertyTools.DataAnnotations;

namespace DspSharp.Signal
{
    /// <summary>
    ///     Represents an ideal sinc-based Lowpass.
    /// </summary>
    /// <seealso cref="SyntheticSignal" />
    public class IdealLowpass : SyntheticSignal
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="IdealLowpass" /> class.
        /// </summary>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="fc">The corner frequency.</param>
        /// <exception cref="System.Exception"></exception>
        public IdealLowpass(double sampleRate, double fc)
            : base(time => Mathematic.Sinc(2 * fc * time / sampleRate) * (2 * fc / sampleRate), sampleRate)
        {
            if ((fc < 0) || (fc > sampleRate / 2))
                throw new Exception();

            this.Fc = fc;
            var frequencies = new CustomSeries(new[] {0, 20, fc, fc, sampleRate / 2});
            this.Spectrum = new Spectrum.Spectrum(frequencies, new Complex[] {1, 1, 1, 0, 0});
            this.DisplayName = "ideal lowpass, fc = " + fc;
        }

        /// <summary>
        ///     Gets the spectrum.
        /// </summary>
        public override ISpectrum Spectrum { get; }

        [Category("ideal lowpass")]
        [DisplayName("cutoff frequency")]
        public double Fc { get; }
    }
}