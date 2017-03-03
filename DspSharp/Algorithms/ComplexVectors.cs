// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComplexVectors.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DspSharp.Algorithms
{
    public static class ComplexVectors
    {
        /// <summary>
        ///     Calculates the complex conjugate of a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <returns>The complex conjugate of the sequence.</returns>
        public static IEnumerable<Complex> ComplexConjugate(this IEnumerable<Complex> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => new Complex(c.Real, -c.Imaginary));
        }

        /// <summary>
        ///     Extracts the imaginary part of a complex sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> Imaginary(this IEnumerable<Complex> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => c.Imaginary);
        }

        /// <summary>
        ///     Converts a complex-valued vector to a real-valued vector containing the real and imaginary parts in an alternating
        ///     pattern.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public static IEnumerable<double> Interleave(this IEnumerable<Complex> c)
        {
            if (c == null)
                throw new ArgumentNullException(nameof(c));

            foreach (var complex in c)
            {
                yield return complex.Real;
                yield return complex.Imaginary;
            }
        }

        /// <summary>
        ///     Extracts the magnitude from a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> Magitude(this IEnumerable<Complex> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => c.Magnitude);
        }

        /// <summary>
        ///     Extracts the phase from a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> Phase(this IEnumerable<Complex> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => c.Phase);
        }

        /// <summary>
        ///     Extracts the real part from a complex-valued vector.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <returns></returns>
        public static IEnumerable<double> Real(this IEnumerable<Complex> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => c.Real);
        }

        /// <summary>
        ///     Creates a new complex sequence from a sequence of real parts.
        /// </summary>
        /// <param name="input">The real-valued input sequence.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> ToComplex(this IEnumerable<double> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(d => new Complex(d, 0));
        }

        /// <summary>
        ///     Creates a complex-valued vector from an interleaved real-valued vector containing real and imaginary parts in an
        ///     alternating pattern.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> UnInterleaveComplex(this IEnumerable<double> d)
        {
            if (d == null)
                throw new ArgumentNullException(nameof(d));

            using (var enumerator = d.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var real = enumerator.Current;
                    enumerator.MoveNext();
                    yield return new Complex(real, enumerator.Current);
                }
            }
        }
    }
}