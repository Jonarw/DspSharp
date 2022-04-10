// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Window.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Algorithms;
using DspSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UTilities;

namespace DspSharp.Signal.Windows
{
    /// <summary>
    /// Provides function for creating time domain windows.
    /// </summary>
    public static class Window
    {
        /// <summary>
        /// Creates a window.
        /// </summary>
        /// <param name="type">The window type.</param>
        /// <param name="mode">The window mode.</param>
        /// <param name="length">The window length.</param>
        /// <remarks>This is evaluated lazily.</remarks>
        public static IReadOnlyCollection<double> CreateWindow(WindowType type, WindowMode mode, int length)
        {
            return mode switch
            {
                WindowMode.Symmetric => GetWindow(type, length),
                WindowMode.Causal => GetCausalHalfWindow(type, length),
                WindowMode.AntiCausal => GetAntiCausalHalfWindow(type, length),
                _ => throw EnumOutOfRangeException.Create(mode),
            };
        }

        /// <summary>
        /// Gets the negative half of a window function.
        /// </summary>
        /// <param name="type">The window type.</param>
        /// <param name="length">The length of the half window.</param>
        /// <remarks>This is evaluated lazily.</remarks>
        public static IReadOnlyCollection<double> GetAntiCausalHalfWindow(WindowType type, int length)
        {
            var winfunc = GetWindowFunction(type);
            return Enumerable
                .Range(1, length)
                .Select(i => winfunc((double)i / length))
                .WithCount(length);
        }

        /// <summary>
        /// Gets the positive half of a window function.
        /// </summary>
        /// <param name="type">The window type.</param>
        /// <param name="length">The length of the half window.</param>
        /// <remarks>This is evaluated lazily.</remarks>
        public static IReadOnlyCollection<double> GetCausalHalfWindow(WindowType type, int length)
        {
            return GetAntiCausalHalfWindow(type, length)
                .Reverse()
                .WithCount(length);
        }

        /// <summary>
        /// Gets the time samples for the specified window.
        /// </summary>
        /// <param name="type">The window type.</param>
        /// <param name="length">The length.</param>
        /// <remarks>This is evaluated lazily.</remarks>
        public static IReadOnlyCollection<double> GetWindow(WindowType type, int length)
        {
            var hw = (IReadOnlyList<double>)GetCausalHalfWindow(type, (length >> 1) + 1).ToList();
            return hw
                .Reverse()
                .Concat(hw.GetPaddedRange(1, ((length + 1) >> 1) - 1))
                .WithCount(length);
        }

        /// <summary>
        /// Gets the window function for the specified window type.
        /// </summary>
        /// <param name="windowType">The window type.</param>
        /// <remarks>https://en.wikipedia.org/wiki/Window_function#A_list_of_window_functions</remarks>
        public static Func<double, double> GetWindowFunction(WindowType windowType)
        {
            return windowType switch
            {
                WindowType.Rectangular => _ => 1,
                WindowType.Hann => HannWindow,
                WindowType.Hamming => HammingWindow,
                WindowType.Triangular => TriangularWindow,
                WindowType.Welch => WelchWindow,
                WindowType.Blackman => BlackmanWindow,
                WindowType.BlackmanHarris => BlackmanHarrisWindow,
                WindowType.KaiserAlpha2 => Kaiser2Window,
                WindowType.KaiserAlpha3 => Kaiser3Window,
                _ => throw EnumOutOfRangeException.Create(windowType),
            };
        }

        public static double GetWindowValue(WindowType windowType, double value)
        {
            if (value <= 0)
                return 0;

            if (value >= 1)
                return 1;

            return GetWindowFunction(windowType)(value);
        }

        public static IEnumerable<double> GetWindowValues(WindowType windowType, IEnumerable<double> value)
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
    }
}