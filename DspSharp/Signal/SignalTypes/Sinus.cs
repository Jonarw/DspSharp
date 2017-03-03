// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sinus.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Numerics;
using DspSharp.Series;
using DspSharp.Spectrum;
using PropertyTools.DataAnnotations;

namespace DspSharp.Signal
{
    /// <summary>
    ///     Represents a sine wave.
    /// </summary>
    /// <seealso cref="SyntheticSignal" />
    public class Sinus : SyntheticSignal
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Sinus" /> class.
        /// </summary>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="frequency">The frequency.</param>
        /// <param name="phaseOffset">The phase offset in radians.</param>
        /// <exception cref="System.Exception"></exception>
        public Sinus(double sampleRate, double frequency, double phaseOffset = 0)
            : base(time => Math.Sin(2 * Math.PI * time * frequency / sampleRate + phaseOffset), sampleRate)
        {
            this.Frequency = frequency;
            this.PhaseOffset = phaseOffset;
            if ((frequency < 0) || (frequency > sampleRate / 2))
                throw new Exception();

            var frequencies = new CustomSeries(new[] {0, frequency, frequency, frequency, sampleRate / 2});
            this.Spectrum = new Spectrum.Spectrum(frequencies, new Complex[] {0, 0, double.PositiveInfinity, 0, 0});
            this.DisplayName = "sinus, f = " + frequency;
        }

        /// <summary>
        ///     Gets the spectrum.
        /// </summary>
        public override ISpectrum Spectrum { get; }

        [Category("sinus")]
        [DisplayName("frequency")]
        public double Frequency { get; }

        [DisplayName("phase offset")]
        public double PhaseOffset { get; }
    }
}