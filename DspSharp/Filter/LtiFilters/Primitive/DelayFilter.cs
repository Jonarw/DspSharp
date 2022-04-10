// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Filter.LtiFilters.Primitive
{
    /// <summary>
    /// Represents a filter with a constant group delay and no effects otherwise.
    /// </summary>
    public class DelayFilter : FiniteFilter
    {
        private double _Delay;
        private int _SampleDelay;

        public DelayFilter(double samplerate) : base(samplerate)
        {
            this.DisplayName = "delay filter";
        }

        /// <summary>
        /// Gets the delay of the <see cref="DelayFilter" /> in seconds.
        /// </summary>
        public double Delay
        {
            get => this._Delay;
            private set => this.SetField(ref this._Delay, value);
        }

        /// <summary>
        /// Gets or sets the delay of the <see cref="DelayFilter" /> in integer samples.
        /// </summary>
        public int SampleDelay
        {
            get => Convert.ToInt32(this.Delay * this.Samplerate);
            set => this.SetField(ref this._SampleDelay, value, this.CoerceSampleDelay);
        }

        /// <inheritdoc/>
        protected override bool HasEffectOverride => this.SampleDelay != 0;

        /// <inheritdoc/>
        protected override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return SignalGenerators.GetZeros(this.SampleDelay).Concat(signal);
        }

        private void CoerceSampleDelay()
        {
            this.Delay = this.Samplerate / this.Samplerate;
            this.RaiseChangedEvent();
        }
    }
}