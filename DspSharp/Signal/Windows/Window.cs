// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Window.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using DspSharp.Algorithms;
using PropertyTools.DataAnnotations;

namespace DspSharp.Signal.Windows
{
    /// <summary>
    ///     Represents a Window function, usually applied to another impulse response.
    /// </summary>
    public class Window : FiniteSignal
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Window" /> Class.
        /// </summary>
        /// <param name="type">The window type.</param>
        /// <param name="start">The start time of thw window.</param>
        /// <param name="length">The window length.</param>
        /// <param name="samplerate">The window samplerate.</param>
        /// <param name="mode">The window mode.</param>
        /// <param name="ratio">
        ///     The ratio of the window. Values can be between 0 and 1 where 1 is a "normal" window, 0 a
        ///     rectangular window and everyting between a tapered window.
        /// </param>
        public Window(WindowTypes type, int start, int length, double samplerate, WindowModes mode, double ratio = 1)
            : base(CreateWindow(type, mode, length, ratio), samplerate, start)
        {
            this.Type = type;
            this.Mode = mode;
            this.DisplayName = type + " " + mode + " window, length = " + length;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Window" /> Class. The window will have its maximum at time 0.
        /// </summary>
        /// <param name="type">The window type.</param>
        /// <param name="length">The window length.</param>
        /// <param name="samplerate">The window samplerate.</param>
        /// <param name="mode">The window mode.</param>
        /// <param name="ratio">
        ///     The ratio of the window. Values can be between 0 and 1 where 1 is a "normal" window, 0 a
        ///     rectangular window and everyting between a tapered window.
        /// </param>
        public Window(WindowTypes type, int length, double samplerate, WindowModes mode, double ratio = 1)
            : this(type, GetDefaultStart(mode, length), length, samplerate, mode, ratio)
        {
        }

        /// <summary>
        /// Creates the window.
        /// </summary>
        /// <param name="type">The window type.</param>
        /// <param name="length">The window length.</param>
        /// <param name="mode">The window mode.</param>
        /// <param name="ratio">
        ///     The ratio of the window. Values can be between 0 and 1 where 1 is a "normal" window, 0 a
        ///     rectangular window and everyting between a tapered window.
        /// </param>
        public static IReadOnlyList<double> CreateWindow(WindowTypes type, WindowModes mode, int length, double ratio)
        {
            var l = Convert.ToInt32(length * ratio);

            if (mode == WindowModes.Symmetric)
            {
                var hw = GetCausalHalfWindow(type, (l >> 1) + 1).ToReadOnlyList();
                var positivehw = hw.GetPaddedRange(1, ((l + 1) >> 1) - 1).ToReadOnlyList();
                return
                    hw.Reverse()
                        .Concat(Enumerable.Repeat(1.0, length - hw.Count - positivehw.Count))
                        .Concat(positivehw)
                        .ToReadOnlyList();
            }

            if (mode == WindowModes.Causal)
                return Enumerable.Repeat(1.0, length - l).Concat(GetCausalHalfWindow(type, l)).ToReadOnlyList();

            if (mode == WindowModes.AntiCausal)
            {
                return
                    GetCausalHalfWindow(type, length)
                        .Reverse()
                        .Concat(Enumerable.Repeat(1.0, length - l))
                        .ToReadOnlyList();
            }

            throw new Exception();
        }

        /// <summary>
        ///     Gets the negative half of a window function.
        /// </summary>
        /// <param name="type">The window type.</param>
        /// <param name="length">The length of the half window.</param>
        /// <returns></returns>
        public static IEnumerable<double> GetAntiCausalHalfWindow(WindowTypes type, int length)
        {
            var winfunc = GetWindowFunction(type);
            return Enumerable.Range(1, length).Select(i => winfunc((double)i / length));
        }

        /// <summary>
        ///     Gets the positive half of a window function.
        /// </summary>
        /// <param name="type">The window type.</param>
        /// <param name="length">The length of the half window.</param>
        /// <returns></returns>
        public static IEnumerable<double> GetCausalHalfWindow(WindowTypes type, int length)
        {
            var winfunc = GetWindowFunction(type);
            return Enumerable.Range(1, length).Reverse().Select(i => winfunc((double)i / length));
        }

        /// <summary>
        ///     Gets the time samples for the specified window.
        /// </summary>
        /// <param name="type">The window type.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static IEnumerable<double> GetWindow(WindowTypes type, int length)
        {
            var hw = GetCausalHalfWindow(type, (length >> 1) + 1).ToReadOnlyList();
            return hw.Reverse().Concat(hw.GetPaddedRange(1, ((length + 1) >> 1) - 1));
        }

        /// <remarks>https://en.wikipedia.org/wiki/Window_function#A_list_of_window_functions</remarks>
        public static Func<double, double> GetWindowFunction(WindowTypes windowType)
        {
            switch (windowType)
            {
            case WindowTypes.Rectangular:
                return d => 1;
            case WindowTypes.Hann:
                return HannWindow;
            case WindowTypes.Hamming:
                return HammingWindow;
            case WindowTypes.Triangular:
                return TriangularWindow;
            case WindowTypes.Welch:
                return WelchWindow;
            case WindowTypes.Blackman:
                return BlackmanWindow;
            case WindowTypes.BlackmanHarris:
                return BlackmanHarrisWindow;
            case WindowTypes.KaiserAlpha2:
                return Kaiser2Window;
            case WindowTypes.KaiserAlpha3:
                return Kaiser3Window;
            }

            throw new ArgumentOutOfRangeException(nameof(windowType), windowType, null);
        }

        public static double GetWindowValue(WindowTypes windowType, double value)
        {
            if (value <= 0)
                return 0;

            if (value >= 1)
                return 1;

            return GetWindowFunction(windowType)(value);
        }

        public static IEnumerable<double> GetWindowValues(WindowTypes windowType, IEnumerable<double> value)
        {
            var winfunc = GetWindowFunction(windowType);

            return value.Select(
                d =>
                {
                    if (d <= 0)
                        return 0;

                    if (d >= 1)
                        return 1;

                    return winfunc(d);
                });
        }

        private static double BlackmanHarrisWindow(double value)
        {
            return 0.35875 - 0.48829 * Math.Cos(Math.PI * value) + 0.14128 * Math.Cos(2 * Math.PI * value) -
                   0.01168 * Math.Cos(3 * Math.PI * value);
        }

        private static double BlackmanWindow(double value)
        {
            return 0.42659 - 0.49656 * Math.Cos(Math.PI * value) + 0.076849 * Math.Cos(2 * Math.PI * value);
        }

        private static int GetDefaultStart(WindowModes mode, int length)
        {
            if (mode == WindowModes.Symmetric)
                return -(length >> 1);

            if (mode == WindowModes.Causal)
                return 0;

            if (mode == WindowModes.AntiCausal)
                return -length;

            throw new Exception();
        }

        private static double HammingWindow(double value)
        {
            return 0.54 - 0.46 * Math.Cos(Math.PI * value);
        }

        private static double HannWindow(double value)
        {
            return 0.5 * (1 - Math.Cos(Math.PI * value));
        }

        private static double Kaiser2Window(double value)
        {
            return Mathematic.ModBessel0(Math.PI * 2 * Math.Sqrt(1 - Math.Pow(value - 1, 2))) /
                   Mathematic.ModBessel0(Math.PI * 2);
        }

        private static double Kaiser3Window(double value)
        {
            return Mathematic.ModBessel0(Math.PI * 3 * Math.Sqrt(1 - Math.Pow(value - 1, 2))) /
                   Mathematic.ModBessel0(Math.PI * 3);
        }

        private static double TriangularWindow(double value)
        {
            return value;
        }

        private static double WelchWindow(double value)
        {
            return 1 - Math.Pow(1 - value, 2);
        }

        [Category("window")]
        [DisplayName("type")]
        public WindowTypes Type { get; }

        [DisplayName("mode")]
        public WindowModes Mode { get; }
    }
}