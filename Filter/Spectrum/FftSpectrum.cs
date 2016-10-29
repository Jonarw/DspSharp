using System.Collections.Generic;
using System.Numerics;
using Filter.Algorithms;
using Filter.Extensions;
using Filter.Series;

namespace Filter.Spectrum
{
    public class FftSpectrum : Spectrum, IFftSpectrum
    {
        public FftSpectrum(FftSeries frequencies, IReadOnlyList<Complex> values) : base(frequencies, values)
        {
            this.Frequencies = frequencies;
        }

        public FftSpectrum(IEnumerable<double> timeSignal, int fftLength, double sampleRate, int start)
            : this(new FftSeries(sampleRate, fftLength), Fft.RealFft(timeSignal.CircularShift(start), fftLength))
        {
        }

        public FftSpectrum(IReadOnlyList<double> timeSignal, double sampleRate, int start)
            : this(new FftSeries(sampleRate, timeSignal.Count), Fft.RealFft(timeSignal.CircularShift(start)))
        {
            this.TimeDomainSignal = timeSignal.CircularShift(start).ToReadOnlyList();
        }

        public new FftSeries Frequencies { get; }

        private IReadOnlyList<double> TimeDomainSignal { get; set; }

        public IReadOnlyList<double> GetTimeDomainSignal()
        {
            return this.TimeDomainSignal ?? (this.TimeDomainSignal = Fft.RealIfft(this.Values));
        } 

    }
}