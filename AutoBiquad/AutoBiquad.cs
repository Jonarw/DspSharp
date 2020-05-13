using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using DspSharp.Algorithms;
using DspSharp.Filter;
using DspSharp.Filter.LtiFilters.Iir;
using DspSharp.Filter.LtiFilters.Primitive;

namespace AutoBiquad
{
    public class AutoBiquadModel
    {
        public double FlatnessTarget { get; set; } = 1;
        public double InitialStepSize { get; set; } = 2;
        public double MaxFilterError { get; set; } = 2;
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

            for (int i = 0; i < fdb.Count; i++)
            {
                if (Math.Abs(fdb[i]) > this.MaxFilterError && (Math.Sign(fdb[i]) == Math.Sign(originalSpectrum[i]) ||
                                                               Math.Abs(fdb[i]) - this.MaxFilterError > Math.Abs(originalSpectrum[i])))
                    return double.PositiveInfinity;
            }

            return fdb.Add(originalSpectrum).Rms();
        }

        public IReadOnlyList<double> GetFrequencies()
        {
            return SignalGenerators.LogSeries(this.RangeStart, this.RangeEnd, this.NumberOfPoints).ToReadOnlyList();
        }

        public IList<IFilter> MakeFilters(IReadOnlyList<double> x, IReadOnlyList<double> y)
        {
            var ret = new List<IFilter>();
            var targetX = this.GetFrequencies();
            var targetY = Interpolation.AdaptiveInterpolation(this.TargetX, this.TargetY, targetX, true, false).ToReadOnlyList();
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

        public static double[] Nlms(IReadOnlyList<double> parameters, Func<IReadOnlyList<double>, double> errorFunc, int iterations = 100, double gradientFactor = .001, double stepSize = .1, bool rollingUpdate = false)
        {
            var initialError = errorFunc(parameters);
            var ret = parameters.ToArray();
            var gradients = new double[parameters.Count];
            double previousError = initialError;

            for (int i = 0; i < iterations; i++)
            {
                var currentError = errorFunc(ret);
                Debug.Print($"Step {i}. Current Error: {currentError}, Previous Error: {previousError}");
                if (currentError > previousError)
                    break;

                for (int j = 0; j < parameters.Count; j++)
                {
                    var oldParameter = ret[j];
                    var gradientX = ret[j] * gradientFactor;
                    ret[j] += gradientX;
                    var gradientY = errorFunc(ret) - currentError;
                    gradients[j] = gradientY / gradientX;
                    ret[j] = oldParameter;

                    if (rollingUpdate)
                        ret[j] = ret[j] - gradients[j] * stepSize * currentError / initialError;
                }

                if (!rollingUpdate)
                {
                    for (int j = 0; j < parameters.Count; j++)
                        ret[j] = ret[j] - gradients[j] * stepSize * currentError / initialError;
                }

                previousError = currentError;
            }

            return ret;
        }

        private BiquadFilter MakeFilter(IReadOnlyList<double> x, IReadOnlyList<double> y)
        {
            var index = y.AbsMaxIndex();
            var frequency = x[index];
            var gain = -y[index];
            //gain += gain < 0 ? this.FlatnessTarget : -this.FlatnessTarget;
            var q = this.StartQ;
            var stepSize = this.InitialStepSize;

            var ret = new BiquadFilter(this.SampleRate, BiquadFilter.BiquadFilters.Peaking, frequency, q, gain);

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