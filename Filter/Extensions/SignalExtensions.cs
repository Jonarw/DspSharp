using System;
using System.Collections.Generic;
using System.Linq;
using Filter.Algorithms;
using Filter.Exceptions;
using Filter.Signal;

namespace Filter.Extensions
{
    /// <summary>
    ///     Provides static extension methods for the Signal interfaces.
    /// </summary>
    public static class SignalExtensions
    {
        /// <summary>
        ///     Adds the specified signals.
        /// </summary>
        /// <param name="s1">The first signal.</param>
        /// <param name="s2">The second signal.</param>
        /// <returns></returns>
        /// <exception cref="SamplerateMismatchException"></exception>
        public static ISignal Add(this ISignal s1, ISignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new SamplerateMismatchException();
            }

            return new InfiniteSignal((start, length) => s1.GetWindowedSignal(start, length).Add(s2.GetWindowedSignal(start, length)), s1.SampleRate)
            {
                DisplayName = "addition result"
            };
        }

        /// <summary>
        ///     Adds the specified finite signals.
        /// </summary>
        /// <param name="s1">The first signal.</param>
        /// <param name="s2">The second signal.</param>
        /// <returns></returns>
        /// <exception cref="SamplerateMismatchException"></exception>
        public static IFiniteSignal Add(this IFiniteSignal s1, IFiniteSignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new SamplerateMismatchException();
            }

