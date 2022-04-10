// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BiquadFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace DspSharp.Filter.LtiFilters.Iir
{
    /// <summary>
    /// Represents a second-order <see cref="IirFilter" />.
    /// </summary>
    /// <remarks>http://www.musicdsp.org/files/Audio-EQ-Cookbook.txt</remarks>
    public class BiquadFilter : IirFilter
    {
        private double _Fc;
        private double _Gain;
        private double _Q;
        private BiquadFilterType _Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="BiquadFilter" /> class.
        /// </summary>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="type">The biquad type.</param>
        /// <param name="f0">The corner frequency.</param>
        /// <param name="q">The quality factor.</param>
        /// <param name="gain">The gain factor (for peaking and shelving).</param>
        public BiquadFilter(double sampleRate, BiquadFilterType type, double f0, double q, double gain = 1)
            : base(sampleRate)
        {
            this.DisplayName = "biquad filter";
            this._Type = type;
            this._Fc = f0;
            this._Gain = gain;
            this._Q = q;
            this.CalculateCoefficients();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BiquadFilter" /> class.
        /// </summary>
        public BiquadFilter(double sampleRate) : this(sampleRate, BiquadFilterType.Highpass, 1000, 0.71)
        {
        }

        public double A0 => this.A[0];

        public double A1 => this.A[1];

        public double A2 => this.A[2];

        public double B0 => this.B[0];

        public double B1 => this.B[1];

        public double B2 => this.B[2];

        public double A0n => 1;

        public double A1n => this.A1 / this.A0;

        public double A2n => this.A2 / this.A0;

        public double B0n => this.B0 / this.A0;

        public double B1n => this.B1 / this.A0;

        public double B2n => this.B2 / this.A0;

        /// <summary>
        /// The corner frequency.
        /// </summary>
        public double Fc
        {
            get => this._Fc;
            set
            {
                this.SetField(ref this._Fc, value);
                this.CalculateCoefficients();
                this.RaiseChangedEvent();
            }
        }

        /// <summary>
        /// The gain in dB (for peaking and shelving filters).
        /// </summary>
        public double Gain
        {
            get => this._Gain;
            set
            {
                this.SetField(ref this._Gain, value);
                this.CalculateCoefficients();
                this.RaiseChangedEvent();
            }
        }

        public bool IsGainUsed => this.Type == BiquadFilterType.Peaking || this.Type == BiquadFilterType.Highshelf || this.Type == BiquadFilterType.Lowshelf;

        /// <summary>
        /// The quality factor Q.
        /// </summary>
        public double Q
        {
            get => this._Q;
            set
            {
                this.SetField(ref this._Q, value);
                this.CalculateCoefficients();
                this.RaiseChangedEvent();
            }
        }

        /// <summary>
        /// The filter type.
        /// </summary>
        public BiquadFilterType Type
        {
            get => this._Type;
            set
            {
                this.SetField(ref this._Type, value);
                this.OnPropertyChanged(nameof(this.IsGainUsed));
                this.CalculateCoefficients();
                this.RaiseChangedEvent();
            }
        }

        /// <inheritdoc/>
        protected override bool HasEffectOverride
        {
            get
            {
                if (double.IsNaN(this.Q) || !(this.Q > 0))
                    return false;

                if (double.IsNaN(this.Fc) || !(this.Fc > 0 && this.Fc < this.Samplerate / 2.0))
                    return false;

                if (this.IsGainUsed && double.IsNaN(this.Gain))
                    return false;

                return true;
            }
        }

        public static (double a0, double a1, double a2, double b0, double b1, double b2) CalculateCoefficients(BiquadFilterType type, double samplerate, double f, double q, double gain = 0)
        {
            var amp = Math.Pow(10, gain / 40);
            var w0 = 2 * Math.PI * f / samplerate;
            var alpha = Math.Sin(w0) / (2 * q);

            double a0, a1, a2, b0, b1, b2;

            switch (type)
            {
                case BiquadFilterType.Lowpass:
                    b0 = (1 - Math.Cos(w0)) / 2;
                    b1 = 1 - Math.Cos(w0);
                    b2 = (1 - Math.Cos(w0)) / 2;
                    a0 = 1 + alpha;
                    a1 = -2 * Math.Cos(w0);
                    a2 = 1 - alpha;
                    break;
                case BiquadFilterType.Highpass:
                    b0 = (1 + Math.Cos(w0)) / 2;
                    b1 = -(1 + Math.Cos(w0));
                    b2 = (1 + Math.Cos(w0)) / 2;
                    a0 = 1 + alpha;
                    a1 = -2 * Math.Cos(w0);
                    a2 = 1 - alpha;
                    break;
                case BiquadFilterType.Peaking:
                    b0 = 1 + alpha * amp;
                    b1 = -2 * Math.Cos(w0);
                    b2 = 1 - alpha * amp;
                    a0 = 1 + alpha / amp;
                    a1 = -2 * Math.Cos(w0);
                    a2 = 1 - alpha / amp;
                    break;
                case BiquadFilterType.Bandpass:
                    b0 = alpha;
                    b1 = 0;
                    b2 = -alpha;
                    a0 = 1 + alpha;
                    a1 = -2 * Math.Cos(w0);
                    a2 = 1 - alpha;
                    break;
                case BiquadFilterType.Notch:
                    b0 = 1;
                    b1 = -2 * Math.Cos(w0);
                    b2 = 1;
                    a0 = 1 + alpha;
                    a1 = -2 * Math.Cos(w0);
                    a2 = 1 - alpha;
                    break;
                case BiquadFilterType.Allpass:
                    b0 = 1 - alpha;
                    b1 = -2 * Math.Cos(w0);
                    b2 = 1 + alpha;
                    a0 = 1 + alpha;
                    a1 = -2 * Math.Cos(w0);
                    a2 = 1 - alpha;
                    break;
                case BiquadFilterType.Lowshelf:
                    b0 = amp * (amp + 1 - (amp - 1) * Math.Cos(w0) + 2 * Math.Sqrt(amp) * alpha);
                    b1 = 2 * amp * (amp - 1 - (amp + 1) * Math.Cos(w0));
                    b2 = amp * (amp + 1 - (amp - 1) * Math.Cos(w0) - 2 * Math.Sqrt(amp) * alpha);
                    a0 = amp + 1 + (amp - 1) * Math.Cos(w0) + 2 * Math.Sqrt(amp) * alpha;
                    a1 = -2 * (amp - 1 + (amp + 1) * Math.Cos(w0));
                    a2 = amp + 1 + (amp - 1) * Math.Cos(w0) - 2 * Math.Sqrt(amp) * alpha;
                    break;
                case BiquadFilterType.Highshelf:
                    b0 = amp * (amp + 1 + (amp - 1) * Math.Cos(w0) + 2 * Math.Sqrt(amp) * alpha);
                    b1 = -2 * amp * (amp - 1 + (amp + 1) * Math.Cos(w0));
                    b2 = amp * (amp + 1 + (amp - 1) * Math.Cos(w0) - 2 * Math.Sqrt(amp) * alpha);
                    a0 = amp + 1 - (amp - 1) * Math.Cos(w0) + 2 * Math.Sqrt(amp) * alpha;
                    a1 = 2 * (amp - 1 - (amp + 1) * Math.Cos(w0));
                    a2 = amp + 1 - (amp - 1) * Math.Cos(w0) - 2 * Math.Sqrt(amp) * alpha;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return (a0, a1, a2, b0, b1, b2);
        }

        /// <summary>
        ///     Calculates the biquad coefficients.
        /// </summary>
        protected void CalculateCoefficients()
        {
            var (a0, a1, a2, b0, b1, b2) = CalculateCoefficients(this.Type, this.Samplerate, this.Fc, this.Q, this.Gain);

            this.SetCoefficients(new[] { a0, a1, a2 }, new[] { b0, b1, b2 });
            this.OnPropertyChanged(nameof(this.A0));
            this.OnPropertyChanged(nameof(this.A1));
            this.OnPropertyChanged(nameof(this.A2));
            this.OnPropertyChanged(nameof(this.B0));
            this.OnPropertyChanged(nameof(this.B1));
            this.OnPropertyChanged(nameof(this.B2));
            this.OnPropertyChanged(nameof(this.A));
            this.OnPropertyChanged(nameof(this.B));
        }
    }
}