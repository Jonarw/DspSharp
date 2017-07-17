// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrequencyDomain.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DspSharp.Algorithms
{
    public static class FrequencyDomain
    {
        //TODO: unit test
        public static IReadOnlyList<double> PinkenSpectrum(IReadOnlyList<double> signal, double samplerate, double f0)
        {
            var spec = Fft.RealFft(signal);
            var freq = Fft.GetFrequencies(samplerate, signal.Count);
            var spec1 = PinkenSpectrum(freq, spec, f0);
            return Fft.RealIfft(spec1, signal.Count);
        }

        //TODO: unit test
        public static IEnumerable<Complex> PinkenSpectrum(IEnumerable<double> frequencies, IEnumerable<Complex> spectrum, double f0)
        {
            return frequencies.Zip(spectrum, (f, c) => c / Math.Sqrt(Math.Max(1, f / f0)));
        }

        /// <summary>
        ///     Applies a time delay to a complex frequency spectrum by representing the constant group delay in the complex phase
        ///     information.
        /// </summary>
        /// <param name="frequencies">The frequencies of the complex spectrum.</param>
        /// <param name="amplitudes">
        ///     The complex amplitudes of the spectrum.
        /// </param>
        /// <param name="delay">The delay to be applied to the spectrum. Can be negative.</param>
        /// <returns>
        ///     A new array containing the result. If <paramref name="frequencies" /> and <paramref name="amplitudes" /> are not
        ///     the same length, the longer one is truncated.
        /// </returns>
        public static IEnumerable<Complex> ApplyDelayToSpectrum(
            IEnumerable<double> frequencies,
            IEnumerable<Complex> amplitudes,
            double delay)
        {
            if (frequencies == null)
                throw new ArgumentNullException(nameof(frequencies));

            if (amplitudes == null)
                throw new ArgumentNullException(nameof(amplitudes));

            var factor = Complex.ImaginaryOne * 2 * Math.PI * delay;
            return frequencies.Zip(amplitudes, (f, a) => Complex.Exp(factor * f) * a);
        }

        /// <summary>
        ///     Calculates the group delay of a system for a given phase response.
        /// </summary>
        /// <param name="frequencies">
        ///     The frequencies the phase values correspond to.
        /// </param>
        /// <param name="phase">The phase values.</param>
        /// <returns>
        ///     An array containing the result (in seconds). If <paramref name="frequencies" /> and <paramref name="phase" /> are
        ///     not the same length, the longer one is truncated.
        /// </returns>
        public static IEnumerable<double> CalculateGroupDelay(IEnumerable<double> frequencies, IEnumerable<double> phase)
        {
            if (frequencies == null)
                throw new ArgumentNullException(nameof(frequencies));

            if (phase == null)
                throw new ArgumentNullException(nameof(phase));

            var phaselist = phase.ToReadOnlyList();
            var frequencylist = frequencies.ToReadOnlyList();

            var n = Math.Min(phaselist.Count, frequencylist.Count);

            if (n == 0)
                yield break;

            yield return (phaselist[0] - phaselist[1]) / (2 * Math.PI * (frequencylist[1] - frequencylist[0]));
            for (var c = 1; c < n - 1; c++)
            {
                yield return
                    (phaselist[c - 1] - phaselist[c + 1]) / (2 * Math.PI * (frequencylist[c + 1] - frequencylist[c - 1]));
            }

            yield return (phaselist[phaselist.Count - 2] - phaselist[phaselist.Count - 1]) /
                         (2 * Math.PI * (frequencylist[frequencylist.Count - 1] - frequencylist[frequencylist.Count - 2]));
        }

        //TODO: unit test
        public static double CalculateEnergy(IReadOnlyList<Complex> spectrum, IReadOnlyList<double> frequencies, double fc1, double fc2)
        {
            if (spectrum == null)
                throw new ArgumentNullException(nameof(spectrum));
            if (frequencies == null)
                throw new ArgumentNullException(nameof(frequencies));
            if (frequencies.Count != spectrum.Count)
                throw new ArgumentException();
            if (fc1 <= 0)
                throw new ArgumentOutOfRangeException(nameof(fc1));
            if (fc2 <= fc1)
                throw new ArgumentOutOfRangeException(nameof(fc2));

            var energy = 0d;
            var i = 0;
            while ((i < spectrum.Count) && frequencies[i] < fc1)
            {
                i++;
            }

            while ((i < spectrum.Count) && frequencies[i] < fc2)
            {
                energy += spectrum[i].Magnitude * spectrum[i].Magnitude;
                i++;
            }

            return energy;
        }

        /// <summary>
        ///     Converts an array from dB to linear scale.
        /// </summary>
        /// <param name="dB">The array in dB scale.</param>
        /// <returns>A new array of the same length as <paramref name="dB" /> containing the result.</returns>
        public static IEnumerable<double> DbToLinear(IEnumerable<double> dB)
        {
            if (dB == null)
                throw new ArgumentNullException(nameof(dB));

            return dB.Select(DbToLinear);
        }

        /// <summary>
        ///     Converts a single value from dB to linear scale.
        /// </summary>
        /// <param name="dB">The value in dB.</param>
        /// <returns>The value in linear scale.</returns>
        public static double DbToLinear(double dB)
        {
            return Math.Pow(10, dB / 20);
        }

        /// <summary>
        ///     Converts a singe value from degree to rad.
        /// </summary>
        /// <param name="deg">The value in degree.</param>
        /// <returns>The value in rad.</returns>
        public static double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }

        /// <summary>
        ///     Converts an array from degree to rad.
        /// </summary>
        /// <param name="deg">The array in degree.</param>
        /// <returns>A new array of the same length as <paramref name="deg" /> containing the result.</returns>
        public static IEnumerable<double> DegToRad(IEnumerable<double> deg)
        {
            if (deg == null)
                throw new ArgumentNullException(nameof(deg));

            return deg.Select(DegToRad);
        }

        /// <summary>
        ///     Calculates the frequency response of an IIR filter.
        /// </summary>
        /// <param name="a">The denominator coefficients.</param>
        /// <param name="b">The numerator coefficents.</param>
        /// <param name="frequencies">The frequencies.</param>
        /// <param name="samplerate">The samplerate.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static IEnumerable<Complex> IirFrequencyResponse(
            IReadOnlyList<double> a,
            IReadOnlyList<double> b,
            IReadOnlyList<double> frequencies,
            double samplerate)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            if (b == null)
                throw new ArgumentNullException(nameof(b));

            if (frequencies == null)
                throw new ArgumentNullException(nameof(frequencies));

            if (samplerate <= 0)
                throw new ArgumentOutOfRangeException(nameof(samplerate));

            if (a.Count < b.Count)
                a = a.PadRight(b.Count - a.Count).ToReadOnlyList();
            else
            {
                if (b.Count < a.Count)
                    b = b.PadRight(a.Count - b.Count).ToReadOnlyList();
            }

            if ((a.Count == 0) || (a[0] == 0))
                throw new Exception("a0 cannot be 0.");

            var n = a.Count;
            var factor = 2 * Math.PI / samplerate;

            foreach (var d in frequencies)
            {
                var w = d * factor;
                Complex nom = 0;
                Complex den = 0;
                for (var c1 = 0; c1 < n; c1++)
                {
                    nom += b[c1] * Complex.Exp(-(n - c1) * Complex.ImaginaryOne * w);
                    den += a[c1] * Complex.Exp(-(n - c1) * Complex.ImaginaryOne * w);
                }

                yield return nom / den;
            }
        }

        /// <summary>
        ///     Converts a single value from linear scale to dB.
        /// </summary>
        /// <param name="linear">The value in linear scale.</param>
        /// <param name="minValue">The minimum return value.</param>
        /// <returns>The value in dB.</returns>
        public static double LinearToDb(double linear, double minValue = double.NegativeInfinity)
        {
            if (linear <= 0)
                return minValue;

            return Math.Max(20 * Math.Log10(linear), minValue);
        }

        /// <summary>
        ///     Converts an array from linear scale to dB.
        /// </summary>
        /// <param name="linear">The array in linear scale.</param>
        /// <param name="minValue">The minimum return value.</param>
        /// <returns>A new array of the same length as <paramref name="linear" /> containing the result.</returns>
        public static IEnumerable<double> LinearToDb(
            IEnumerable<double> linear,
            double minValue = double.NegativeInfinity)
        {
            if (linear == null)
                throw new ArgumentNullException(nameof(linear));

            return linear.Select(d => LinearToDb(d, minValue));
        }

        /// <summary>
        ///     Converts two individual arrays containing magnitude and phase information to one complex array.
        /// </summary>
        /// <param name="amplitude">The amplitude data.</param>
        /// <param name="phase">The phase data. Has to be the same length as <paramref name="amplitude" />.</param>
        /// <returns>
        ///     A new complex array of the same length as <paramref name="amplitude" /> and <paramref name="phase" />
        ///     containing the result.
        /// </returns>
        public static IEnumerable<Complex> PolarToComplex(IEnumerable<double> amplitude, IEnumerable<double> phase)
        {
            if (amplitude == null)
                throw new ArgumentNullException(nameof(amplitude));

            if (phase == null)
                throw new ArgumentNullException(nameof(phase));

            return amplitude.Zip(phase, Complex.FromPolarCoordinates);
        }

        /// <summary>
        ///     Converts a single value from rad to degree.
        /// </summary>
        /// <param name="rad">The value in rad.</param>
        /// <returns>The value in degree.</returns>
        public static double RadToDeg(double rad)
        {
            return rad * 180 / Math.PI;
        }

        /// <summary>
        ///     Converts a sequence from rad to deg.
        /// </summary>
        /// <param name="rad">The array in rad.</param>
        /// <returns>A new sequence of the same length as <paramref name="rad" /> containing the result.</returns>
        public static IEnumerable<double> RadToDeg(IEnumerable<double> rad)
        {
            if (rad == null)
                throw new ArgumentNullException(nameof(rad));

            return rad.Select(RadToDeg);
        }

        /// <summary>
        ///     Unwraps phase information.
        /// </summary>
        /// <param name="phase">The phase sequence.</param>
        /// <param name="useDeg">If true, the phase unit is assumed to be degree, otherwise rad (default).</param>
        /// <returns>A new sequence of the same length as <paramref name="phase" /> containing the result.</returns>
        public static IEnumerable<double> UnwrapPhase(IEnumerable<double> phase, bool useDeg = false)
        {
            if (phase == null)
                throw new ArgumentNullException(nameof(phase));

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

            using (var e = phase.GetEnumerator())
            {
                if (!e.MoveNext())
                    yield break;

                var previousPhase = e.Current;
                yield return e.Current;

                while (e.MoveNext())
                {
                    if (previousPhase - e.Current > halfPeriod)
                        offset += fullPeriod;
                    else
                    {
                        if (previousPhase - e.Current < -halfPeriod)
                            offset -= fullPeriod;
                    }

                    previousPhase = e.Current;
                    yield return e.Current + offset;
                }
            }
        }

        /// <summary>
        ///     Wraps phase data from an array, so that all resulting values are in the range -180° to +180° (or -pi to +pi).
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <param name="useDeg">If true, the angular unit is assumed to be degree, otherwise radians (default).</param>
        /// <returns></returns>
        public static IEnumerable<double> WrapPhase(IEnumerable<double> input, bool useDeg = false)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

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
                    var tmp = d % fullPeriod;
                    if (tmp > halfPeriod)
                        tmp -= fullPeriod;
                    else
                    {
                        if (tmp < -halfPeriod)
                            tmp += fullPeriod;
                    }

                    return tmp;
                });
        }
    }
}