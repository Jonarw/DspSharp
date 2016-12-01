using System.Collections.Generic;

namespace Filter.LtiFilters
{
    /// <summary>
    ///     Represents a filter with a constant gain and no effects otherwise.
    /// </summary>
    public class ZeroFilter : FilterBase
    {
        public ZeroFilter(double samplerate) : base(samplerate)
        {
            this.Name = "zero filter";
        }

        /// <summary>
        ///     Returns true.
        /// </summary>
        protected override bool HasEffectOverride => true;

        public override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            var e = signal.GetEnumerator();
            while (e.MoveNext())
            {
                yield return 0.0;
            }
        }
    }
}