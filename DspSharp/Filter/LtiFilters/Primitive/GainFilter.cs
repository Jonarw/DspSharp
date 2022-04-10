// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GainFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp.Algorithms;

namespace DspSharp.Filter.LtiFilters.Primitive
{
    /// <summary>
    /// Represents a filter with a constant gain.
    /// </summary>
    public class GainFilter : FiniteFilter
    {
        private double _Gain = 1;

        public GainFilter(double samplerate) : base(samplerate)
        {
            this.DisplayName = "Gain Filter";
        }

        /// <summary>
        /// Gets or sets the linear gain factor of the <see cref="GainFilter" />.
        /// </summary>
        public double Gain
        {
            get => this._Gain;
            set => this.SetField(ref this._Gain, value);
        }

        /// <inheritdoc/>
        protected override bool HasEffectOverride => this.Gain != 1;

        /// <inheritdoc/>
        protected override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return signal.Multiply(this.Gain);
        }
    }
}