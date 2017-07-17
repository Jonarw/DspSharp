// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFftProvider.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Numerics;

namespace DspSharp.Algorithms
{
    public enum NormalizationKind
    {
        None,
        N,
        SqrtN
    }

    /// <summary>
    ///     Defines an interface for FFT-Algorithms required by this library.
    /// </summary>
    public interface IFftProvider
    {
        /// <summary>
        ///     Computes the FFT of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="n">The FFT length. The input vector is automatically truncated or zero-padded to this length.</param>
        /// <param name="normalization">The normalization to be applied after transforming.</param>
        /// <returns>The FFT of the sequence.</returns>
        IReadOnlyList<Complex> ComplexFft(IReadOnlyList<Complex> input, int n = -1, NormalizationKind normalization = NormalizationKind.None);

        /// <summary>
        ///     Computes the IFFT of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <returns>The IFFT of the sequence.</returns>
        /// <param name="normalization">The normalization to be applied after transforming.</param>
        IReadOnlyList<Complex> ComplexIfft(IReadOnlyList<Complex> input, NormalizationKind normalization = NormalizationKind.N);

        /// <summary>
        ///     Gets the length of next biggest FFT length that the FFT provider can compute efficiently.
        /// </summary>
        /// <param name="originalLength">The original length.</param>
        /// <returns>The most efficient fft length greater or equal to the original length.</returns>
        int GetOptimalFftLength(int originalLength);

        /// <summary>
        ///     Computes the FFT of the real valued input vector.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="n">The FFT length. The input vector is automatically truncated or zero-padded to this length.</param>
        /// <returns>The positive half of the hermitian-symmetric spectrum corresponding to the input vector.</returns>
        /// <param name="normalization">The normalization to be applied after transforming.</param>
        IReadOnlyList<Complex> RealFft(IReadOnlyList<double> input, int n = -1, NormalizationKind normalization = NormalizationKind.None);

        /// <summary>
        ///     Computes the IFFT of the positive half of a hermitian-symmetric spectrum.
        /// </summary>
        /// <param name="input">The positive half of a hermitian-symmetric spectrum.</param>
        /// <returns>The real-valued time signal corresponding to the input sequence.</returns>
        /// <param name="normalization">The normalization to be applied after transforming.</param>
        IReadOnlyList<double> RealIfft(IReadOnlyList<Complex> input, int n = -1, NormalizationKind normalization = NormalizationKind.N);
    }
}