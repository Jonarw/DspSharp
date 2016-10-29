using System.Collections.Generic;
using Filter.Series;
using Filter.Signal;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    /// A filter with a transfer function of 1.
    /// </summary>
    public class UnityFilter : LtiFilterBase
    {
        /// <summary>
        /// Returns false.
        /// </summary>
        protected override bool HasEffectOverride => false;

        public override IEnumerable<double> Process(IEnumerable<double> signal)
        {
            return signal;
        }

        /// <summary>
        /// Gets the default impulse length.
        /// </summary>
        /// <returns>1</returns>
        public override int GetDefaultImpulseLength() => 1;
    }
}