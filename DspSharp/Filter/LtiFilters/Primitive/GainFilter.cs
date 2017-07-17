// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GainFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp.Algorithms;
using PropertyTools.DataAnnotations;

namespace DspSharp.Filter.LtiFilters.Primitive
{
    /// <summary>
    ///     Represents a filter with a constant gain and no effects otherwise.
    /// </summary>
    public class GainFilter : FiniteFilter
    {
        private double _Gain = 1;

        public GainFilter(double samplerate) : base(samplerate)
        {
            this.Name = "Gain Filter";
        }

        /// <summary>
        ///     True if <see cref="Gain" /> is not 1, false otherwise.
        /// </summary>
        protected override bool HasEffectOverride
        {
            get
            {
                if (this.Gain == 1)
                    return false;

                return true;
            }
        }

        public override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return signal.Multiply(this.Gain);
        }

        /// <summary>
        ///     Gets or sets the linear gain factor of the <see cref="GainFilter" />.
        /// </summary>
        [Category("Gain Filter")]
        [DisplayName("Gain")]
        public double Gain
        {
            get { return this._Gain; }
            set { this.SetField(ref this._Gain, value); }
        }
    }
}