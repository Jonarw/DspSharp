using System.Collections.Generic;
using Filter.Algorithms;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    ///     Base class for all filters that can be described by their impulse response.
    /// </summary>
    /// <seealso cref="FilterBase" />
    public class Convolver : FilterBase
    {
        private IReadOnlyList<double> _impulseResponse;

        public Convolver(double samplerate) : base(samplerate)
        {
        }

        public virtual IReadOnlyList<double> ImpulseResponse
        {
            get { return this._impulseResponse; }
            set
            {
                this.SetField(ref this._impulseResponse, value);
                this.OnChange();
            }
        }

        protected override bool HasEffectOverride
        {
            get
            {
                if (this.ImpulseResponse == null)
                {
                    return false;
                }

                return true;
            }
        }

        public override IEnumerable<double> Process(IEnumerable<double> signal)
        {
            return Dsp.Convolve(signal, this.ImpulseResponse);
        }
    }
}