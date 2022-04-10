// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interpolation.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Extensions;
using System.Collections.Generic;

namespace DspSharp.Algorithms
{
    /// <summary>
    /// Represents an <see cref="Interpolator"/> which employs an adaptive algorithm that dynamically chooses between average, linear and (optionally) spline interpolation depending on local point density.
    /// </summary>
    public class AdaptiveInterpolator : Interpolator
    {
        /// <summary>
        /// Gets or sets a value indicating whether spline interpolation should be used for very low point densities. If this is set to <c>false</c>, linear interpolation will be used instead.
        /// </summary>
        public bool UseSpline { get; set; }

        /// <inheritdoc/>
        protected override IEnumerable<double> InterpolateOverride(IReadOnlyList<double> x, IReadOnlyList<double> y, IReadOnlyList<double> targetX)
        {
            IReadOnlyList<double> spline = null;

            var xc = 0;
            while (x[xc] < targetX[0])
                xc++;

            var xCurrent = x[xc];
            for (var c = 0; c < targetX.Count; c++)
            {
                var xlim = c == targetX.Count - 1
                    ? targetX[c]
                    : (targetX[c + 1] + targetX[c]) / 2;

                var pointCounter = 0;
                while (xCurrent < xlim)
                {
                    pointCounter++;
                    xCurrent++;
                }

                if (this.UseSpline && pointCounter < 2) // spline
                {
                    if (spline == null)
                        spline = CubicSpline.CubicSpline.Compute(x.CastOrToArray(), y.CastOrToArray(), targetX.CastOrToArray());

                    yield return spline[c];
                }
                else if (pointCounter < 3) // linear interpolation
                {
                    yield return LinearInterpolation(targetX[c], x[xc - 1], x[xc], y[xc - 1], y[xc]);
                }
                else // average
                {
                    var sum = 0d;
                    for (var c2 = 1; c2 <= pointCounter; c2++)
                        sum += y[xc - c2];

                    sum /= pointCounter;
                    yield return sum;
                }
            }
        }
    }
}