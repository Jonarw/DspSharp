﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using DspSharp.Collections;
using DspSharp.Filters;
using DspSharp.LtiFilters;
using DspSharp.Signal;

namespace DspSharp
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
            FilterTypes type,
            double samplerate,
            IReadOnlyObservableList<ISignal> availableSignals = null)
        {
            if (type == FilterTypes.Distortion)
                return new DistortionFilter(samplerate);

            if (type == FilterTypes.Biquad)
                return new BiquadFilter(samplerate);

            if (type == FilterTypes.CustomConvolver)
                return new CustomConvolver(samplerate) {AvailableSignals = availableSignals};

            if (type == FilterTypes.Correcting)
                return new CorrectingFilter(samplerate);

            if (type == FilterTypes.Delay)
                return new DelayFilter(samplerate);

            if (type == FilterTypes.Dirac)
                return new DiracFilter(samplerate);

            if (type == FilterTypes.Fir)
                return new FirFilter(samplerate);

            if (type == FilterTypes.Gain)
                return new GainFilter(samplerate);

            if (type == FilterTypes.Iir)
                return new IirFilter(samplerate);

            if (type == FilterTypes.Invert)
                return new InvertFilter(samplerate);

            if (type == FilterTypes.Zero)
                return new ZeroFilter(samplerate);

            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}