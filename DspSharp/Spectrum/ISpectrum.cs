// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISpectrum.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Numerics;
using DspSharp.Series;

namespace DspSharp.Spectrum
{
    /// <summary>
    ///     Describes the spectrum of a digital signal.
    /// </summary>
    public interface ISpectrum
    {
        /// <summary>
        ///     Gets the frequencies where the spectrum is defined.
        /// </summary>
        ISeries Frequencies { get; }

        /// <summary>
        ///     Gets the group delay.
        /// </summary>
        IReadOnlyList<double> GroupDelay { get; }

        /// <summary>
        ///     Gets the magnitude.
        /// </summary>
        IReadOnlyList<double> Magnitude { get; }

        /// <summary>
        ///     Gets the phase.
        /// </summary>
        IReadOnlyList<double> Phase { get; }

        /// <summary>
        ///     Gets the complex values.
        /// </summary>
        IReadOnlyList<Complex> Values { get; }

        /// <summary>
        ///     Gets the value at the specified frequency.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        /// <returns></returns>
        Complex GetValue(double frequency);
    }
}