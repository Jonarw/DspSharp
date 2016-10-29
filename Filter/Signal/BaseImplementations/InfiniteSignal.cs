using System;
using System.Collections.Generic;

namespace Filter.Signal
{
    public class InfiniteSignal : ISignal
    {
        public InfiniteSignal(Func<int, double> sampleFunction, double sampleRate)
        {
            this.SampleFunction = sampleFunction;
            this.SampleRate = sampleRate;
            this.TimeDomainFunction = this.GetTimeDomainFunction;
        }

        public InfiniteSignal(Func<int, int, IEnumerable<double>> timeDomainFunction, double sampleRate)
        {
            this.TimeDomainFunction = timeDomainFunction;
            this.SampleRate = sampleRate;
        }

        private IEnumerable<double> GetTimeDomainFunction(int start, int length)
        {
            for (int i = start; i < length; i++)
            {
                yield return this.SampleFunction.Invoke(i);
            }
        }

        public Func<int, int, IEnumerable<double>> TimeDomainFunction { get; set; }

        public IEnumerable<double> GetWindowedSignal(int start, int length)
        {
            return this.TimeDomainFunction.Invoke(start, length);
        }

        public Func<int, double> SampleFunction { get; set; }
        public double SampleRate { get; }
        public string Name { get; set; } = "infinite signal";
    }
}