// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFiniteFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace DspSharp.Filter
{
    /// <summary>
    ///     Describes a digital filter with a finite impulse response.
    /// </summary>
    /// <seealso cref="IFilter" />
    public interface IFiniteFilter : IFilter
    {
        /// <summary>
        ///     Processes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        IReadOnlyList<double> Process(IReadOnlyList<double> input);
    }
}