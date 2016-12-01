using System.Collections.Generic;
using Filter.Extensions;

namespace Filter.LtiFilters
{
    /// <summary>
    ///     A filter with a transfer function of -1.
    /// </summary>
    public class InvertFilter : FilterBase
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