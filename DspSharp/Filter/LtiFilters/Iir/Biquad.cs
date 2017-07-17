// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Biquad.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using PropertyTools.DataAnnotations;

namespace DspSharp.Filter.LtiFilters.Iir
{
    /// <summary>
    ///     Represents a second-order <see cref="IirFilter" />.
    /// </summary>
    /// <remarks>http://www.musicdsp.org/files/Audio-EQ-Cookbook.txt</remarks>
    public class BiquadFilter : IirFilter
    {
        /// <summary>
        ///     Enumerates all available kinds of biquad filters.
        /// </summary>
        public enum BiquadFilters
        {
            /// <summary>
            ///     The lowpass biquad filter.
            /// </summary>
            Lowpass,

            /// <summary>
            ///     The highpass biquad filter.
            /// </summary>
            Highpass,

            /// <summary>
            ///     The peaking biquad filter.
            /// </summary>
            Peaking,

            /// <summary>
            ///     The bandpass biquad filter.
            /// </summary>
            Bandpass,

            /// <summary>
            ///     The notch biquad filter.
            /// </summary>
            Notch,

            /// <summary>
            ///     The lowshelf biquad filter.
            /// </summary>
            Lowshelf,

            /// <summary>
            ///     The highshelf biquad filter.
            /// </summary>
            Highshelf,

            /// <summary>
            ///     The allpass biquad filter.
            /// </summary>
            Allpass
        }

