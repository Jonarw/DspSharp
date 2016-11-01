using System.Collections.Generic;
using Filter.Spectrum;

namespace Filter.Signal
{
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