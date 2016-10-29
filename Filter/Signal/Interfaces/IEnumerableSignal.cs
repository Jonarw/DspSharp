using System.Collections.Generic;
using Filter.Spectrum;

namespace Filter.Signal
{
    public interface IEnumerableSignal : ISignal
    {
        IEnumerable<double> Signal { get; }
        int Start { get; }
        IFftSpectrum GetSpectrum(int fftLength);
    }
}