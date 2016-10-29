using System;
using System.Collections.Generic;
using System.Linq;
using Filter.Algorithms;
using Filter.Extensions;

namespace Filter.Signal
{
    public static class SignalExtensions
    {
        public static ISignal Add(this ISignal s1, ISignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new Exception();
            }

            return new InfiniteSignal((start, length) => s1.GetWindowedSignal(start, length).Add(s2.GetWindowedSignal(start, length)), s1.SampleRate)
            {
                Name = "addition result"
            };
        }

        public static IFiniteSignal Add(this IFiniteSignal s1, IFiniteSignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new Exception();
            }

            return new FiniteSignal(
                s1.Signal.AddFullWithOffset(s2.Signal, s2.Start - s1.Start).ToReadOnlyList(),
                s1.SampleRate,
                Math.Min(s1.Start, s2.Start)) {Name = "addition result"};
        }

        public static IEnumerableSignal ApplyFilter(this IEnumerableSignal input, IFilter filter)
        {
            if (input.SampleRate != filter.Samplerate)
            {
                throw new Exception();
            }

            if (input is FiniteSignal && !filter.HasInfiniteImpulseResponse)
            {
                return new FiniteSignal(filter.Process(input.Signal).ToReadOnlyList(), input.SampleRate, input.Start) {Name = "filter result"};
            }

            return new EnumerableSignal(filter.Process(input.Signal), input.SampleRate, input.Start) {Name = "filter result"};
        }

        public static IFiniteSignal Convolve(this IFiniteSignal s1, IFiniteSignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new Exception();
            }

            var l = s1.Length + s2.Length - 1;
            var n = Fft.NextPowerOfTwo(l);
            var spectrum1 = Fft.RealFft(s1.Signal, n);
            var spectrum2 = Fft.RealFft(s2.Signal, n);
            var spectrum = spectrum2.Multiply(spectrum1);
            var signal = Fft.RealIfft(spectrum).Take(l);

            return new FiniteSignal(signal.ToReadOnlyList(), s1.SampleRate, s1.Start + s2.Start) {Name = "convolution result"};
        }

        public static ISignal Convolve(this IFiniteSignal s1, ISignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new Exception();
            }

            return new InfiniteSignal(
                (start, length) =>
                {
                    var l = s1.Length + Math.Max(s1.Length, length) - 1;
                    var n = Fft.NextPowerOfTwo(l);

                    var spectrum1 = Fft.RealFft(s1.Signal, n);

                    var signal2A = s2.GetWindowedSignal(start - s1.Length - s1.Start, s1.Length);
                    var spectrum2A = Fft.RealFft(signal2A, n);
                    var signal2B = s2.GetWindowedSignal(start - s1.Start, length);
                    var spectrum2B = Fft.RealFft(signal2B, n);

                    var spectrumA = spectrum1.Multiply(spectrum2A);
                    var spectrumB = spectrum1.Multiply(spectrum2B);

                    var signalA = Fft.RealIfft(spectrumA).Skip(s1.Length).Take(Math.Min(length, s1.Length - 1));
                    var signalB = Fft.RealIfft(spectrumB).Take(length);

                    var signal = signalA.AddFull(signalB);

                    return signal;
                },
                s1.SampleRate) {Name = "convolution result"};
        }

        public static IFiniteSignal Multiply(this IFiniteSignal s1, ISignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new Exception();
            }

            return new FiniteSignal(s1.Signal.Multiply(s2.GetWindowedSignal(s1.Start, s1.Length)).ToReadOnlyList(), s1.SampleRate, s1.Start)
            {
                Name = "multiplication result"
            };
        }

        public static IFiniteSignal Multiply(this ISignal s1, IFiniteSignal s2)
        {
            return s2.Multiply(s1);
        }

        public static IFiniteSignal Multiply(this IFiniteSignal s1, IFiniteSignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new Exception();
            }

            int start;
            IReadOnlyList<double> signal;
            if (s1.Start < s2.Start)
            {
                start = s2.Start;
                signal = s1.Signal.Skip(s2.Start - s1.Start).Multiply(s2.Signal).ToReadOnlyList();
            }
            else
            {
                start = s1.Start;
                signal = s2.Signal.Skip(s1.Start - s2.Start).Multiply(s1.Signal).ToReadOnlyList();
            }

            return new FiniteSignal(signal, s1.SampleRate, start) {Name = "multiplication result"};
        }

        public static ISignal Multiply(this ISignal s1, ISignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new Exception();
            }

            return new InfiniteSignal(
                (start, length) => s1.GetWindowedSignal(start, length).Multiply(s2.GetWindowedSignal(start, length)),
                s1.SampleRate) {Name = "multiplication result"};
        }

        public static IFiniteSignal Negate(this IFiniteSignal s)
        {
            return new FiniteSignal(s.Signal.Negate().ToReadOnlyList(), s.SampleRate) {Name = "negation result"};
        }

        public static ISignal Negate(this ISignal s)
        {
            return new InfiniteSignal((start, length) => s.GetWindowedSignal(start, length).Negate(), s.SampleRate) {Name = "negation result"};
        }
    }
}