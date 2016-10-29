using System;
using System.Collections.Generic;
using System.Linq;
using Filter.Algorithms;
using Filter.Extensions;
using Filter.Series;
using Filter.Signal;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    /// Base class for all filters that can be described by their impulse response.
    /// </summary>
    /// <seealso cref="LtiFilterBase" />
    public class Convolver : LtiFilterBase
    {
        private IReadOnlyList<double> _impulseResponse;

        public virtual IReadOnlyList<double> ImpulseResponse
        {
            get { return this._impulseResponse; }
            set
            {
                this.SetField(ref this._impulseResponse, value); 
                this.OnChange();
            }
        }

        public override int GetDefaultImpulseLength()
        {
            return this.ImpulseResponse.Count;
        }

        public override IEnumerable<double> Process(IEnumerable<double> signal)
        {
            return Dsp.Convolve(signal, this.ImpulseResponse);
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
    }
}