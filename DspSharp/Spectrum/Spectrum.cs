// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Spectrum.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Numerics;
using DspSharp.Algorithms;
using DspSharp.Series;

namespace DspSharp.Spectrum
{
    /// <summary>
    ///     Represents the spectrum of a digital signal.
    /// </summary>
    public class Spectrum : ISpectrum
    {
        private IReadOnlyList<double> _groupDelay;
        private IReadOnlyList<double> _magnitude;
        private IReadOnlyList<double> _phase;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Spectrum" /> class.
        /// </summary>
        /// <param name="frequencies">The frequencies.</param>
        /// <param name="values">The values.</param>
        /// <exception cref="System.Exception"></exception>
        public Spectrum(ISeries frequencies, IReadOnlyList<Complex> values)
        {
            if (frequencies.Length != values.Count)
                throw new Exception();

            this.Frequencies = frequencies;
            this.Values = values;
            this.Length = this.Frequencies.Length;
        }

        public int Length { get; }

        /// <summary>
        ///     Gets the frequencies where the spectrum is defined.
        /// </summary>
        public ISeries Frequencies { get; }

        /// <summary>
        ///     Gets the value at the specified frequency.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        /// <returns></returns>
        public Complex GetValue(double frequency)
        {
            if (frequency < this.Frequencies.Values[0])
                return this.Values[0];

            if (frequency > this.Frequencies.Values[this.Length - 1])
                return this.Values[this.Length - 1];

            int c = 0;
            while (this.Frequencies.Values[c] < frequency)
            {
                c++;
            }

            if (this.Frequencies.Values[c].Equals(frequency))
                return this.Values[c];

            var dif = this.Frequencies.Values[c] - this.Frequencies.Values[c - 1];
            var d1 = (frequency - this.Frequencies.Values[c - 1]) / dif;
            var d2 = (this.Frequencies.Values[c] - frequency) / dif;

            return d2 * this.Values[c - 1] + d1 * this.Values[c];
        }

        /// <summary>
        ///     Gets the group delay.
        /// </summary>
        public IReadOnlyList<double> GroupDelay
        {
            get
            {
                return this._groupDelay ??
                       (this._groupDelay =
                           FrequencyDomain.CalculateGroupDelay(this.Frequencies.Values, this.Phase).ToReadOnlyList());
            }
        }

        /// <summary>
        ///     Gets the magnitude.
        /// </summary>
        public IReadOnlyList<double> Magnitude
        {
            get { return this._magnitude ?? (this._magnitude = this.Values.Magitude().ToReadOnlyList()); }
        }

        /// <summary>
        ///     Gets the phase.
        /// </summary>
        public IReadOnlyList<double> Phase
        {
            get { return this._phase ?? (this._phase = this.Values.Phase().ToReadOnlyList()); }
        }

        /// <summary>
        ///     Gets the complex values.
        /// </summary>
        public IReadOnlyList<Complex> Values { get; }
    }
}