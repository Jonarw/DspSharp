using System.Collections.Generic;
using Filter.Extensions;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    ///     Represents a filter with a constant gain and no effects otherwise.
    /// </summary>
    public class GainFilter : FilterBase
    {
        private double _Gain = 1;

        public GainFilter(double samplerate) : base(samplerate)
        {
        }

        /// <summary>
        ///     Gets or sets the linear gain factor of the <see cref="GainFilter" />.
        /// </summary>
        public double Gain
        {
            get { return this._Gain; }
            set { this.SetField(ref this._Gain, value); }
        }

        /// <summary>
        ///     True if <see cref="Gain" /> is not 1, false otherwise.
        /// </summary>
        protected override bool HasEffectOverride
        {
            get
            {
                if (this.Gain == 1)
                {
                    return false;
                }

                return true;
            }
        }

        public override IEnumerable<double> Process(IEnumerable<double> signal)
        {
            return signal.Multiply(this.Gain);
        }
    }
}