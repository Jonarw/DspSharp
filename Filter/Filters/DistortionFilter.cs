using System.Collections.Generic;
using System.Linq;

namespace Filter.Filters
{
    using System;

    /// <summary>
    /// Represents a filter with non-linear distortions, useful for testing other algorithms.
    /// </summary>
    /// <seealso cref="Filter.FilterBase" />
    public class DistortionFilter : FilterBase
    {
        /// <summary>
        /// Returns true.
        /// </summary>
        protected override bool HasEffectOverride => true;

        public override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return signal.Select(d => d + 0.2 * Math.Pow(d, 2) * Math.Sign(d) + 0.1 * Math.Pow(d, 3));
        }

        public DistortionFilter(double samplerate) : base(samplerate)
        {
        }
    }
}