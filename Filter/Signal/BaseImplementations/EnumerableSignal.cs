using System.Collections.Generic;
using Filter.Extensions;
using Filter.Spectrum;

namespace Filter.Signal
{
    public class EnumerableSignal : IEnumerableSignal
    {
        public EnumerableSignal(IEnumerable<double> signal, double sampleRate, int start = 0)
        {
            this.Signal = signal;
            this.SampleRate = sampleRate;
            this.Start = start;
            this.Name = "enumerable signal";
        }

        public IFftSpectrum GetSpectrum(int fftLength)
        {
            var signal = this.Signal.ToReadOnlyList(fftLength);
            return new FftSpectrum(signal, fftLength, this.Start);
        }

        public IEnumerable<double> Signal { get; }

        public int Start { get; }

        public IEnumerable<double> GetWindowedSignal(int start, int length)
        {
            return this.Signal.GetPaddedRange(start - this.Start, length);
        }

        public string Name { get; set; }

        public double SampleRate { get; }
    }
}