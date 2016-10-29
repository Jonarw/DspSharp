using Filter.Spectrum;

namespace Filter.Signal
{
    public interface ISyntheticSignal : ISignal
    {
        ISpectrum Spectrum { get; }
    }
}