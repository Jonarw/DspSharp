// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComplexVectors.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Extensions;
using DspSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DspSharp.Algorithms
{
    public static partial class ComplexVectors
    {
        /// <summary>
        /// Combines two real-valued collections of magnitude and phase into one complex-valued collection.
        /// </summary>
        /// <param name="magnitude">The magnitude.</param>
        /// <param name="phase">The phase. Must have the same length as <paramref name="magnitude"/>.</param>
        /// <returns>The complex-valued collection.</returns>
        public static IEnumerable<Complex> FromMagnitudeAndPhase(IEnumerable<double> magnitude, IEnumerable<double> phase)
        {
            return magnitude.ZipExact(phase, Complex.FromPolarCoordinates);
        }

        /// <summary>
        /// Combines two real-valued collections of magnitude and phase into one complex-valued collection.
        /// </summary>
        /// <param name="magnitude">The magnitude.</param>
        /// <param name="phase">The phase. Must have the same length as <paramref name="magnitude"/>.</param>
        /// <returns>The complex-valued collection.</returns>
        /// <remarks>This is evaluated lazily.</remarks>
        public static ILazyReadOnlyCollection<Complex> FromMagnitudeAndPhase(IReadOnlyCollection<double> magnitude, IReadOnlyCollection<double> phase)
        {
            return magnitude.ZipWithCount(phase, Complex.FromPolarCoordinates);
        }

        /// <summary>
        /// Combines two real-valued collections of magnitude and phase into one complex-valued collection.
        /// </summary>
        /// <param name="magnitude">The magnitude.</param>
        /// <param name="phase">The phase. Must have the same length as <paramref name="magnitude"/>.</param>
        /// <returns>The complex-valued collection.</returns>
        /// <remarks>This is evaluated lazily.</remarks>
        public static ILazyReadOnlyList<Complex> FromMagnitudeAndPhase(IReadOnlyList<double> magnitude, IReadOnlyList<double> phase)
        {
            return magnitude.ZipIndexed(phase, Complex.FromPolarCoordinates);
        }

        /// <summary>
        /// Converts a complex-valued vector to a real-valued vector containing the real and imaginary parts in an alternating pattern.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        public static IEnumerable<double> Interleave(this IEnumerable<Complex> sequence)
        {
            foreach (var complex in sequence)
            {
                yield return complex.Real;
                yield return complex.Imaginary;
            }
        }

        /// <summary>
        /// Creates a new complex sequence from a sequence of real parts.
        /// </summary>
        /// <param name="sequence">The real-valued input sequence.</param>
        public static IEnumerable<Complex> ToComplex(this IEnumerable<double> sequence)
        {
            return sequence.Select(d => new Complex(d, 0));
        }

        /// <summary>
        /// Creates a complex-valued vector from an interleaved real-valued vector containing real and imaginary parts in an alternating pattern.
        /// </summary>
        /// <param name="sequence">The sequence of interleaved real and imaginary parts.</param>
        public static IEnumerable<Complex> UnInterleaveComplex(this IEnumerable<double> sequence)
        {
            using var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var real = enumerator.Current;
                if (!enumerator.MoveNext())
                    throw new ArgumentException("The sequence must have an even number of elements.");

                yield return new Complex(real, enumerator.Current);
            }
        }
    }
}