// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignalFactoryFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace DspSharpDemo.ViewModel.Signal
{
    public static class SignalFactoryFactory
    {
        public static SignalFactories.SignalFactory CreateSignalFactory(SignalType type, double samplerate)
        {
            switch (type)
            {
            case SignalType.Sine:
                return new SignalFactories.SinusFactory (samplerate);
            case SignalType.Sinc:
                return new SignalFactories.SincFactory(samplerate);
            case SignalType.WhiteNoise:
                return new SignalFactories.WhiteNoiseFactory(samplerate);
            case SignalType.IdealHighpass:
                return new SignalFactories.IdealHighpassFactory(samplerate);
            case SignalType.IdealLowpass:
                return new SignalFactories.IdealLowpassFactory(samplerate);
            case SignalType.Dirac:
                return new SignalFactories.DiracFactory(samplerate);
            case SignalType.LogSweep:
                return new SignalFactories.LogSweepFactory(samplerate);
            case SignalType.Window:
                return new SignalFactories.WindowFactory(samplerate);
            case SignalType.FileImpulseResponse:
                return new SignalFactories.FileImpulseResponseFactory(samplerate);
            case SignalType.FileFrequencyResponse:
                return new SignalFactories.FileFrequencyResponseFactory(samplerate);
            }
            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}