// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp.Extensions;
using DspSharp.Signal;
using DspSharp.Signal.Windows;

namespace DspSharp.Filter.LtiFilters.Fir
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
        private WindowType _WindowType = WindowType.Hann;

        public FirFilter(double samplerate) : base(samplerate)
        {
            this.DisplayName = "FIR filter";
        }

        /// <summary>
        ///     The cut-off frequency.
        /// </summary>
        public double Fc
        {
            get { return this._F0; }
            set
            {
                this.SetField(ref this._F0, value);
                this.Coefficients = null;
            }
        }

        /// <summary>
        ///     The length of the FIR-Filter.
        /// </summary>
        public int FilterLength
        {
            get { return this._FilterLength; }
            set
            {
                this.SetField(ref this._FilterLength, value);
                this.Coefficients = null;
            }
        }

        /// <summary>
        ///     Gets or sets the type of the filter.
        /// </summary>
        public Types FilterType
        {
            get { return this._FilterType; }
            set
            {
                this.SetField(ref this._FilterType, value);
                this.Coefficients = null;
            }
        }

        public override IReadOnlyList<double> ImpulseResponse
        {
            get
            {
                if (this.Coefficients == null)
                {
                    var win = new Window(this.WindowType, this.FilterLength, this.Samplerate, WindowModes.Symmetric);

                    ISignal coef;
                    if (this.FilterType == Types.Highpass)
                        coef = new IdealHighpass(this.Samplerate, this.Fc);
                    else
                        coef = new IdealLowpass(this.Samplerate, this.Fc);

                    this.Coefficients = coef.Multiply(win).Signal;
                }

                return this.Coefficients;
            }
        }

        /// <summary>
        ///     The type of the window used for fading out the infinite-length sinc pulse.
        /// </summary>
        public WindowType WindowType
        {
            get { return this._WindowType; }
            set
            {
                this.SetField(ref this._WindowType, value);
                this.Coefficients = null;
            }
        }

        /// <summary>
        ///     Returns true if all parameters are valid, false otherwise.
        /// </summary>
        protected override bool HasEffectOverride
        {
            get
            {
                if (this.Fc <= 0 || this.FilterLength <= 0)
                    return false;
                return true;
            }
        }

        private IReadOnlyList<double> Coefficients { get; set; }
    }
}