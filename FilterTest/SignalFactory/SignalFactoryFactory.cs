using System;

namespace FilterTest.SignalFactory
{
    public static class SignalFactoryFactory
    {
        public static SignalFactory CreateSignalFactory(AvailableSignals type, double samplerate)
        {
            if (type == AvailableSignals.Sinus)
            {
                return new SinusFactory {SampleRate = samplerate};
            }
            if (type == AvailableSignals.Sinc)
            {
                return new SincFactory {SampleRate = samplerate};
            }
            if (type == AvailableSignals.WhiteNoise)
            {
                return new WhiteNoiseFactory {SampleRate = samplerate};
            }
            if (type == AvailableSignals.IdealHighpass)
            {
                return new IdealHighpassFactory {SampleRate = samplerate};
            }
            if (type == AvailableSignals.IdealLowpass)
            {
                return new IdealLowpassFactory {SampleRate = samplerate};
            }
            if (type == AvailableSignals.Dirac)
            {
                return new DiracFactory {SampleRate = samplerate};
            }
            if (type == AvailableSignals.LogSweep)
            {
                return new LogSweepFactory {SampleRate = samplerate};
            }
            if (type == AvailableSignals.Window)
            {
                return new WindowFactory {SampleRate = samplerate};
            }
            if (type == AvailableSignals.FileImpulseResponse)
            {
                return new FileImpulseResponseFactory {SampleRate = samplerate};
            }
            if (type == AvailableSignals.FileFrequencyResponse)
            {
                return new FileFrequencyResponseFactory {SampleRate = samplerate};
            }
            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}