using System.Collections.Generic;
using Filter.Series;

namespace Filter.Spectrum
{
    public interface IFftSpectrum : ISpectrum
    {
        new FftSeries Frequencies { get; }
        IReadOnlyList<double> GetTimeDomainSignal();
    }
}