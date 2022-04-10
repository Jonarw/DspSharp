// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterExtensions.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Filter;
using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Extensions
{
    /// <summary>
    /// Provides static extension for the IFilter interface.
    /// </summary>
    public static class FilterExtensions
    {
        /// <summary>
        /// Computes the impulse response of the filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public static IEnumerable<double> GetImpulseResponse(this IFilter filter)
        {
            return filter.Process(Enumerable.Repeat(1d, 1));
        }

        /// <summary>
        /// Chains two filters together using a <see cref="FilterSet"/>.
        /// </summary>
        /// <param name="filter1">The first filter.</param>
        /// <param name="filter2">The second filter.</param>
        /// <remarks>If <paramref name="filter1"/> is a <see cref="FilterSet"/>, <paramref name="filter2"/> is appended and <paramref name="filter1"/>is returned. Otherwise a new <see cref="FilterSet"/> containing both filters is created and returned.</remarks>
        public static IFilter Chain(this IFilter filter1, IFilter filter2)
        {
            if (filter1 is FilterSet set)
            {
                set.Filters.Add(filter2);
            }
            else
            {
                set = new FilterSet(filter1.Samplerate);
                set.Filters.Add(filter1);
                set.Filters.Add(filter2);
            }

            return set;
        }
    }
}