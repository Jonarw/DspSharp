// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interpolation.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Signal.Windows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Algorithms
{
    public class SmoothingInterpolator : Interpolator
    {
        public SmoothingInterpolator()
        {
            this.ExtrapolationMode = ExtrapolationMode.NaN;
        }

        /// <summary>
        /// Gets or sets the window function that is used to weight samples in the smoothing window.
        /// </summary>
        public WindowType WindowType { get; set; } = WindowType.Rectangular;

        /// <summary>
        /// Gets or sets the width of the smoothing window (in X units).
        /// </summary>
        public double WindowWidth { get; set; }

        /// <summary>
        /// Creates and initializes a new <see cref="SmoothingInterpolator"/> which is set of for smoothing with a constant bandwidth ratio.
        /// </summary>
        /// <param name="bandwidthRatio">The smoothing bandwidth ratio. E.g. a value of 2 means the smoothing bandwidth is one octave.</param>
        public static SmoothingInterpolator FromBandwidth(double bandwidthRatio)
        {
            return new SmoothingInterpolator { LogarithmicX = true, WindowWidth = Math.Log(bandwidthRatio) };
        }

        /// <summary>
        /// Creates and initializes a new <see cref="SmoothingInterpolator"/> which is set of for smoothing with a constant number of points per octave.
        /// </summary>
        /// <param name="pointsPerOctave">The points per octave. E.g. a value of 12 means 1/12th octave smoothing.</param>
        public static SmoothingInterpolator FromPointsPerOctave(double pointsPerOctave)
        {
            return new SmoothingInterpolator { LogarithmicX = true, WindowWidth = Math.Log(2 / pointsPerOctave) };
        }

        /// <inheritdoc/>
        protected override IEnumerable<double> InterpolateOverride(IReadOnlyList<double> x, IReadOnlyList<double> y, IReadOnlyList<double> targetX)
        {
            var smoothWindow = Window.GetWindowFunction(this.WindowType);
            var halfWindowWidth = this.WindowWidth / 2;

            var xList = x.ToList();
            foreach (var currentTargetX in targetX)
            {
                var lowerThresholdX = currentTargetX - halfWindowWidth;
                var upperThresholdX = currentTargetX + halfWindowWidth;

                var lowerThresholdIndex = xList.BinarySearch(lowerThresholdX);
                if (lowerThresholdIndex < 0)
                    lowerThresholdIndex = ~lowerThresholdIndex;

                var sum = 0d;
                var normalization = 0d;
                var i = lowerThresholdIndex;
                for (; i < x.Count; i++)
                {
                    var currentX = x[i];
                    if (currentX >= upperThresholdX)
                        break;

                    var windowInput = Math.Abs((currentX - currentTargetX) / halfWindowWidth);
                    var factor = smoothWindow(1 - windowInput);
                    normalization += factor;
                    sum += factor * y[i];
                }

                if (i == lowerThresholdIndex)
                {
                    yield return LinearInterpolation(currentTargetX, x[i - 1], x[i], y[i - 1], y[i]);
                    continue;
                }

                if (i - lowerThresholdIndex == 1)
                {
                    if (currentTargetX > x[lowerThresholdIndex])
                        i--;

                    yield return LinearInterpolation(currentTargetX, x[i - 1], x[i], y[i - 1], y[i]);
                    continue;
                }

                yield return sum / normalization;
            }
        }
    }
}