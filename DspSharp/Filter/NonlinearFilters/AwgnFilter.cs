// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwgnFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DspSharp.Algorithms;

namespace DspSharp.Filter.NonlinearFilters
{
    /// <summary>
    ///     Represents a filter that adds white Gaussian noise to a signal.
    /// </summary>
    /// <seealso cref="FilterBase" />
    public class AwgnFilter : FiniteFilter
    {
        private double _Sigma;
        private double _Variance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AwgnFilter" /> class.
        /// </summary>
        /// <param name="samplerate">The samplerate.</param>
        public AwgnFilter(double samplerate) : base(samplerate)
        {
            this.Variance = .1;
        }

        public double Sigma
        {
            get { return this._Sigma; }
            set
            {
                this.SetField(ref this._Sigma, value);
                this.SetField(nameof(this.Variance), ref this._Variance, Math.Pow(this.Sigma, 2));
            }
        }

        public double Variance
        {
            get { return this._Variance; }
            set
            {
                this.SetField(ref this._Variance, value);
                this.SetField(nameof(this.Sigma), ref this._Sigma, Math.Sqrt(this.Variance));
            }
        }

        /// <summary>
        ///     Specifies whether the filter object has an effect or not.
        /// </summary>
        protected override bool HasEffectOverride => true;

        /// <summary>
        ///     Processes the specified sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <returns></returns>
        public override IEnumerable<double> ProcessOverride(IEnumerable<double> input)
        {
            return input.Add(SignalGenerators.WhiteNoise().Multiply(this.Sigma));
        }
    }
}