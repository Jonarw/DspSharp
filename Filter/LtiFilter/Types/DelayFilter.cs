using System;
using System.Collections.Generic;
using System.Linq;
using Filter.Extensions;
using Filter.Signal;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    ///     Represents a filter with a constant group delay and no effects otherwise.
    /// </summary>
    public class SampleDelayFilter : LtiFilterBase
    {
        private double _delay;
        private int _SampleDelay;

        /// <summary>
        ///     True if <see cref="Delay" /> is not 0, false otherwise.
        /// </summary>
        protected override bool HasEffectOverride
        {
            get
            {
                if (this.SampleDelay == 0)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        ///     Gets the delay of the <see cref="SampleDelayFilter" /> in seconds.
        /// </summary>
        public double Delay
        {
            get { return this._delay; }
            private set { this.SetField(ref this._delay, value); }
        }

        /// <summary>
        ///     Gets or sets the delay of the <see cref="SampleDelayFilter" /> in integer samples.
        /// </summary>
        public int SampleDelay
        {
            get { return Convert.ToInt32(this.Delay * this.Samplerate); }
            set
            {
                if (!this.SetField(ref this._SampleDelay, value))
                {
                    return;
                }

                this.Delay = value / this.Samplerate;
                this.OnChange();
            }
        }

        /// <summary>
        ///     Gets the default length for the impulse response.
        /// </summary>
        /// <returns>
        ///     The default impulse length.
        /// </returns>
        public override int GetDefaultImpulseLength()
        {
            return this.SampleDelay + 1;
        }

        public override IEnumerable<double> Process(IEnumerable<double> signal)
        {
            return Enumerable.Repeat(0.0, this.SampleDelay).Concat(signal);
        }
    }
}