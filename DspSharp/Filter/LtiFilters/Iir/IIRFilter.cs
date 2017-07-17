// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIRFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DspSharp.Algorithms;

namespace DspSharp.Filter.LtiFilters.Iir
{
    /// <summary>
    ///     Represents a filter with a_n and b_n coefficients with an infinite impulse response.
    /// </summary>
    public class IirFilter : FilterBase
    {
        private IReadOnlyList<double> _a;
        private IReadOnlyList<double> _b;
        private int _order;

        public IirFilter(double samplerate) : base(samplerate)
        {
            this.Name = "IIR filter";
        }

        /// <summary>
        ///     Gets or sets the denominator coefficients.
        /// </summary>
        public IReadOnlyList<double> A
        {
            get { return this._a; }
            private set { this.SetField(ref this._a, value); }
        }

        /// <summary>
        ///     Gets or sets the numerator coefficients.
        /// </summary>
        public IReadOnlyList<double> B
        {
            get { return this._b; }
            private set { this.SetField(ref this._b, value); }
        }

        /// <summary>
        ///     Gets the filter order.
        /// </summary>
        public int Order
        {
            get { return this._order; }
            private set { this.SetField(ref this._order, value); }
        }

        /// <summary>
        ///     True if coefficients are valid, false otherwise.
        /// </summary>
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

        public override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return TimeDomain.IirFilter(signal, this.A, this.B);
        }

        protected void SetCoefficients(IEnumerable<double> a, IEnumerable<double> b)
        {
            this.A = a.ToReadOnlyList();
            this.B = b.ToReadOnlyList();

            var n = this.A.Count;

            if (n != this.B.Count)
                throw new ArgumentException();

            this.Order = n - 1;
        }
    }
}