using System;

namespace FilterTest.SignalFactory
{
    public static class SignalFactoryFactory
    {
        public static SignalFactory CreateSignalFactory(AvailableSignals type)
        {
            switch (type)
            {
            case AvailableSignals.Sinus:
                return new SinusFactory();
            case AvailableSignals.Sinc:
                return new SincFactory();
            case AvailableSignals.WhiteNoise:
                return new WhiteNoiseFactory();
            case AvailableSignals.IdealHighpass:
                return new IdealHighpassFactory();
            case AvailableSignals.IdealLowpass:
                return new IdealLowpassFactory();
            case AvailableSignals.Dirac:
                return new DiracFactory();
            case AvailableSignals.LogSweep:
                return new LogSweepFactory();
            case AvailableSignals.Window:
                return new WindowFactory();
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}