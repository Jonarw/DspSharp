using System;
using System.Collections.Generic;
using Filter.Spectrum;

namespace Filter.Signal
{
    public abstract class SyntheticSignal : InfiniteSignal, ISyntheticSignal
    {
        protected SyntheticSignal(Func<int, double> sampleFunction, double sampleRate) : base(sampleFunction, sampleRate)
        {
            this.Name = "synthetic signal";
        }

        protected SyntheticSignal(Func<int, int, IEnumerable<double>> timeDomainFunction, double sampleRate) : base(timeDomainFunction, sampleRate)
        {
            this.Name = "synthetic signal";
        }

        public abstract ISpectrum Spectrum { get; }
    }
}