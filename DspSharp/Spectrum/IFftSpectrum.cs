using System.Collections.Generic;
using DspSharp.Series;

namespace DspSharp.Spectrum
{
    public interface IFftSpectrum : ISpectrum
    {
        new FftSeries Frequencies { get; }
        IReadOnlyList<double> GetTimeDomainSignal();
    }
}