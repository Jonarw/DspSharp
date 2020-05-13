// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using DspSharp.Algorithms;

namespace DspSharp.Filter.LtiFilters.Primitive
{
    /// <summary>
    ///     Represents a filter with a constant group delay and no effects otherwise.
    /// </summary>
    public class DelayFilter : FiniteFilter
    {
        private double _delay;
        private int _SampleDelay;

        public DelayFilter(double samplerate) : base(samplerate)
        {
            this.DisplayName = "delay filter";
        }

        /// <summary>
        ///     Gets the delay of the <see cref="DelayFilter" /> in seconds.
        /// </summary>
        public double Delay
        {
            get { return this._delay; }
            private set { this.SetField(ref this._delay, value); }
        }

        /// <summary>
        ///     Gets or sets the delay of the <see cref="DelayFilter" /> in integer samples.
        /// </summary>
        public int SampleDelay
        {
            get { return Convert.ToInt32(this.Delay * this.Samplerate); }
            set
            {
                if (!this.SetField(ref this._SampleDelay, value))
                    return;

                this.Delay = value / this.Samplerate;
                this.RaiseChangedEvent();
            }
        }

        /// <summary>
        ///     True if <see cref="Delay" /> is not 0, false otherwise.
        /// </summary>
        protected override bool HasEffectOverride
        {
            get
            {
                if (this.SampleDelay == 0)
                    return false;
                return true;
            }
        }

        public override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return SignalGenerators.GetZeros(this.SampleDelay).Concat(signal);
        }
    }
}