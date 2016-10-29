using System.Collections.Generic;
using Filter.Extensions;
using Filter.Series;
using Filter.Signal;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    /// A filter with a transfer function of -1.
    /// </summary>
    public class InvertFilter : LtiFilterBase
    {
        /// <summary>
        /// Returns true.
        /// </summary>
        protected override bool HasEffectOverride => true;

        public override IEnumerable<double> Process(IEnumerable<double> signal)
        {
            return signal.Negate();
        }

        /// <summary>
        /// Gets the default impulse length.
        /// </summary>
        /// <returns>1</returns>
        public override int GetDefaultImpulseLength() => 1;
    }
}