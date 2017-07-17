// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiracFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace DspSharp.Filter.LtiFilters.Primitive
{
    /// <summary>
    ///     A filter with a transfer function of 1.
    /// </summary>
    public class DiracFilter : FiniteFilter
    {
        public DiracFilter(double samplerate) : base(samplerate)
        {
            this.Name = "Dirac Filter";
        }

        /// <summary>
        ///     Returns false.
        /// </summary>
        protected override bool HasEffectOverride => false;

        public override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return signal;
        }
    }
}