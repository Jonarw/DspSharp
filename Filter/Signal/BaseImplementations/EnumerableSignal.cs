using System.Collections.Generic;
using Filter.Extensions;
using Filter.Spectrum;
using PropertyTools.DataAnnotations;

namespace Filter.Signal
{
    /// <summary>
    ///     Represents a digital signal that is representable in time domain with a fixed starting time. The signal can still
    ///     be infinitely long in the positive direction, but not in the negative direction.
    /// </summary>
    /// <seealso cref="Filter.Signal.SignalBase" />
    /// <seealso cref="Filter.Signal.IEnumerableSignal" />
    public class EnumerableSignal : SignalBase, IEnumerableSignal
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumerableSignal" /> class.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="start">The start sample time of the signal.</param>
        public EnumerableSignal(IEnumerable<double> signal, double sampleRate, int start = 0) : base(sampleRate)
        {
            this.Signal = signal;
            this.Start = start;
            this.DisplayName = "enumerable signal";
        }

        /// <summary>
        ///     Computes the spectrum.
        /// </summary>
        /// <param name="fftLength">Length of the FFT used to compute the spectrum.</param>
        /// <returns>
        ///     The spectrum.
        /// </returns>
        public IFftSpectrum GetSpectrum(int fftLength)
        {
            var signal = this.Signal.ToReadOnlyList(fftLength);
            return new FftSpectrum(signal, fftLength, this.SampleRate, this.Start);
        }

        /// <summary>
        ///     Gets the signal in time domain.
        /// </summary>
        public IEnumerable<double> Signal { get; }

        /// <summary>
        ///     Gets a section of the signal in time domain.
        /// </summary>
        /// <param name="start">The start of the section.</param>
        /// <param name="length">The length of the section.</param>
        /// <returns>
        ///     The specified section.
        /// </returns>
        public override IEnumerable<double> GetWindowedSignal(int start, int length)
        {
            return this.Signal.GetPaddedRange(start - this.Start, length);
        }

        [Category("enumerable signal")]
        [DisplayName("start time")]
        public int Start { get; }
    }
}