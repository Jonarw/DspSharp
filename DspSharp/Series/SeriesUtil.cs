// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SeriesUtil.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace DspSharp.Series
{
    /// <summary>
    ///     Provides methods for operating on <see cref="ISeries" /> objects.
    /// </summary>
    public sealed class SeriesUtil
    {
        /// <summary>
        ///     Computes the union of two <see cref="ISeries" /> objects, which contains every value in either of the source series
        ///     exactly once.
        /// </summary>
        /// <param name="s1">The first source series.</param>
        /// <param name="s2">The second source series.</param>
        /// <returns>A new <see cref="ISeries" /> object containing the result.</returns>
        public static ISeries Merge(ISeries s1, ISeries s2)
        {
            if (s1.Equals(s2))
                return s1;
            var values = s1.Values.Union(s2.Values).OrderBy(m => m);

            return new CustomSeries(values, s1.IsLogarithmic && s2.IsLogarithmic);
        }
    }
}