        private double _Fc;
        private double _Gain;
        private double _Q;
        private BiquadFilters _Type;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BiquadFilter" /> class.
        /// </summary>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="type">The biquad type.</param>
        /// <param name="f0">The corner frequency.</param>
        /// <param name="q">The quality factor.</param>
        /// <param name="gain">The gain factor (for peaking and shelving).</param>
        public BiquadFilter(double sampleRate, BiquadFilters type, double f0, double q, double gain = 1)
            : base(sampleRate)
        {
            this.Name = "biquad filter";
            this._Type = type;
            this._Fc = f0;
            this._Gain = gain;
            this._Q = q;
            this.CalculateCoefficients();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BiquadFilter" /> class.
        /// </summary>
        public BiquadFilter(double sampleRate) : this(sampleRate, BiquadFilters.Highpass, 1000, 0.71)
        {
        }

        public bool IsGainUsed
            =>
                (this.Type == BiquadFilters.Peaking) || (this.Type == BiquadFilters.Highshelf) ||
                (this.Type == BiquadFilters.Lowshelf);

        /// <summary>
        ///     True for valid parameters, otherwise false.
        /// </summary>
        protected override bool HasEffectOverride
        {
            get
            {
                if (double.IsNaN(this.Q) || !(this.Q > 0))
                    return false;

                if (double.IsNaN(this.Fc) || !((this.Fc > 0) && (this.Fc < this.Samplerate / 2.0)))
                    return false;

                if (((this.Type == BiquadFilters.Peaking) || (this.Type == BiquadFilters.Lowshelf) ||
                     (this.Type == BiquadFilters.Highshelf)) &&
                    double.IsNaN(this.Gain))
                    return false;

                return true;
            }
        }

        /// <summary>
        ///     Calculates the biquad coefficients.
        /// </summary>
        protected void CalculateCoefficients()
        {
            var amp = Math.Pow(10, this.Gain / 40);
            var w0 = 2 * Math.PI * this.Fc / this.Samplerate;
            var alpha = Math.Sin(w0) / (2 * this.Q);

            var b = new double[3];
            var a = new double[3];

            switch (this.Type)
            {
            case BiquadFilters.Lowpass:
                b[0] = (1 - Math.Cos(w0)) / 2;
                b[1] = 1 - Math.Cos(w0);
                b[2] = (1 - Math.Cos(w0)) / 2;
                a[0] = 1 + alpha;
                a[1] = -2 * Math.Cos(w0);
                a[2] = 1 - alpha;
                break;
            case BiquadFilters.Highpass:
                b[0] = (1 + Math.Cos(w0)) / 2;
                b[1] = -(1 + Math.Cos(w0));
                b[2] = (1 + Math.Cos(w0)) / 2;
                a[0] = 1 + alpha;
                a[1] = -2 * Math.Cos(w0);
                a[2] = 1 - alpha;
                break;
            case BiquadFilters.Peaking:
                b[0] = 1 + alpha * amp;
                b[1] = -2 * Math.Cos(w0);
                b[2] = 1 - alpha * amp;
                a[0] = 1 + alpha / amp;
                a[1] = -2 * Math.Cos(w0);
                a[2] = 1 - alpha / amp;
                break;
            case BiquadFilters.Bandpass:
                b[0] = alpha;
                b[1] = 0;
                b[2] = -alpha;
                a[0] = 1 + alpha;
                a[1] = -2 * Math.Cos(w0);
                a[2] = 1 - alpha;
                break;
            case BiquadFilters.Notch:
                b[0] = 1;
                b[1] = -2 * Math.Cos(w0);
                b[2] = 1;
                a[0] = 1 + alpha;
                a[1] = -2 * Math.Cos(w0);
                a[2] = 1 - alpha;
                break;
            case BiquadFilters.Allpass:
                b[0] = 1 - alpha;
                b[1] = -2 * Math.Cos(w0);
                b[2] = 1 + alpha;
                a[0] = 1 + alpha;
                a[1] = -2 * Math.Cos(w0);
                a[2] = 1 - alpha;
                break;
            case BiquadFilters.Lowshelf:
                b[0] = amp * (amp + 1 - (amp - 1) * Math.Cos(w0) + 2 * Math.Sqrt(amp) * alpha);
                b[1] = 2 * amp * (amp - 1 - (amp + 1) * Math.Cos(w0));
                b[2] = amp * (amp + 1 - (amp - 1) * Math.Cos(w0) - 2 * Math.Sqrt(amp) * alpha);
                a[0] = amp + 1 + (amp - 1) * Math.Cos(w0) + 2 * Math.Sqrt(amp) * alpha;
                a[1] = -2 * (amp - 1 + (amp + 1) * Math.Cos(w0));
                a[2] = amp + 1 + (amp - 1) * Math.Cos(w0) - 2 * Math.Sqrt(amp) * alpha;
                break;
            case BiquadFilters.Highshelf:
                b[0] = amp * (amp + 1 + (amp - 1) * Math.Cos(w0) + 2 * Math.Sqrt(amp) * alpha);
                b[1] = -2 * amp * (amp - 1 + (amp + 1) * Math.Cos(w0));
                b[2] = amp * (amp + 1 + (amp - 1) * Math.Cos(w0) - 2 * Math.Sqrt(amp) * alpha);
                a[0] = amp + 1 - (amp - 1) * Math.Cos(w0) + 2 * Math.Sqrt(amp) * alpha;
                a[1] = 2 * (amp - 1 - (amp + 1) * Math.Cos(w0));
                a[2] = amp + 1 - (amp - 1) * Math.Cos(w0) - 2 * Math.Sqrt(amp) * alpha;
                break;
            }

            this.SetCoefficients(a, b);
            this.RaisePropertyChanged(nameof(this.a0));
            this.RaisePropertyChanged(nameof(this.a1));
            this.RaisePropertyChanged(nameof(this.a2));
            this.RaisePropertyChanged(nameof(this.b0));
            this.RaisePropertyChanged(nameof(this.b1));
            this.RaisePropertyChanged(nameof(this.b2));
            this.RaisePropertyChanged(nameof(this.A));
            this.RaisePropertyChanged(nameof(this.B));
        }

        /// <summary>
        ///     The type of the <see cref="BiquadFilter" />.
        /// </summary>
        [Category("biquad")]
        [DisplayName("filter type")]
        public BiquadFilters Type
        {
            get { return this._Type; }
            set
            {
                this.SetField(ref this._Type, value);
                this.RaisePropertyChanged(nameof(this.IsGainUsed));
                this.CalculateCoefficients();
                this.RaiseChangedEvent();
            }
        }

        /// <summary>
        ///     The corner frequenciy Fc.
        /// </summary>
        [DisplayName("corner frequency")]
        public double Fc
        {
            get { return this._Fc; }
            set
            {
                this.SetField(ref this._Fc, value);
                this.CalculateCoefficients();
                this.RaiseChangedEvent();
            }
        }

        /// <summary>
        ///     The gain factor (for peaking and shelving filters).
        /// </summary>
        [DisplayName("gain")]
        public double Gain
        {
            get { return this._Gain; }
            set
            {
                this.SetField(ref this._Gain, value);
                this.CalculateCoefficients();
                this.RaiseChangedEvent();
            }
        }

        /// <summary>
        ///     The quality factor Q.
        /// </summary>
        [DisplayName("quality factor")]
        public double Q
        {
            get { return this._Q; }
            set
            {
                this.SetField(ref this._Q, value);
                this.CalculateCoefficients();
                this.RaiseChangedEvent();
            }
        }

        [Category("coefficients")]
        [DisplayName("a0")]
        public double a0 => this.A[0];

        [DisplayName("a1")]
        public double a1 => this.A[1];

        [DisplayName("a2")]
        public double a2 => this.A[2];

        [DisplayName("b0")]
        public double b0 => this.B[0];

        [DisplayName("b1")]
        public double b1 => this.B[1];

        [DisplayName("b2")]
        public double b2 => this.B[2];
    }
}