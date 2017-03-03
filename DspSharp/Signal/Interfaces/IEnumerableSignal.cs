// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnumerableSignal.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp.Spectrum;

namespace DspSharp.Signal
{
    /// <summary>
    ///     Describes a digital signal that is representable in time domain with a fixed starting time. The signal can still be
    ///     infinitely long in the positive direction, but not in the negative direction.
    /// </summary>
    public interface IEnumerableSignal : ISignal
    {
        /// <summary>
        ///     Gets the signal in time domain.
        /// </summary>
        IEnumerable<double> Signal { get; }

        /// <summary>
        ///     Gets the start time of the signal.
        /// </summary>
        int Start { get; }

        /// <summary>
        ///     Computes the spectrum.
        /// </summary>
        /// <param name="fftLength">Length of the FFT used to compute the spectrum.</param>
        /// <returns>The spectrum.</returns>
        IFftSpectrum GetSpectrum(int fftLength);
    }
}