using System;
using System.Collections.Generic;
using Filter.Algorithms;
using Filter.Extensions;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    ///     Represents a filter with a_n and b_n coefficients with an infinite impulse response.
    /// </summary>
    public class IirFilter : LtiFilterBase
    {
        private IReadOnlyList<double> _a;
        private IReadOnlyList<double> _b;
        private int _order;

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
                {
                    return false;
                }
                if (this.Order == 0)
                {
                    return false;
                }
                return true;
            }
        }

        protected override void OnSamplerateChanged()
        {
            this.OnChange();
        }

        public override IEnumerable<double> Process(IEnumerable<double> signal)
        {
            return Dsp.IirFilter(signal, this.A, this.B);
        }

        protected void SetCoefficients(IEnumerable<double> a, IEnumerable<double> b)
        {
            this.A = a.ToReadOnlyList();
            this.B = b.ToReadOnlyList();

            var n = this.A.Count;

            if (n != this.B.Count)
            {
                throw new ArgumentException();
            }

            this.Order = n - 1;
        }
    }
}