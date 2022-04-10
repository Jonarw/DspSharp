// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrequencyDomain.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Exceptions;
using DspSharp.Extensions;
using DspSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DspSharp.Algorithms
{
    public static class FrequencyDomain
    {
        /// <summary>
        /// Applies a 'pinkening' filter to the signal. This filter dampens high frequencies above <paramref name="f0"/> with a slope of 3 dB per octave.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <param name="samplerate">The samplerate.</param>
        /// <param name="f0">The frequency determining the lower border where the pinkening filter takes effect.</param>
        /// <remarks>This works in frequency domain using a FFT-based approach.</remarks>
        public static IReadOnlyList<double> PinkenSpectrum(IReadOnlyCollection<double> signal, double samplerate, double f0)
        {
            var spec = Fft.RealFft(signal);
            var freq = Fft.GetFrequencies(samplerate, signal.Count);
            var spec1 = PinkenSpectrum(freq, spec, f0);
            return Fft.RealIfft(spec1, signal.Count);
        }

        /// <summary>
        /// Applies a 'pinkening' filter to a spectrum. This filter dampens high frequencies above <paramref name="f0"/> with a slope of 3 dB per octave.
        /// </summary>
        /// <param name="frequencies">The frequencies of the spectrum.</param>
        /// <param name="spectrum">The complex magnitude of the spectrum. Must be the same length as <paramref name="frequencies"/>.</param>
        /// <param name="f0">The frequency determining the lower border where the pinkening filter takes effect.</param>
        public static ILazyReadOnlyCollection<Complex> PinkenSpectrum(IReadOnlyCollection<double> frequencies, IReadOnlyCollection<Complex> spectrum, double f0)
        {
            if (frequencies.Count != spectrum.Count)
                throw new LengthMismatchException(nameof(frequencies), nameof(spectrum));

            return frequencies
                .Zip(spectrum, (f, c) => c / Math.Sqrt(Math.Max(1, f / f0)))
                .WithCount(frequencies.Count);
        }

        /// <summary>
        /// Applies a time delay to a complex frequency spectrum by representing the constant group delay in the complex phase information.
        /// </summary>
        /// <param name="frequencies">The frequencies of the complex spectrum.</param>
        /// <param name="amplitudes">The complex amplitudes of the spectrum.</param>
        /// <param name="delay">The delay to be applied to the spectrum. Can be negative.</param>
        /// <returns>A new collection containing the result.</returns>
        public static ILazyReadOnlyCollection<Complex> ApplyDelayToSpectrum(IReadOnlyCollection<double> frequencies, IReadOnlyCollection<Complex> amplitudes, double delay)
        {
            if (frequencies.Count != amplitudes.Count)
                throw new LengthMismatchException(nameof(frequencies), nameof(amplitudes));

            var factor = Complex.ImaginaryOne * 2 * Math.PI * delay;
            return frequencies
                .Zip(amplitudes, (f, a) => Complex.Exp(factor * f) * a)
                .WithCount(frequencies.Count);
        }

        /// <summary>
        /// Calculates the group delay of a system for a given phase response.
        /// </summary>
        /// <param name="frequencies">The frequencies the phase values correspond to.</param>
        /// <param name="phase">The phase values.</param>
        public static ILazyReadOnlyCollection<double> CalculateGroupDelay(IReadOnlyList<double> frequencies, IReadOnlyList<double> phase)
        {
            if (frequencies.Count != phase.Count)
                throw new LengthMismatchException(nameof(frequencies), nameof(phase));

            return CalculateGroupDelayIterator().WithCount(frequencies.Count);

            IEnumerable<double> CalculateGroupDelayIterator()
            {
                yield return (phase[0] - phase[1]) / (2 * Math.PI * (frequencies[1] - frequencies[0]));
                for (var c = 1; c < phase.Count - 1; c++)
                {
                    yield return
                        (phase[c - 1] - phase[c + 1]) / (2 * Math.PI * (frequencies[c + 1] - frequencies[c - 1]));
                }

                yield return (phase[phase.Count - 2] - phase[phase.Count - 1]) /
                             (2 * Math.PI * (frequencies[frequencies.Count - 1] - frequencies[frequencies.Count - 2]));
            }
        }

        /// <summary>
        /// Converts a collection from dB to linear scale.
        /// </summary>
        /// <param name="dbValues">The collection in dB scale.</param>
        /// <returns>A new collection of the same length as <paramref name="dbValues" /> containing the result.</returns>
        public static IEnumerable<double> DbToLinear(IEnumerable<double> dbValues)
        {
            return dbValues.Select(DbToLinear);
        }

        /// <summary>
        /// Converts a collection from dB to linear scale.
        /// </summary>
        /// <param name="dbValues">The collection in dB scale.</param>
        /// <returns>A new collection of the same length as <paramref name="dbValues" /> containing the result.</returns>
        public static ILazyReadOnlyCollection<double> DbToLinear(IReadOnlyCollection<double> dbValues)
        {
            return dbValues.SelectWithCount(DbToLinear);
        }

        /// <summary>
        /// Converts a collection from dB to linear scale.
        /// </summary>
        /// <param name="dbValues">The collection in dB scale.</param>
        /// <returns>A new collection of the same length as <paramref name="dbValues" /> containing the result.</returns>
        public static ILazyReadOnlyList<double> DbToLinear(IReadOnlyList<double> dbValues)
        {
            return dbValues.SelectIndexed(DbToLinear);
        }

        /// <summary>
        /// Converts a single value from dB to linear scale.
        /// </summary>
        /// <param name="dbValue">The value in dB.</param>
        /// <returns>The value in linear scale.</returns>
        public static double DbToLinear(double dbValue)
        {
            return Math.Pow(10, dbValue / 20);
        }

        /// <summary>
        /// Converts a single value from degree to rad.
        /// </summary>
        /// <param name="degValue">The value in degree.</param>
        /// <returns>The value in rad.</returns>
        public static double DegToRad(double degValue)
        {
            return degValue * Math.PI / 180;
        }

        /// <summary>
        /// Converts a collection from degree to rad.
        /// </summary>
        /// <param name="degValues">The collection in degree.</param>
        /// <returns>A new collection of the same length as <paramref name="degValues" /> containing the result.</returns>
        public static IEnumerable<double> DegToRad(IEnumerable<double> degValues)
        {
            return degValues.Select(DegToRad);
        }

        /// <summary>
        /// Converts a collection from degree to rad.
        /// </summary>
        /// <param name="degValues">The collection in degree.</param>
        /// <returns>A new collection of the same length as <paramref name="degValues" /> containing the result.</returns>
        public static ILazyReadOnlyCollection<double> DegToRad(IReadOnlyCollection<double> degValues)
        {
            return degValues.SelectWithCount(DegToRad);
        }

        /// <summary>
        /// Converts a collection from degree to rad.
        /// </summary>
        /// <param name="degValues">The collection in degree.</param>
        /// <returns>A new collection of the same length as <paramref name="degValues" /> containing the result.</returns>
        public static ILazyReadOnlyList<double> DegToRad(IReadOnlyList<double> degValues)
        {
            return degValues.SelectIndexed(DegToRad);
        }

        /// <summary>
        /// Calculates the frequency response of an IIR filter defined by a and b coefficients.
        /// </summary>
        /// <param name="a">The denominator coefficients.</param>
        /// <param name="b">The numerator coefficents. Must be the same length as <paramref name="a"/>.</param>
        /// <param name="frequencies">The frequencies.</param>
        /// <param name="samplerate">The samplerate.</param>
        /// <returns>A new collection of the same length as <paramref name="frequencies"/> containing the frequency response of the IIR filter.</returns>
        public static ILazyReadOnlyCollection<Complex> IirFrequencyResponse(
            IReadOnlyList<double> a,
            IReadOnlyList<double> b,
            IReadOnlyCollection<double> frequencies,
            double samplerate)
        {
            if (samplerate <= 0)
                throw new ArgumentOutOfRangeException(nameof(samplerate));

            if (a.Count != b.Count)
                throw new LengthMismatchException(nameof(a), nameof(b));

            if ((a.Count == 0) || (a[0] == 0))
                throw new Exception("a0 cannot be 0.");

            return IirFrequencyResponseIterator().WithCount(frequencies.Count);

            IEnumerable<Complex> IirFrequencyResponseIterator()
            {
                var n = a.Count;
                var twoPiOverFs = 2 * Math.PI / samplerate;

                foreach (var f in frequencies)
                {
                    var w = f * twoPiOverFs;
                    Complex nom = 0;
                    Complex den = 0;
                    for (var c = 0; c < n; c++)
                    {
                        nom += b[c] * Complex.Exp(-(n - c) * Complex.ImaginaryOne * w);
                        den += a[c] * Complex.Exp(-(n - c) * Complex.ImaginaryOne * w);
                    }

                    yield return nom / den;
                }
            }
        }

        /// <summary>
        /// Converts a single value from linear scale to dB.
        /// </summary>
        /// <param name="linearValue">The value in linear scale.</param>
        /// <returns>The value in dB.</returns>
        public static double LinearToDb(double linearValue)
        {
            if (linearValue <= 0)
                return double.NegativeInfinity;

            return 20 * Math.Log10(linearValue);
        }

        /// <summary>
        /// Converts a collection from linear scale to dB.
        /// </summary>
        /// <param name="linearValues">The collection in linear scale.</param>
        /// <returns>A new collection of the same length as <paramref name="linearValues" /> containing the result.</returns>
        public static IEnumerable<double> LinearToDb(this IEnumerable<double> linearValues)
        {
            return linearValues.Select(LinearToDb);
        }

        /// <summary>
        /// Converts a collection from linear scale to dB.
        /// </summary>
        /// <param name="linearValues">The collection in linear scale.</param>
        /// <returns>A new collection of the same length as <paramref name="linearValues" /> containing the result.</returns>
        public static ILazyReadOnlyCollection<double> LinearToDb(this IReadOnlyCollection<double> linearValues)
        {
            return linearValues.SelectWithCount(LinearToDb);
        }

        /// <summary>
        /// Converts a collection from linear scale to dB.
        /// </summary>
        /// <param name="linearValues">The collection in linear scale.</param>
        /// <returns>A new collection of the same length as <paramref name="linearValues" /> containing the result.</returns>
        public static ILazyReadOnlyList<double> LinearToDb(this IReadOnlyList<double> linearValues)
        {
            return linearValues.SelectIndexed(LinearToDb);
        }

        /// <summary>
        /// Converts two individual arrays containing magnitude and phase information to one complex array.
        /// </summary>
        /// <param name="amplitude">The amplitude data.</param>
        /// <param name="phase">The phase data. Must to be the same length as <paramref name="amplitude" />.</param>
        /// <returns>A new complex array of the same length as <paramref name="amplitude" /> and <paramref name="phase" /> containing the result.</returns>
        public static ILazyReadOnlyCollection<Complex> PolarToComplex(IReadOnlyCollection<double> amplitude, IReadOnlyCollection<double> phase)
        {
            if (amplitude.Count != phase.Count)
                throw new LengthMismatchException(nameof(amplitude), nameof(phase));

            return amplitude.Zip(phase, Complex.FromPolarCoordinates).WithCount(amplitude.Count);
        }

        /// <summary>
        /// Converts a single value from rad to degree.
        /// </summary>
        /// <param name="radValue">The value in rad.</param>
        /// <returns>The value in degree.</returns>
        public static double RadToDeg(double radValue)
        {
            return radValue * 180 / Math.PI;
        }

        /// <summary>
        /// Converts a collection from rad to degree.
        /// </summary>
        /// <param name="radValues">The collection in rad.</param>
        /// <returns>A new collection of the same length as <paramref name="radValues" /> containing the result.</returns>
        public static IEnumerable<double> RadToDeg(IEnumerable<double> radValues)
        {
            return radValues.Select(RadToDeg);
        }

        /// <summary>
        /// Converts a collection from rad to degree.
        /// </summary>
        /// <param name="radValues">The collection in rad.</param>
        /// <returns>A new collection of the same length as <paramref name="radValues" /> containing the result.</returns>
        public static ILazyReadOnlyCollection<double> RadToDeg(IReadOnlyCollection<double> radValues)
        {
            return radValues.SelectWithCount(RadToDeg);
        }

        /// <summary>
        /// Converts a collection from rad to degree.
        /// </summary>
        /// <param name="radValues">The collection in rad.</param>
        /// <returns>A new collection of the same length as <paramref name="radValues" /> containing the result.</returns>
        public static ILazyReadOnlyList<double> RadToDeg(IReadOnlyList<double> radValues)
        {
            return radValues.SelectIndexed(RadToDeg);
        }

        /// <summary>
        ///  Unwraps phase information.
        /// </summary>
        /// <param name="phase">The phase sequence.</param>
        /// <param name="useDeg">If true, the phase unit is assumed to be degree, otherwise rad (default).</param>
        /// <returns>A new sequence of the same length as <paramref name="phase" /> containing the result.</returns>
        public static ILazyReadOnlyCollection<double> UnwrapPhase(IReadOnlyCollection<double> phase, bool useDeg = false)
        {
            return UnwrapPhase((IEnumerable<double>)phase, useDeg).WithCount(phase.Count);
        }

        /// <summary>
        ///  Unwraps phase information.
        /// </summary>
        /// <param name="phase">The phase sequence.</param>
        /// <param name="useDeg">If true, the phase unit is assumed to be degree, otherwise rad (default).</param>
        /// <returns>A new sequence of the same length as <paramref name="phase" /> containing the result.</returns>
        public static IEnumerable<double> UnwrapPhase(IEnumerable<double> phase, bool useDeg = false)
        {
            double fullPeriod;
            double halfPeriod;
            if (useDeg)
            {
                fullPeriod = 360;
                halfPeriod = 180;
            }
            else
            {
                fullPeriod = 2 * Math.PI;
                halfPeriod = Math.PI;
            }

            double offset = 0;

            using var e = phase.GetEnumerator();
            if (!e.MoveNext())
                yield break;

            var previousPhase = e.Current;
            yield return e.Current;

            while (e.MoveNext())
            {
                if (previousPhase - e.Current > halfPeriod)
                    offset += fullPeriod;
                else if (previousPhase - e.Current < -halfPeriod)
                    offset -= fullPeriod;

                previousPhase = e.Current;
                yield return e.Current + offset;
            }
        }

        /// <summary>
        /// Wraps a collection of phase data such that all resulting values are in the range -180° to +180° (or -pi to +pi).
        /// </summary>
        /// <param name="input">The input collection.</param>
        /// <param name="useDeg">If true, the angular unit is assumed to be degree, otherwise radians (default).</param>
        public static ILazyReadOnlyCollection<double> WrapPhase(IReadOnlyCollection<double> input, bool useDeg = false)
        {
            return WrapPhase((IEnumerable<double>)input, useDeg).WithCount(input.Count);

        }
        /// <summary>
        /// Wraps a collection of phase data such that all resulting values are in the range -180° to +180° (or -pi to +pi).
        /// </summary>
        /// <param name="input">The input collection.</param>
        /// <param name="useDeg">If true, the angular unit is assumed to be degree, otherwise radians (default).</param>
        public static IEnumerable<double> WrapPhase(IEnumerable<double> input, bool useDeg = false)
        {
            double fullPeriod;
            double halfPeriod;

            if (useDeg)
            {
                fullPeriod = 360;
                halfPeriod = 180;
            }
            else
            {
                fullPeriod = 2 * Math.PI;
                halfPeriod = Math.PI;
            }

            return input.Select(
                d =>
                {
                    var ret = d % fullPeriod;
                    if (ret > halfPeriod)
                        ret -= fullPeriod;
                    else if (ret < -halfPeriod)
                        ret += fullPeriod;

                    return ret;
                });
        }
    }
}