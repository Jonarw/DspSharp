// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFiniteSignal.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp.Spectrum;

namespace DspSharp.Signal
{
    /// <summary>
    ///     Describes a digital signal that is representable in time domain with a finite number of time samples.
    /// </summary>
    public interface IFiniteSignal : IEnumerableSignal
    {
        /// <summary>
        ///     Gets the length.
        /// </summary>
        int Length { get; }

        /// <summary>
        ///     Gets the signal.
        /// </summary>
        new IReadOnlyList<double> Signal { get; }

        /// <summary>
        ///     Gets the spectrum.
        /// </summary>
        IFftSpectrum Spectrum { get; }

        /// <summary>
        ///     Gets the index of the sample following the last sample.
        /// </summary>
        int Stop { get; }

        /// <summary>
        ///     Gets the sample at the specified time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>The sample.</returns>
        double GetSample(int time);
    }
}