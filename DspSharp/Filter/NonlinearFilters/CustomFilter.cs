// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DspSharp.Filter.NonlinearFilters
{
    /// <summary>
    /// Represents a filter that is based on a custom filter function.
    /// </summary>
    public class CustomFilter : FilterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFilter" /> class.
        /// </summary>
        /// <param name="samplerate">The samplerate.</param>
        /// <param name="filterFunction">The filter function.</param>
        public CustomFilter(double samplerate, Func<IEnumerable<double>, IEnumerable<double>> filterFunction)
            : base(samplerate)
        {
            this.FilterFunction = filterFunction;
        }

        /// <summary>
        /// Gets the filter function.
        /// </summary>
        public Func<IEnumerable<double>, IEnumerable<double>> FilterFunction { get; }

        /// <inheritdoc/>
        protected override bool HasEffectOverride => true;

        /// <inheritdoc/>
        protected override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return this.FilterFunction(signal);
        }
    }
}