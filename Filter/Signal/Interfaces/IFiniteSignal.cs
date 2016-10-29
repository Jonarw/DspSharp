using System.Collections.Generic;
using Filter.Spectrum;

namespace Filter.Signal
{
    public interface IFiniteSignal : IEnumerableSignal
    {
        int Length { get; }
        new IReadOnlyList<double> Signal { get; }
        int Stop { get; }
        double GetSample(int time);
        IFftSpectrum Spectrum { get; }
    }
}