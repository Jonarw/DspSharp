// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvertFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp.Algorithms;

namespace DspSharp.Filter.LtiFilters.Primitive
{
    /// <summary>
    ///     A filter with a transfer function of -1.
    /// </summary>
    public class InvertFilter : FiniteFilter
    {
        public InvertFilter(double samplerate) : base(samplerate)
        {
            this.Name = "invert filter";
        }

        /// <summary>
        ///     Returns true.
        /// </summary>
        protected override bool HasEffectOverride => true;

        public override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return signal.Negate();
        }
    }
}