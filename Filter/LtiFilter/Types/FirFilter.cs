using System;
using System.Collections.Generic;
using Filter.Signal.Windows;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    ///     A standard FIR filter, e.g. lowpass, highpass etc.
    /// </summary>
    public class FirFilter : Convolver
    {
        /// <summary>
        ///     Enumeration of the supported filter types.
        /// </summary>
        public enum Types
        {
            /// <summary>
            ///     FIR lowpass filter.
            /// </summary>
            Lowpass,

            /// <summary>
            ///     FIR highpass filter.
            /// </summary>
            Highpass
        }

        private const int Defaultfilterlength = 10000;
        private double _F0;
        private int _FilterLength = Defaultfilterlength;
        private Types _FilterType;
        private WindowTypes _WindowType = WindowTypes.Hann;

        public FirFilter(double samplerate) : base(samplerate)
        {
        }

        /// <summary>
        ///     The cut-off frequency.
        /// </summary>
        public double Fc
        {
            get { return this._F0; }
            set { this.SetField(ref this._F0, value); }
        }

        /// <summary>
        ///     The length of the FIR-Filter.
        /// </summary>
        public int FilterLength
        {
            get { return this._FilterLength; }
            set { this.SetField(ref this._FilterLength, value); }
        }

        /// <summary>
        ///     Gets or sets the type of the filter.
        /// </summary>
        public Types FilterType
        {
            get { return this._FilterType; }
            set { this.SetField(ref this._FilterType, value); }
        }

        public override IReadOnlyList<double> ImpulseResponse
        {
            get
            {
                //if (this.Win == null)
                //{
                //    this.Win = new Window(this.WindowType, this.FilterLength, this.Samplerate);
                //}

                //var lowpass = this.Win.Multiply(2 * this.Fc / this.Samplerate).Multiply(new Sinc(this.Fc, this.Samplerate, this.FilterLength));

                //if (this.FilterType == Types.Lowpass)
                //{
                //    return lowpass.GetSamples().ToReadOnlyList();
                //}

                //if (this.FilterType == Types.Highpass)
                //{
                //    return new Dirac(1, this.Samplerate).Subtract(lowpass).GetSamples().ToReadOnlyList();
                //}

                //return new Dirac(1, this.Samplerate).GetSamples().ToReadOnlyList();
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     The type of the window used for fading out the infinite-length sinc pulse.
        /// </summary>
        public WindowTypes WindowType
        {
            get { return this._WindowType; }
            set
            {
                if (this.SetField(ref this._WindowType, value))
                {
                    this.Win = null;
                }
            }
        }

        /// <summary>
        ///     Returns true if all parameters are valid, false otherwise.
        /// </summary>
        protected override bool HasEffectOverride
        {
            get
            {
                if ((this.Fc <= 0) || (this.FilterLength <= 0))
                {
                    return false;
                }
                return true;
            }
        }

        private Window Win { get; set; }
    }
}