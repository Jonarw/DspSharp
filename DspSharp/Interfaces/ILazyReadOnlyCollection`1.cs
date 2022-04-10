// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SamplerateMismatchException.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace DspSharp.Interfaces
{
    /// <summary>
    /// Represents a collection with a known number of items. This interface is used to indicate that the items of a returned collection are NOT a materialized in-memory collection, but are rather computed as the collection is iterated over, which could potentially be computationally expensive.
    /// </summary>
    public interface ILazyReadOnlyCollection<T> : IReadOnlyCollection<T>
    {
    }
}