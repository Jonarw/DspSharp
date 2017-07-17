using System;
using System.Collections.Generic;

namespace DspSharp.Filter.LtiFilters.Iir
{
    public enum ButterworthFilterType
    {
        Lowpass,
        Highpass
    }

    /// <summary>
    ///     Represents a Butterworth filter of arbitrary order.
    /// </summary>
    /// <seealso cref="FilterBase" />
    /// <remarks>https://en.wikipedia.org/wiki/Butterworth_filter</remarks>
    public class ButterworthFilter : FilterBase
    {
        private double _Fc;

        private int _FilterOrder;

        private ButterworthFilterType _FilterType;

        private IFilter internalFilter;

        public ButterworthFilter(double samplerate) : base(samplerate)
        {
        }

        public ButterworthFilter(double samplerate, ButterworthFilterType filterType, int filterOrder, double fc) : base(samplerate)
        {
            this._FilterType = filterType;
            this._FilterOrder = filterOrder;
            this._Fc = fc;
            this.UpdateInternalFilter();
        }

        public double Fc
        {
            get => this._Fc;
            set => this.SetField(ref this._Fc, value);
        }

        public int FilterOrder
        {
            get => this._FilterOrder;
            set
            {
                this.SetField(ref this._FilterOrder, value);
                this.UpdateInternalFilter();
            }
        }

        public ButterworthFilterType FilterType
        {
            get => this._FilterType;
            set => this.SetField(ref this._FilterType, value);
        }

        protected override bool HasEffectOverride => true;

        public override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
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
            for (int k = 1; k <= this.FilterOrder / 2; k++)
            {
                var q = 1d / (-2 * Math.Cos((2d * k + n - 1) / (2 * n) * Math.PI));
                set.Filters.Add(
                    new BiquadFilter(
                        this.Samplerate,
                        this.FilterType == ButterworthFilterType.Highpass ? BiquadFilter.BiquadFilters.Highpass : BiquadFilter.BiquadFilters.Lowpass,
                        this.Fc,
                        q));
            }

            this.internalFilter = set;
        }
    }
}