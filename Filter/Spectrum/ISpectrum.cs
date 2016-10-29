using System.Collections.Generic;
using System.Numerics;
using Filter.Series;

namespace Filter.Spectrum
{
    public interface ISpectrum
    {
        ISeries Frequencies { get; }
        IReadOnlyList<double> GroupDelay { get; }
        IReadOnlyList<double> Magnitude { get; }
        IReadOnlyList<double> Phase { get; }
        IReadOnlyList<Complex> Values { get; }
    }
}