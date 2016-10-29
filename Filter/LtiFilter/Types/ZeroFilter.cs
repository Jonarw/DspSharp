using System;
using System.Collections.Generic;
using System.Linq;
using Filter.Series;
using Filter.Signal;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    /// Represents a filter with a constant gain and no effects otherwise.
    /// </summary>
    public class ZeroFilter : LtiFilterBase
    {
        /// <summary>
        /// Returns true.
        /// </summary>
        protected override bool HasEffectOverride => true;

        public override IEnumerable<double> Process(IEnumerable<double> signal)
        {
            return Enumerable.Repeat(0.0, signal.Count());
        }
        /// <summary>
        /// Gets the default length for the impulse response of the <see cref="LtiFilterBase" />.
        /// </summary>
        /// <returns>1</returns>
        public override int GetDefaultImpulseLength() => 1;
    }
}