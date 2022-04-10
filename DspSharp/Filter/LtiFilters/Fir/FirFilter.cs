// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Algorithms;
using DspSharp.Extensions;
using DspSharp.Signal.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using UTilities;

namespace DspSharp.Filter.LtiFilters.Fir
{
    /// <summary>
    /// Represents a FIR lowpass or highpass filter.
    /// </summary>
    public class FirFilter : Convolver
    {
        private const int Defaultfilterlength = 10000;
        private double _F0;
        private int _FilterLength = Defaultfilterlength;
        private FirFilterType _FilterType;
        private WindowType _WindowType = WindowType.Hann;

        public FirFilter(double samplerate) : base(samplerate)
        {
            this.DisplayName = "FIR filter";
        }

        /// <summary>
        /// The cut-off frequency.
        /// </summary>
        public double Fc
        {
            get => this._F0;
            set => this.SetField(ref this._F0, value, this.InvalidateCoefficients);
        }

        /// <summary>
        /// The length of the FIR-Filter.
        /// </summary>
        public int FilterLength
        {
            get => this._FilterLength;
            set => this.SetField(ref this._FilterLength, value, this.InvalidateCoefficients);
        }

        /// <summary>
        /// Gets or sets the type of the filter.
        /// </summary>
        public FirFilterType FilterType
        {
            get => this._FilterType;
            set => this.SetField(ref this._FilterType, value, this.InvalidateCoefficients);
        }

        /// <inheritdoc/>
        public override IReadOnlyList<double> ImpulseResponse
        {
            get
            {
                if (this.Coefficients == null)
                {
                    var w = 2 * this.Fc / this.Samplerate;
                    Func<int, double> sinc = this.FilterType switch
                    {
                        FirFilterType.Lowpass => time => Mathematic.Sinc(2 * time) * w,
                        FirFilterType.Highpass => time => (time == 0 ? 1.0 : 0.0) - Mathematic.Sinc(2 * time) * w,
                        _ => throw EnumOutOfRangeException.Create(this.FilterType),
                    };

                    var win = Window.CreateWindow(this.WindowType, WindowMode.Symmetric, this.FilterLength);
                    this.Coefficients = win
                        .SelectWithCount((d, i) => d * sinc(i - this.FilterLength / 2))
                        .ToList();
                }

                return this.Coefficients;
            }
        }

        /// <summary>
        /// The type of the window used for fading out the infinite-length sinc pulse.
        /// </summary>
        public WindowType WindowType
        {
            get => this._WindowType;
            set => this.SetField(ref this._WindowType, value, this.InvalidateCoefficients);
        }

        /// <inheritdoc/>
        protected override bool HasEffectOverride => this.Fc > 0 && this.FilterLength > 0;

        private IReadOnlyList<double> Coefficients { get; set; }

        private void InvalidateCoefficients()
        {
            this.Coefficients = null;
        }
    }
}