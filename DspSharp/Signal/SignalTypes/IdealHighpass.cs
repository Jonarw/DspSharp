// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdealHighpass.cs">
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
    ///     Represents an ideal sinc-based highpass.
    /// </summary>
    /// <seealso cref="SyntheticSignal" />
    public class IdealHighpass : SyntheticSignal
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="IdealHighpass" /> class.
        /// </summary>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="fc">The corner frequency.</param>
        /// <exception cref="System.Exception"></exception>
        public IdealHighpass(double sampleRate, double fc)
            : base(time => (time == 0 ? 1.0 : 0.0) - Mathematic.Sinc(2 * fc * time / sampleRate) * (2 * fc / sampleRate), sampleRate
                )
        {
            if ((fc < 0) || (fc > sampleRate / 2))
                throw new Exception();

            this.Fc = fc;
            var frequencies = new CustomSeries(new[] {0, 20, fc, fc, sampleRate / 2});
            this.Spectrum = new Spectrum.Spectrum(frequencies, new Complex[] {0, 0, 0, 1, 1});
            this.DisplayName = "ideal highpass, fc = " + fc;
        }

        /// <summary>
        ///     Gets the spectrum.
        /// </summary>
        public override ISpectrum Spectrum { get; }

        [Category("ideal highpass")]
        [DisplayName("cutoff frequency")]
        public double Fc { get; }
    }
}