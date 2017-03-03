// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISeries.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DspSharp.Series
{
    /// <summary>
    ///     Defines methods and properties used to interface a series of values.
    /// </summary>
    public interface ISeries : IEquatable<ISeries>
    {
        /// <summary>
        ///     True for a logarithmic series.
        /// </summary>
        bool IsLogarithmic { get; }

        int Length { get; }

        IReadOnlyList<double> Values { get; }

        /// <summary>
        ///     Computes the Hashcode for the <see cref="ISeries" /> object.
        /// </summary>
        /// <returns>The Hashcode.</returns>
        int GetHashCode();
    }
}