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
using DspSharp.Signal;
using UTilities.Collections;

namespace DspSharp.Filter
{
    /// <summary>
    ///     Provides a static function from creating new filter objects.
    /// </summary>
    public static class FilterFactory
    {
        /// <summary>
        ///     Creates a new filter object of the specified filter type.
        /// </summary>
        /// <param name="type">The filter type.</param>
        /// <param name="samplerate">The samplerate.</param>
        /// <param name="availableSignals">The available signals for all filters that are based on existing signals.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">null</exception>
        public static IFilter CreateFilter(
            FilterType type,
            double samplerate,
            IReadOnlyObservableList<ISignal> availableSignals = null)
        {
            switch (type)
            {
            case FilterType.Distortion:
                return new DistortionFilter(samplerate);
            case FilterType.Biquad:
                return new BiquadFilter(samplerate);
            case FilterType.CustomConvolver:
                return new CustomConvolver(samplerate) {AvailableSignals = availableSignals};
            case FilterType.Correcting:
                return new CorrectingFilter(samplerate);
            case FilterType.Delay:
                return new DelayFilter(samplerate);
            case FilterType.Dirac:
                return new DiracFilter(samplerate);
            case FilterType.Fir:
                return new FirFilter(samplerate);
            case FilterType.Gain:
                return new GainFilter(samplerate);
            case FilterType.Iir:
                return new IirFilter(samplerate);
            case FilterType.Invert:
                return new InvertFilter(samplerate);
            case FilterType.Zero:
                return new ZeroFilter(samplerate);
            }

            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}