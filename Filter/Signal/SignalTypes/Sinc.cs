using System;
using System.Numerics;
using Filter.Algorithms;
using Filter.Series;
using Filter.Spectrum;

namespace Filter.Signal
{
    public class Sinc : SyntheticSignal
    {
        public Sinc(double sampleRate, double frequency) : base(time => Dsp.Sinc(frequency * time / sampleRate), sampleRate)
        {
            if ((frequency < 0) || (frequency > sampleRate / 2))
            {
                throw new Exception();
            }

            var frequencies = new CustomSeries(new[] {0, frequency, frequency, sampleRate / 2});
            this.Spectrum = new Spectrum.Spectrum(frequencies, new Complex[] {1 / (2 * frequency), 1 / (2 * frequency), 0, 0});
            this.Name = "sinc, f = " + frequency;
        }

        public override ISpectrum Spectrum { get; }
    }
}