// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFftProvider.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Numerics;

namespace DspSharp.Algorithms
{

    /// <summary>
    /// Defines an interface for FFT-Algorithms required by this library.
    /// </summary>
    public interface IFftProvider
    {
        /// <summary>
        /// Computes the FFT of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <returns>The FFT of the sequence.</returns>
        Complex[] ComplexFft(IReadOnlyList<Complex> input);

        /// <summary>
        /// Computes the IFFT of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <returns>The IFFT of the sequence.</returns>
        Complex[] ComplexIfft(IReadOnlyList<Complex> input);

        /// <summary>
        ///     Computes the FFT of the real valued input vector.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <returns>The positive half of the hermitian-symmetric spectrum corresponding to the input vector.</returns>
        Complex[] RealFft(IReadOnlyList<double> input);

        /// <summary>
        ///     Computes the IFFT of the positive half of a hermitian-symmetric spectrum.
        /// </summary>
        /// <param name="input">The positive half of a hermitian-symmetric spectrum.</param>
        /// <param name="isEven">A value indicating whether the time domain signal corresponding to the spectrum is even-length or not.</param>
        /// <returns>The real-valued time signal corresponding to the input sequence.</returns>
        double[] RealIfft(IReadOnlyList<Complex> input, bool isEven);
    }
}