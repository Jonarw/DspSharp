// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyntheticSignal.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Spectrum;

namespace DspSharp.Signal
{
    /// <summary>
    ///     Describes a digital signal representable in time domain with a known and analytically calculable spectrum.
    /// </summary>
    public interface ISyntheticSignal : ISignal
    {
        /// <summary>
        ///     Gets the spectrum.
        /// </summary>
        ISpectrum Spectrum { get; }
    }
}