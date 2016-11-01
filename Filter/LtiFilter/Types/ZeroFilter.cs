using System.Collections.Generic;
using System.Linq;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    ///     Represents a filter with a constant gain and no effects otherwise.
    /// </summary>
    public class ZeroFilter : FilterBase
    {
        public ZeroFilter(double samplerate) : base(samplerate)
        {
        }

        /// <summary>
        ///     Returns true.
        /// </summary>
        protected override bool HasEffectOverride => true;

        public override IEnumerable<double> Process(IEnumerable<double> signal)
        {
            return Enumerable.Repeat(0.0, signal.Count());
        }
    }
}