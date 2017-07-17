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
    ///     Represents a filter with non-linear distortions, useful for testing other algorithms.
    /// </summary>
    /// <seealso cref="FilterBase" />
    public class DistortionFilter : FiniteFilter
    {
        public DistortionFilter(double samplerate) : base(samplerate)
        {
        }

        /// <summary>
        ///     Specifies whether the filter object has an effect or not.
        /// </summary>
        protected override bool HasEffectOverride => true;

        /// <summary>
        ///     Processes the override.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns></returns>
        public override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return signal.Select(d => d < 0 ? -Math.Log(1 - d) : Math.Log(1 + Math.Sqrt(d))  );
        }
    }
}