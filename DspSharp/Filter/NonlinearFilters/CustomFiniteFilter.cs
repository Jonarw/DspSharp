// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomFiniteFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DspSharp.Filter.NonlinearFilters
{
    /// <summary>
    ///     Represents a filter that is based on a custom finite filter function.
    /// </summary>
    /// <seealso cref="FiniteFilter" />
    public class CustomFiniteFilter : FiniteFilter
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomFiniteFilter" /> class.
        /// </summary>
        /// <param name="samplerate">The samplerate.</param>
        /// <param name="filterFunction">The filter function.</param>
        public CustomFiniteFilter(double samplerate, Func<IEnumerable<double>, IEnumerable<double>> filterFunction)
            : base(samplerate)
        {
            this.FilterFunction = filterFunction;
        }

        /// <summary>
        ///     Gets the filter function.
        /// </summary>
        public Func<IEnumerable<double>, IEnumerable<double>> FilterFunction { get; }

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
            return this.FilterFunction(input);
        }
    }
}