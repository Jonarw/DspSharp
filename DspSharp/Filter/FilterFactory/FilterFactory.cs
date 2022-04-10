// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using DspSharp.Filter.LtiFilters.Fir;
using DspSharp.Filter.LtiFilters.Iir;
using DspSharp.Filter.LtiFilters.Primitive;
using DspSharp.Filter.NonlinearFilters;

namespace DspSharp.Filter
{
    /// <summary>
    /// Provides a static function from creating new filter objects.
    /// </summary>
    public static class FilterFactory
    {
        /// <summary>
        /// Creates a new filter object of the specified filter type.
        /// </summary>
        /// <param name="type">The filter type.</param>
        /// <param name="samplerate">The samplerate.</param>
        public static IFilter CreateFilter(
            FilterType type,
            double samplerate)
        {
            return type switch
            {
                FilterType.Distortion => new DistortionFilter(samplerate),
                FilterType.Biquad => new BiquadFilter(samplerate),
                FilterType.Delay => new DelayFilter(samplerate),
                FilterType.Dirac => new DiracFilter(samplerate),
                FilterType.Fir => new FirFilter(samplerate),
                FilterType.Gain => new GainFilter(samplerate),
                FilterType.Invert => new InvertFilter(samplerate),
                FilterType.Zero => new ZeroFilter(samplerate),
                FilterType.Butterworth => new ButterworthFilter(samplerate),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }
    }
}