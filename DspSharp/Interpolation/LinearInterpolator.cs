// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interpolation.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace DspSharp.Algorithms
{
    /// <summary>
    /// Represents an <see cref="Interpolator"/> which uses linear interpolation.
    /// </summary>
    public class LinearInterpolator : Interpolator
    {
        /// <inheritdoc/>
        protected override IEnumerable<double> InterpolateOverride(IReadOnlyList<double> x, IReadOnlyList<double> y, IReadOnlyList<double> targetX)
        {
            var xc = 1;

            for (var c = 0; c < targetX.Count; c++)
            {
                while (x[xc] < targetX[c])
                    xc++;

                yield return LinearInterpolation(targetX[c], x[xc - 1], x[xc], y[xc - 1], y[xc]);
            }
        }
    }
}