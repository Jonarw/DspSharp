// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interpolation.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Extensions;
using System.Collections.Generic;

namespace DspSharp.Algorithms
{
    public class SplineInterpolator : Interpolator
    {
        /// <inheritdoc/>
        protected override IEnumerable<double> InterpolateOverride(IReadOnlyList<double> x, IReadOnlyList<double> y, IReadOnlyList<double> targetX)
        {
            return CubicSpline.CubicSpline.Compute(x.CastOrToArray(), y.CastOrToArray(), targetX.CastOrToArray());
        }
    }
}