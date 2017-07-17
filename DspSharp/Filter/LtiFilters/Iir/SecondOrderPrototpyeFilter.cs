using System;
using System.Collections.Generic;
using System.Numerics;

namespace DspSharp.Filter.LtiFilters.Iir
{
    public class SecondOrderPrototpyeFilter : FilterBase
    {
        private double _Fc;
        private ButterworthFilterType _FilterType;
        private double _K1;
        private double _K2;

        public SecondOrderPrototpyeFilter(double samplerate) : base(samplerate)
        {
        }

        public double Fc
        {
            get => this._Fc;
            set => this.SetField(ref this._Fc, value);
        }

        public ButterworthFilterType FilterType
        {
            get => this._FilterType;
            set => this.SetField(ref this._FilterType, value);
        }

        public double K1
        {
            get => this._K1;
            private set => this.SetField(ref this._K1, value);
        }

        public double K2
        {
            get => this._K2;
            private set => this.SetField(ref this._K2, value);
        }

        private IFilter InternalFilter { get; set; }

        public override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return this.InternalFilter.Process(signal);
        }

        public void SetPrototypeCoefficients(double k1, double k2)
        {
            this.K1 = k1;
            this.K2 = k2;
        }

        public void SetPrototypePoles(double p1, double p2)
        {
            this.K1 = -p1 - p2;
            this.K2 = p1 * p2;
        }

        public void SetPrototypePoles(Complex p)
        {
            this.K1 = (-p - Complex.Conjugate(p)).Real;
            this.K2 = (p * Complex.Conjugate(p)).Real;
        }

        private void UpdateFilter()
        {
            var a = new double[3];
            var b = new double[3];

            var w0 = 2 * Math.PI * this.Fc / this.Samplerate;

            a[0] = -this.K1 * Math.Sin(w0) + this.K2 * Math.Cos(w0) - this.K2 - Math.Cos(w0) - 1;
            a[1] = 2 * this.K2 * Math.Cos(w0) - 2 * this.K2 + 2 * Math.Cos(w0) + 1;
            a[2] = this.K1 * Math.Sin(w0) + this.K2 * Math.Cos(w0) - this.K2 - Math.Cos(w0) - 1;

            if (this.FilterType == ButterworthFilterType.Lowpass)
            {
                b[0] = Math.Cos(w0) - 1;
                b[1] = 2 * Math.Cos(w0) - 2;
                b[2] = Math.Cos(w0) - 1;
            }
            else if (this.FilterType == ButterworthFilterType.Highpass)
            {
                b[0] = -Math.Cos(w0) - 1;
                b[1] = -2 * Math.Cos(w0) - 2;
                b[2] = -Math.Cos(w0) - 1;
            }

            this.InternalFilter = new CustomIirFilter(this.Samplerate, a, b);
        }
    }
}