// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISignalBasedFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Collections;
using DspSharp.Signal;

namespace DspSharp
{
    /// <summary>
    ///     Describes a filter that uses a predefined signal from a list of available signals.
    /// </summary>
    public interface ISignalBasedFilter
    {
        /// <summary>
        ///     Gets or sets the available signals.
        /// </summary>
        IReadOnlyObservableList<ISignal> AvailableSignals { get; set; }
    }
}