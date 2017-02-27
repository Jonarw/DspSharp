﻿using System.Collections.Generic;
using Filter.Algorithms;
using Filter.Extensions;

namespace Filter
{
    /// <summary>
    ///     Represents a filter with a finite impulse response.
    /// </summary>
    /// <seealso cref="Filter.FilterBase" />
    /// <seealso cref="Filter.IFiniteFilter" />
    public abstract class FiniteFilter : FilterBase, IFiniteFilter
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FiniteFilter" /> class.
        /// </summary>
        /// <param name="samplerate">The samplerate.</param>
        protected FiniteFilter(double samplerate) : base(samplerate)
        {
        }

        /// <summary>
        ///     Gets a value indicating whether this instance has infinite impulse response.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has an infinite impulse response; otherwise, <c>false</c>.
        /// </value>
        public sealed override bool HasInfiniteImpulseResponse => false;

        public IReadOnlyList<double> Process(IReadOnlyList<double> input)
        {
            return this.HasEffect ? this.ProcessOverride(input) : input;
        }

        /// <summary>
        ///     Processes the specified sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <returns></returns>
        public IReadOnlyList<double> ProcessOverride(IReadOnlyList<double> input)
        {
            return this.ProcessOverride((IEnumerable<double>)input).ToReadOnlyList();
        }
    }
}