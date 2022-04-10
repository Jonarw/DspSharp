// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DistortionFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Filter.NonlinearFilters
{
    /// <summary>
    /// Represents a filter with non-linear distortions, useful for testing other algorithms.
    /// </summary>
    /// <seealso cref="FilterBase" />
    public class DistortionFilter : FiniteFilter
    {
        public DistortionFilter(double samplerate) : base(samplerate)
        {
        }

        /// <inheritdoc/>
        protected override bool HasEffectOverride => true;

        /// <inheritdoc/>
        protected override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return signal.Select(d => d < 0 ? -Math.Log(1 - d) : Math.Log(1 + Math.Sqrt(d))  );
        }
    }
}