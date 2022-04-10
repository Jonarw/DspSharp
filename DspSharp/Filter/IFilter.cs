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
    /// Describes a digital filter.
    /// </summary>
    public interface IFilter : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a value indicating whether this instance has an infinite impulse response.
        /// </summary>
        bool HasInfiniteImpulseResponse { get; }

        /// <summary>
        /// Gets the samplerate.
        /// </summary>
        double Samplerate { get; }

        /// <summary>
        /// Processes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        IEnumerable<double> Process(IEnumerable<double> input);

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="IFilter"/> is enabled.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IFilter"/> has an effect.
        /// </summary>
        bool HasEffect { get; }
    }
}