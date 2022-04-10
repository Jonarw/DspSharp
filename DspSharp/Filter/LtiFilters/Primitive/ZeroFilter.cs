// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZeroFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Filter.LtiFilters.Primitive
{
    /// <summary>
    /// Represents a 'filter' which only returns zeros.
    /// </summary>
    public class ZeroFilter : FiniteFilter
    {
        public ZeroFilter(double samplerate) : base(samplerate)
        {
            this.DisplayName = "Zero Filter";
        }

        /// <inheritdoc/>
        protected override bool HasEffectOverride => true;

        /// <inheritdoc/>
        protected override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return signal.Select(_ => 0.0);
        }
    }
}