using System;
using System.Collections.Generic;
using Filter.Algorithms;
using Filter.Extensions;

namespace Filter.Signal
{
    public class WhiteNoise : ISignal
    {
        public double Mean { get; }
        public double Variance { get; }

        public WhiteNoise(double sampleRate, double mean = 0, double variance = 1)
        {
            this.SampleRate = sampleRate;
            this.Mean = mean;
            this.Sigma = Math.Sqrt(variance);
            this.Variance = variance;
            this.NoiseSource = Dsp.WhiteNoise(unchecked((int)DateTime.Now.Ticks)).GetEnumerator();
            this.Name = "white noise, µ = " + mean + ",σ² = " + variance;
        }

        private double Sigma { get; }

        private List<double> Cache { get; set; }
        private int CacheStart { get; set; }
        private int CacheEnd { get; set; }
        private IEnumerator<double> NoiseSource { get; }

        private List<double> GenerateNoise(int length)
        {
            var ret = new List<double>(length);
            for (int i = 0; i < length; i++)
            {
                this.NoiseSource.MoveNext();
                ret.Add(this.NoiseSource.Current * this.Sigma + this.Mean);
            }

            return ret;
        } 

        public IEnumerable<double> GetWindowedSignal(int start, int length)
        {
            if (this.CacheStart == this.CacheEnd)
            {
                this.Cache = this.GenerateNoise(length);
                this.CacheStart = start;
                this.CacheEnd = start + length;
            }
            else
            {
                if (start < this.CacheStart)
                {
                    this.Cache.InsertRange(0, this.GenerateNoise(this.CacheStart - start));
                    this.CacheStart = start;
                }

                if (length + start > this.CacheEnd)
                {
                    this.Cache.AddRange(this.GenerateNoise(length + start - this.CacheEnd));
                    this.CacheEnd = length + start;
                }
            }

            return this.Cache.GetRangeOptimized(start - this.CacheStart, length);
        }

        public double SampleRate { get; }
        public string Name { get; set; }
    }
}