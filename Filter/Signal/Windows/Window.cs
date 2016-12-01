using System;
using System.Collections.Generic;
using System.Linq;
using Filter.Algorithms;
using Filter.Extensions;
using PropertyTools.DataAnnotations;

namespace Filter.Signal.Windows
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
        public Window(WindowTypes type, int start, int length, double samplerate, WindowModes mode)
            : base(CreateWindow(type, mode, length), samplerate, start)
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
        public Window(WindowTypes type, int length, double samplerate, WindowModes mode)
            : this(type, GetDefaultStart(mode, length), length, samplerate, mode)
        {
        }

        /// <summary>
        ///     Gets the positive half of a window function.
        /// </summary>
        /// <param name="type">The window type.</param>
        /// <param name="length">The length of the half window.</param>
        /// <returns></returns>
        /// <remarks>https://en.wikipedia.org/wiki/Window_function#A_list_of_window_functions</remarks>
        public static IEnumerable<double> GetHalfWindow(WindowTypes type, int length)
        {
            // first value is always 1
            yield return 1.0;

            switch (type)
            {
            case WindowTypes.Rectangular:
                for (var n = length - 1; n > 0; n--)
                {
                    yield return 1.0;
                }
                break;
            case WindowTypes.Hann:
                for (var n = length - 1; n > 0; n--)
                {
                    yield return 0.5 * (1 - Math.Cos(Math.PI * n / length));
                }
                break;
            case WindowTypes.Hamming:
                for (var n = length - 1; n > 0; n--)
                {
                    yield return 0.54 - 0.46 * Math.Cos(Math.PI * n / length);
                }
                break;
            case WindowTypes.Triangular:
                for (var n = length - 1; n > 0; n--)
                {
                    yield return 1 - (double)(length - n) / length;
                }
                break;
            case WindowTypes.Welch:
                for (var n = length - 1; n > 0; n--)
                {
                    yield return 1 - Math.Pow((double)(length - n) / length, 2);
                }
                break;
            case WindowTypes.Blackman:
                for (var n = length - 1; n > 0; n--)
                {
                    yield return 0.42659 - 0.49656 * Math.Cos(Math.PI * n / length) + 0.076849 * Math.Cos(2 * Math.PI * n / length);
                }
                break;
            case WindowTypes.BlackmanHarris:
                for (var n = length - 1; n > 0; n--)
                {
                    yield return 0.35875 - 0.48829 * Math.Cos(Math.PI * n / length) + 0.14128 * Math.Cos(2 * Math.PI * n / length) -
                                 0.01168 * Math.Cos(3 * Math.PI * n / length);
                }
                break;
            case WindowTypes.KaiserAlpha2:
            {
                var denom = Dsp.ModBessel0(Math.PI * 2);
                for (var n = length - 1; n > 0; n--)
                {
                    yield return Dsp.ModBessel0(Math.PI * 2 * Math.Sqrt(1 - Math.Pow((double)n / length - 1, 2))) / denom;
                }
            }
                break;
            case WindowTypes.KaiserAlpha3:
            {
                var denom = Dsp.ModBessel0(Math.PI * 3);
                for (var n = length - 1; n > 0; n--)
                {
                    yield return Dsp.ModBessel0(Math.PI * 3 * Math.Sqrt(1 - Math.Pow((double)n / length - 1, 2))) / denom;
                }
            }
                break;
            }
        }

        /// <summary>
        ///     Gets the time samples for the specified window.
        /// </summary>
        /// <param name="type">The window type.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static IEnumerable<double> GetWindow(WindowTypes type, int length)
        {
            var hw = GetHalfWindow(type, (length >> 1) + 1).ToReadOnlyList();
            return hw.Reverse().Concat(hw.GetPaddedRange(1, ((length + 1) >> 1) - 1));
        }

        private static IReadOnlyList<double> CreateWindow(WindowTypes type, WindowModes mode, int length)
        {
            if (mode == WindowModes.Symmetric)
            {
                return GetWindow(type, length).ToReadOnlyList();
            }

            if (mode == WindowModes.Causal)
            {
                return GetHalfWindow(type, length).ToReadOnlyList();
            }

            if (mode == WindowModes.AntiCausal)
            {
                return GetHalfWindow(type, length).Reverse().ToReadOnlyList();
            }

            throw new Exception();
        }

        private static int GetDefaultStart(WindowModes mode, int length)
        {
            if (mode == WindowModes.Symmetric)
            {
                return -(length >> 1);
            }

            if (mode == WindowModes.Causal)
            {
                return 0;
            }

            if (mode == WindowModes.AntiCausal)
            {
                return -length;
            }

            throw new Exception();
        }

        [Category("window")]
        [DisplayName("type")]
        public WindowTypes Type { get; }

        [DisplayName("mode")]
        public WindowModes Mode { get; }
    }
}