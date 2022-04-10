// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IirFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Algorithms;
using DspSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DspSharp.Filter.LtiFilters.Iir
{
    /// <summary>
    /// Represents a digital IIR (infinite impulse response) filter.
    /// </summary>
    public abstract class IirFilter : FilterBase
    {
        private IReadOnlyList<double> _a;
        private IReadOnlyList<double> _b;
        private int _order;

        protected IirFilter(double samplerate) : base(samplerate)
        {
            this.DisplayName = "IIR filter";
        }

        /// <summary>
        /// Gets or sets the denominator coefficients.
        /// </summary>
        public IReadOnlyList<double> A
        {
            get => this._a;
            private set => this.SetField(ref this._a, value);
        }

        /// <summary>
        /// Gets or sets the numerator coefficients.
        /// </summary>
        public IReadOnlyList<double> B
        {
            get => this._b;
            private set => this.SetField(ref this._b, value);
        }

        /// <summary>
        /// Gets the filter order.
        /// </summary>
        public int Order
        {
            get => this._order;
            private set => this.SetField(ref this._order, value);
        }

        /// <inheritdoc/>
        protected override bool HasEffectOverride
        {
            get
            {
                if ((this.A == null) || (this.B == null))
                    return false;
                if (this.Order == 0)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Gets the frequency response of the filter at the specified frequency points.
        /// </summary>
        /// <param name="frequencies">The frequencies.</param>
        public ILazyReadOnlyCollection<Complex> GetFrequencyResponse(IReadOnlyList<double> frequencies)
        {
            return FrequencyDomain.IirFrequencyResponse(this.A, this.B, frequencies, this.Samplerate);
        }

        /// <inheritdoc/>
        protected override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return TimeDomain.IirFilter(signal, this.A, this.B);
        }

        /// <summary>
        /// Sets the filter coefficients.
        /// </summary>
        /// <param name="a">The denominator coefficients.</param>
        /// <param name="b">The numerator coefficients.</param>
        protected void SetCoefficients(IEnumerable<double> a, IEnumerable<double> b)
        {
            var aList = a.ToList();
            var bList = b.ToList();

            if (aList.Count != bList.Count)
                throw new ArgumentException("a and b must be the same length.");

            this.A = aList;
            this.B = bList;
            this.Order = aList.Count - 1;
        }
    }
}