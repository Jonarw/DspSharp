using System;
using System.Collections.Generic;
using System.Numerics;
using Filter.Algorithms;
using Filter.Extensions;
using Filter.Series;

namespace Filter.Spectrum
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
            {
                throw new Exception();
            }

            this.Frequencies = frequencies;
            this.Values = values;
        }

        /// <summary>
        ///     Gets the frequencies where the spectrum is defined.
        /// </summary>
        public ISeries Frequencies { get; }

        /// <summary>
        ///     Gets the group delay.
        /// </summary>
        public IReadOnlyList<double> GroupDelay
        {
            get { return this._groupDelay ?? (this._groupDelay = Dsp.CalculateGroupDelay(this.Frequencies.Values, this.Phase).ToReadOnlyList()); }
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