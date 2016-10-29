using System.Collections.Generic;
using System.Numerics;
using Filter.Extensions;
using Filter.Series;
using Filter.Signal;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    /// Represents a filter with a constant gain and no effects otherwise.
    /// </summary>
    public class GainFilter : LtiFilterBase
    {
        private double _Gain = 1;

        /// <summary>
        /// Gets or sets the linear gain factor of the <see cref="GainFilter"/>.
        /// </summary>
        public double Gain
        {
            get { return this._Gain; }
            set { this.SetField(ref this._Gain, value); }
        }

        /// <summary>
        /// True if <see cref="Gain"/> is not 1, false otherwise.
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

        /// <summary>
        /// Gets the default length for the impulse response of the <see cref="LtiFilterBase" />.
        /// </summary>
        /// <returns>1</returns>
        public override int GetDefaultImpulseLength() => 1;

    }
}