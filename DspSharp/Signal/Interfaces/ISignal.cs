// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISignal.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace DspSharp.Signal
{
    /// <summary>
    ///     Describes an arbitrary digital signal representable in time domain.
    /// </summary>
    public interface ISignal
    {
        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        ///     Gets the sample rate.
        /// </summary>
        double SampleRate { get; }

        /// <summary>
        ///     Gets a section of the signal in time domain.
        /// </summary>
        /// <param name="start">The start of the section.</param>
        /// <param name="length">The length of the section.</param>
        /// <returns>The specified section.</returns>
        IEnumerable<double> GetWindowedSamples(int start, int length);

        /// <summary>
        ///     Gets a section of the signal in time domain.
        /// </summary>
        /// <param name="start">The start of the section.</param>
        /// <param name="length">The length of the section.</param>
        /// <returns>The specified section.</returns>
        IFiniteSignal GetWindowedSignal(int start, int length);
    }
}