            return new FiniteSignal(
                s1.Signal.AddFullWithOffset(s2.Signal, s2.Start - s1.Start).ToReadOnlyList(),
                s1.SampleRate,
                Math.Min(s1.Start, s2.Start)) {DisplayName = "addition result"};
        }

        /// <summary>
        ///     Convolves the specified finite signals.
        /// </summary>
        /// <param name="s1">The first signal.</param>
        /// <param name="s2">The second signal.</param>
        /// <returns></returns>
        /// <exception cref="SamplerateMismatchException"></exception>
        public static IFiniteSignal Convolve(this IFiniteSignal s1, IFiniteSignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new SamplerateMismatchException();
            }

            return new FiniteSignal(Dsp.Convolve(s1.Signal, s2.Signal), s1.SampleRate, s1.Start + s2.Start) {DisplayName = "convolution result"};
        }

        /// <summary>
        ///     Convolves the specified finite signal with an enumerable signal.
        /// </summary>
        /// <param name="s1">The finite signal.</param>
        /// <param name="s2">The enumerable signal.</param>
        /// <returns></returns>
        /// <exception cref="SamplerateMismatchException"></exception>
        public static IEnumerableSignal Convolve(this IFiniteSignal s1, IEnumerableSignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new SamplerateMismatchException();
            }

            return new EnumerableSignal(Dsp.Convolve(s2.Signal, s1.Signal), s1.SampleRate, s1.Start + s2.Start) {DisplayName = "convolution result"};
        }

        /// <summary>
        ///     Convolves the specified finite signal with an infinite signal.
        /// </summary>
        /// <param name="s1">The finite signal.</param>
        /// <param name="s2">The infinite signal.</param>
        /// <returns></returns>
        /// <exception cref="SamplerateMismatchException"></exception>
        public static ISignal Convolve(this IFiniteSignal s1, ISignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new SamplerateMismatchException();
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
                s1.SampleRate) {DisplayName = "convolution result"};
        }

        /// <summary>
        ///     Computes the cross-correlation of two finite signals.
        /// </summary>
        /// <param name="s1">The first signal.</param>
        /// <param name="s2">The second signal.</param>
        /// <returns></returns>
        /// <exception cref="SamplerateMismatchException"></exception>
        public static IFiniteSignal CrossCorrelate(this IFiniteSignal s1, IFiniteSignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new SamplerateMismatchException();
            }

            return new FiniteSignal(Dsp.CrossCorrelate(s1.Signal, s2.Signal), s1.SampleRate, s1.Start - s2.Start) {DisplayName = "cross correlation"};
        }

        /// <summary>
        ///     Computes the cross-correlation of a finite and a enumerable signal.
        /// </summary>
        /// <param name="s1">The finite signal.</param>
        /// <param name="s2">The enumerable signal.</param>
        /// <returns></returns>
        /// <exception cref="SamplerateMismatchException"></exception>
        public static IEnumerableSignal CrossCorrelate(this IFiniteSignal s1, IEnumerableSignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new SamplerateMismatchException();
            }

            return new EnumerableSignal(Dsp.CrossCorrelate(s2.Signal, s1.Signal), s1.SampleRate, s1.Start - s2.Start)
            {
                DisplayName = "cross correlation"
            };
        }

        /// <summary>
        ///     Multiplies the specified finite signal with a signal.
        /// </summary>
        /// <param name="s1">The finite signal.</param>
        /// <param name="s2">The second signal.</param>
        /// <returns></returns>
        /// <exception cref="SamplerateMismatchException"></exception>
        public static IFiniteSignal Multiply(this IFiniteSignal s1, ISignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new SamplerateMismatchException();
            }

            return new FiniteSignal(s1.Signal.Multiply(s2.GetWindowedSignal(s1.Start, s1.Length)).ToReadOnlyList(), s1.SampleRate, s1.Start)
            {
                DisplayName = "multiplication result"
            };
        }

        /// <summary>
        ///     Multiplies the specified signal with a finite signal.
        /// </summary>
        /// <param name="s1">The first signal.</param>
        /// <param name="s2">The finite signal.</param>
        /// <returns></returns>
        /// <exception cref="SamplerateMismatchException"></exception>
        public static IFiniteSignal Multiply(this ISignal s1, IFiniteSignal s2)
        {
            return s2.Multiply(s1);
        }

        /// <summary>
        ///     Multiplies the specified finite signals.
        /// </summary>
        /// <param name="s1">The first signal.</param>
        /// <param name="s2">The second signal.</param>
        /// <returns></returns>
        /// <exception cref="SamplerateMismatchException"></exception>
        public static IFiniteSignal Multiply(this IFiniteSignal s1, IFiniteSignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new SamplerateMismatchException();
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

            return new FiniteSignal(signal, s1.SampleRate, start) {DisplayName = "multiplication result"};
        }

        /// <summary>
        ///     Multiplies the specified signals.
        /// </summary>
        /// <param name="s1">The first signal.</param>
        /// <param name="s2">The second signal.</param>
        /// <returns></returns>
        /// <exception cref="SamplerateMismatchException"></exception>
        public static ISignal Multiply(this ISignal s1, ISignal s2)
        {
            if (s1.SampleRate != s2.SampleRate)
            {
                throw new SamplerateMismatchException();
            }

            return new InfiniteSignal(
                (start, length) => s1.GetWindowedSignal(start, length).Multiply(s2.GetWindowedSignal(start, length)),
                s1.SampleRate) {DisplayName = "multiplication result"};
        }

        /// <summary>
        ///     Negates the specified finite signal.
        /// </summary>
        /// <param name="s">The signal.</param>
        /// <returns></returns>
        public static IFiniteSignal Negate(this IFiniteSignal s)
        {
            return new FiniteSignal(s.Signal.Negate().ToReadOnlyList(), s.SampleRate) {DisplayName = "negation result"};
        }

        /// <summary>
        ///     Negates the specified signal.
        /// </summary>
        /// <param name="s">The signal.</param>
        /// <returns></returns>
        public static ISignal Negate(this ISignal s)
        {
            return new InfiniteSignal((start, length) => s.GetWindowedSignal(start, length).Negate(), s.SampleRate) {DisplayName = "negation result"};
        }

        /// <summary>
        ///     Reverses the specified finite signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns></returns>
        public static IFiniteSignal Reverse(this IFiniteSignal signal)
        {
            return new FiniteSignal(signal.Signal.Reverse().ToReadOnlyList(), signal.SampleRate, signal.Start);
        }

        /// <summary>
        ///     Reverses the specified infinite signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns></returns>
        public static ISignal Reverse(this ISignal signal)
        {
            return new InfiniteSignal((start, length) => signal.GetWindowedSignal(-start - length + 1, length).Reverse(), signal.SampleRate);
        }
    }
}