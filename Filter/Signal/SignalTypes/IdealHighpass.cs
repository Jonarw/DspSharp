using System;
using System.Numerics;
using Filter.Algorithms;
using Filter.Series;
using Filter.Spectrum;

namespace Filter.Signal
{
    public class IdealHighpass : SyntheticSignal
    {
        public IdealHighpass(double sampleRate, double fc)
            : base(time => (time == 0 ? 1 : 0) - Dsp.Sinc(fc * time / sampleRate) * (2 * fc / sampleRate), sampleRate)
        {
            if ((fc < 0) || (fc > sampleRate / 2))
            {
                throw new Exception();
            }

            var frequencies = new CustomSeries(new[] {0, 20, fc, fc, sampleRate / 2});
            this.Spectrum = new Spectrum.Spectrum(frequencies, new Complex[] {0, 0, 0, 1, 1});
            this.Name = "ideal highpass, fc = " + fc;
        }

        public override ISpectrum Spectrum { get; }
    }
}