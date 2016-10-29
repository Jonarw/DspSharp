using System;
using System.Numerics;
using Filter.Series;
using Filter.Spectrum;

namespace Filter.Signal
{
    public class Sinus : SyntheticSignal
    {
        public Sinus(double sampleRate, double frequency, double phaseOffset = 0)
            : base(time => Math.Sin(2 * Math.PI * time * frequency / sampleRate + phaseOffset), sampleRate)
        {
            if ((frequency < 0) || (frequency > sampleRate / 2))
            {
                throw new Exception();
            }

            var frequencies = new CustomSeries(new[] {0, frequency, frequency, frequency, sampleRate / 2});
            this.Spectrum = new Spectrum.Spectrum(frequencies, new Complex[] {0, 0, double.PositiveInfinity, 0, 0});
            this.Name = "sinus, f = " + frequency;
        }

        public override ISpectrum Spectrum { get; }
    }
}