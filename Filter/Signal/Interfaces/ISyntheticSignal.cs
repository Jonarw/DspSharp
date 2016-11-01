using Filter.Spectrum;

namespace Filter.Signal
{
    public interface ISyntheticSignal : ISignal
    {
        /// <summary>
        ///     Gets the spectrum.
        /// </summary>
        ISpectrum Spectrum { get; }
    }
}