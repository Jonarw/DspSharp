using System.Collections.Generic;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    ///     A filter with a transfer function of 1.
    /// </summary>
    public class DiracFilter : FilterBase
    {
        public DiracFilter(double samplerate) : base(samplerate)
        {
        }

        /// <summary>
        ///     Returns false.
        /// </summary>
        protected override bool HasEffectOverride => false;

        public override IEnumerable<double> Process(IEnumerable<double> signal)
        {
            return signal;
        }
    }
}