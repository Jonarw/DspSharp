// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterExtensions.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Algorithms;
using DspSharp.Filter;
using DspSharp.Signal;

namespace DspSharp.Extensions
{
    /// <summary>
    ///     Provides static extension for the IFilter interface.
    /// </summary>
    public static class FilterExtensions
    {
        /// <summary>
        ///     Computes the impulse response of the filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The impulse response.</returns>
        public static IEnumerableSignal GetImpulseResponse(this IFilter filter)
        {
            return new EnumerableSignal(filter.Process(1.0.ToEnumerable()), filter.Samplerate);
        }

        public static IFilter Chain(this IFilter filter1, IFilter filter2)
        {
            var set = filter1 as FilterSet;
            if (set != null)
                set.Filters.Add(filter2);
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