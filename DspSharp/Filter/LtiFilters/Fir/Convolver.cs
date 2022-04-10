// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Convolver.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp.Algorithms;

namespace DspSharp.Filter.LtiFilters.Fir
{
    /// <summary>
    /// Base class for all filters that can be described by their impulse response.
    /// </summary>
    public abstract class Convolver : FiniteFilter
    {
        protected Convolver(double samplerate) : base(samplerate)
        {
            this.DisplayName = "Convolver";
        }

        public abstract IReadOnlyList<double> ImpulseResponse { get; }

        /// <inheritdoc/>
        protected override bool HasEffectOverride => this.ImpulseResponse != null;

        /// <inheritdoc/>
        protected override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return TimeDomain.Convolve(signal, this.ImpulseResponse);
        }
    }
}