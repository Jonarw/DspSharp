using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DspSharp.Algorithms;
using DspSharp.Filter;
using DspSharp.Filter.LtiFilters.Iir;
using DspSharp.Filter.LtiFilters.Primitive;

namespace DspSharpDemo
{
    public class AutoBiquad
    {
        public double FlatnessTarget { get; set; } = 1;
        public double InitialStepSize { get; set; } = 2;

        public double MaxGain { get; set; } = 6;
        public int MaxStages { get; set; } = 10;
        public int NumberOfPoints { get; set; } = 500;
        public int QStages { get; set; } = 10;
        public double RangeEnd { get; set; } = 20000;
        public double RangeStart { get; set; } = 20;
        public double SampleRate { get; set; } = 48000;
        public double StartQ { get; set; } = 100;
        public IReadOnlyList<double> TargetX { get; private set; } = new[] {20, 20000d};
        public IReadOnlyList<double> TargetY { get; private set; } = new[] {0, 0d};

        public double GetError(IEnumerable<Complex> filterSpectrum, IReadOnlyList<double> originalSpectrum)
        {
            var fdb = FrequencyDomain.LinearToDb(filterSpectrum.Magitude()).ToReadOnlyList();

            if (fdb.Where((t, i) => Math.Abs(t) > this.FlatnessTarget && Math.Sign(t) == Math.Sign(originalSpectrum[i])).Any())
            {
                return double.PositiveInfinity;
            }

            return fdb.Add(originalSpectrum).Rms();
        }

        public IList<IFilter> MakeFilters(IReadOnlyList<double> x, IReadOnlyList<double> y)
        {
            var ret = new List<IFilter>();
            var targetX = SignalGenerators.LogSeries(this.RangeStart, this.RangeEnd, this.NumberOfPoints).ToReadOnlyList();
            var targetY = Interpolation.AdaptiveInterpolation(this.TargetX, this.TargetY, targetX, true, false);
            var originalY = Interpolation.AdaptiveInterpolation(x, y, targetX, true, false);

            var difference = originalY.Subtract(targetY).ToReadOnlyList();
            var min = difference.Min();
            var gain = -min - this.MaxGain;
            ret.Add(new GainFilter(this.SampleRate) {Gain = FrequencyDomain.DbToLinear(gain)});
            var currentY = difference.Add(gain).ToReadOnlyList();

            for (int i = 0; i < this.MaxStages; i++)
            {
                if (currentY.AbsMax() < this.FlatnessTarget)
                    break;

                var newFilter = this.MakeFilter(targetX, currentY);
                ret.Add(newFilter);
                currentY = currentY.Add(FrequencyDomain.LinearToDb(newFilter.GetFrequencyResponse(targetX).Magitude())).ToReadOnlyList();
            }

            return ret;
        }

        public void SetTarget(IReadOnlyList<double> x, IReadOnlyList<double> y)
        {
            if (x.Count != y.Count)
                throw new ArgumentException("Target X and Y must be the same length.");

            this.TargetX = x;
            this.TargetY = y;
        }

        private BiquadFilter MakeFilter(IReadOnlyList<double> x, IReadOnlyList<double> y)
        {
            var index = y.AbsMaxIndex();
            var frequency = x[index];
            var gain = -y[index];
            //gain += gain < 0 ? this.FlatnessTarget : -this.FlatnessTarget;
            var q = this.StartQ;
            var stepSize = this.InitialStepSize;

            var ret = new BiquadFilter(this.SampleRate, BiquadFilter.BiquadFilterType.Peaking, frequency, q, gain);

            var error = this.GetError(ret.GetFrequencyResponse(x), y);

            //while (true)
            //{
            //    ret.Q = q * stepSize;
            //    var newError = this.GetError(ret.GetFrequencyResponse(x), y);
            //    if (newError < error)
            //    {
            //        error = newError;
            //        q = ret.Q;
            //    }
            //    else
            //        break;
            //}

            //q = this.StartQ;
            while (true)
            {
                ret.Q = q / stepSize;
                var newError = this.GetError(ret.GetFrequencyResponse(x), y);
                if (newError < error)
                {
                    error = newError;
                    q = ret.Q;
                }
                else
                    break;
            }

            for (int i = 0; i < this.QStages; i++)
            {
                stepSize = Math.Sqrt(stepSize);

                ret.Q = q * stepSize;
                var newError = this.GetError(ret.GetFrequencyResponse(x), y);
                if (newError < error)
                {
                    error = newError;
                    q = ret.Q;
                }
                else
                {
                    ret.Q = q / stepSize;
                    newError = this.GetError(ret.GetFrequencyResponse(x), y);
                    if (newError < error)
                    {
                        error = newError;
                        q = ret.Q;
                    }
                }
            }

            return ret;
        }
    }
}