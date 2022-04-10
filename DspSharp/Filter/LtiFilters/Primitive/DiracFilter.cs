// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiracFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace DspSharp.Filter.LtiFilters.Primitive
{
    /// <summary>
    /// A 'filter' which does nothing.
    /// </summary>
    public class DiracFilter : FiniteFilter
    {
        public DiracFilter(double samplerate) : base(samplerate)
        {
            this.DisplayName = "Dirac Filter";
        }

        /// <inheritdoc/>
        protected override bool HasEffectOverride => false;

        /// <inheritdoc/>
        protected override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return signal;
        }
    }
}