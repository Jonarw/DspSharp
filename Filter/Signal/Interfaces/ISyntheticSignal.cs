using Filter.Spectrum;

namespace Filter.Signal
{
    /// <summary>
    ///     Describes a digital signal representable in time domain with a known and analytically calculable spectrum.
    /// </summary>
    /// <seealso cref="Filter.Signal.ISignal" />
    public interface ISyntheticSignal : ISignal
    {
        /// <summary>
        ///     Gets the spectrum.
        /// </summary>
        ISpectrum Spectrum { get; }
    }
}