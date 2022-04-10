// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ButterworthFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DspSharp.Filter.LtiFilters.Iir
{

    /// <summary>
    /// Represents a Butterworth filter of arbitrary order.
    /// </summary>
    /// <remarks>https://en.wikipedia.org/wiki/Butterworth_filter</remarks>
    public class ButterworthFilter : FilterBase
    {
        private double _Fc;
        private int _FilterOrder;
        private ButterworthFilterType _FilterType;
        private IFilter internalFilter;

        public ButterworthFilter(double samplerate) : this(samplerate, ButterworthFilterType.Highpass, 2, 1000)
        {
        }

        public ButterworthFilter(double samplerate, ButterworthFilterType filterType, int filterOrder, double fc) : base(samplerate)
        {
            this._FilterType = filterType;
            this._FilterOrder = filterOrder;
            this._Fc = fc;
        }

        /// <summary>
        /// Gets or sets the cutoff frequency.
        /// </summary>
        public double Fc
        {
            get => this._Fc;
            set
            {
                this.internalFilter = null;
                this.SetField(ref this._Fc, value);
            }
        }

        /// <summary>
        /// Gets or sets the filter order.
        /// </summary>
        public int FilterOrder
        {
            get => this._FilterOrder;
            set
            {
                this.internalFilter = null;
                this.SetField(ref this._FilterOrder, value);
            }
        }

        /// <summary>
        /// Gets or sets the type of filter.
        /// </summary>
        public ButterworthFilterType FilterType
        {
            get => this._FilterType;
            set
            {
                this.internalFilter = null;
                this.SetField(ref this._FilterType, value);
            }
        }

        protected override bool HasEffectOverride => true;

        protected override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            if (this.internalFilter == null)
                this.UpdateInternalFilter();

            return this.internalFilter.Process(signal);
        }

        private void UpdateInternalFilter()
        {
            var set = new FilterSet(this.Samplerate);

            if (this.FilterOrder % 2 != 0)
            {
                var a = new double[2];
                var b = new double[2];

                var w0 = 2 * Math.PI * this.Fc / this.Samplerate;
                a[0] = Math.Cos(w0) - Math.Sin(w0) - 1;
                a[1] = Math.Cos(w0) + Math.Sin(w0) - 1;

                if (this.FilterType == ButterworthFilterType.Lowpass)
                {
                    b[0] = Math.Cos(w0) - 1;
                    b[1] = Math.Cos(w0) - 1;
                }
                else if (this.FilterType == ButterworthFilterType.Highpass)
                {
                    b[0] = -Math.Sin(w0);
                    b[1] = Math.Sin(w0);
                }

                set.Filters.Add(new CustomIirFilter(this.Samplerate, a, b));
            }

            var n = this.FilterOrder;
            for (var k = 1; k <= this.FilterOrder / 2; k++)
            {
                var q = 1d / (-2 * Math.Cos((2d * k + n - 1) / (2 * n) * Math.PI));
                set.Filters.Add(
                    new BiquadFilter(
                        this.Samplerate,
                        this.FilterType == ButterworthFilterType.Highpass ? BiquadFilterType.Highpass : BiquadFilterType.Lowpass,
                        this.Fc,
                        q));
            }

            this.internalFilter = set;
        }
    }
}