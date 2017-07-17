// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;

namespace DspSharp.Filter
{
    /// <summary>
    ///     Describes a digital filter.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface IFilter : INotifyPropertyChanged
    {
        /// <summary>
        ///     Gets a value indicating whether this instance has an infinite impulse response.
        /// </summary>
        bool HasInfiniteImpulseResponse { get; }

        /// <summary>
        ///     Gets the samplerate.
        /// </summary>
        double Samplerate { get; }

        /// <summary>
        ///     Processes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        IEnumerable<double> Process(IEnumerable<double> input);
    }
}