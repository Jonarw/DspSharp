using System.Collections.Generic;

namespace Filter.LtiFilters
{
    /// <summary>
    ///     A filter with a transfer function of 1.
    /// </summary>
    public class DiracFilter : FilterBase
    {
        public DiracFilter(double samplerate) : base(samplerate)
        {
            this.Name = "dirac filter";
        }

        /// <summary>
        ///     Returns false.
        /// </summary>
        protected override bool HasEffectOverride => false;

        public override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return signal;
        }
    }
}