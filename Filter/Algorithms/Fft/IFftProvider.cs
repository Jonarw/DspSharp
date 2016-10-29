using System.Collections.Generic;
using System.Numerics;

namespace Filter.Algorithms
{
    /// <summary>
    ///     Defines an interface for FFT-Algorithms required by this library.
    /// </summary>
    public interface IFftProvider
    {
        /// <summary>
        ///     Computes the FFT over the real valued input vector.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="n">The FFT length. The input vector is automatically truncated or zero-padded to this length.</param>
        /// <returns>The positive half of the hermitian-symmetric spectrum corresponding to the input vector.</returns>
        IReadOnlyList<Complex> RealFft(IReadOnlyList<double> input, int n = -1);

        /// <summary>
        ///     Computes the IFFT over the positive half of a hermitian-symmetric spectrum.
        /// </summary>
        /// <param name="input">The positive half of a hermitian-symmetric spectrum.</param>
        /// <returns>The real-valued time signal corresponding to the input vector.</returns>
        IReadOnlyList<double> RealIfft(IReadOnlyList<Complex> input);
    }
}