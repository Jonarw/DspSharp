// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FiniteFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Filter
{
    /// <summary>
    /// Represents a filter with a finite impulse response.
    /// </summary>
    public abstract class FiniteFilter : FilterBase, IFiniteFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FiniteFilter" /> class.
        /// </summary>
        /// <param name="samplerate">The samplerate.</param>
        protected FiniteFilter(double samplerate) : base(samplerate)
        {
        }

        /// <inheritdoc/>
        public sealed override bool HasInfiniteImpulseResponse => false;

        /// <inheritdoc/>
        public IReadOnlyList<double> Process(IReadOnlyList<double> input)
        {
            return this.HasEffect ? this.ProcessOverride(input) : input;
        }

        /// <inheritdoc/>
        protected IReadOnlyList<double> ProcessOverride(IReadOnlyList<double> input)
        {
            return this.ProcessOverride((IEnumerable<double>)input).ToList();
        }
    }
}