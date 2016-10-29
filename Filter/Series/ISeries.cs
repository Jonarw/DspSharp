using System;
using System.Collections.Generic;

namespace Filter.Series
{
    /// <summary>
    /// Defines methods and properties used to interface a series of values.
    /// </summary>
    public interface ISeries : IEquatable<ISeries>
    {
        /// <summary>
        /// True for a logarithmic series.
        /// </summary>
        bool IsLogarithmic { get; }

        /// <summary>
        /// Computes the Hashcode for the <see cref="ISeries"/> object.
        /// </summary>
        /// <returns>The Hashcode.</returns>
        int GetHashCode();

        IEnumerable<double> Values { get; }

        int Length { get; }
    }
}