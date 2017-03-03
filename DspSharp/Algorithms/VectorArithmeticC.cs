// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VectorArithmeticC.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DspSharp.Algorithms
{
    public static class VectorArithmeticC
    {
        /// <summary>
        ///     Adds two complex-valued sequences element-wise. The longer sequence is truncated to the length of the shorter
        ///     sequence.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Add(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input2 == null)
                throw new ArgumentNullException(nameof(input2));

            return input.Zip(input2, (d, d1) => d + d1);
        }

        /// <summary>
        ///     Adds a scalar to a complex-valued sequence.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Add(this IEnumerable<Complex> input, Complex scalar)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => c + scalar);
        }

        /// <summary>
        ///     Divides two complex-valued sequences element-wise. The longer sequence is truncated to the length of the shorter
        ///     sequence.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Divide(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input2 == null)
                throw new ArgumentNullException(nameof(input2));

            return input.Zip(input2, (d, d1) => d / d1);
        }

        /// <summary>
        ///     Divides a scalar by a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Divide(this Complex scalar, IEnumerable<Complex> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => scalar / c);
        }

        /// <summary>
        ///     Multiplies two complex-valued sequences element-wise. The longer sequence is truncated to the length of the shorter
        ///     sequence.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Multiply(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input2 == null)
                throw new ArgumentNullException(nameof(input2));

            return input.Zip(input2, (d, d1) => d * d1);
        }

        /// <summary>
        ///     Multiplies a complex-valued sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Multiply(this IEnumerable<Complex> input, Complex scalar)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => c * scalar);
        }

        /// <summary>
        ///     Negates a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Negate(this IEnumerable<Complex> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => -c);
        }

        /// <summary>
        ///     Subtracts two complex-valued sequences element-wise. The longer sequence is truncated to the length of the shorter
        ///     sequence.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Subtract(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input2 == null)
                throw new ArgumentNullException(nameof(input2));

            return input.Zip(input2, (d, d1) => d - d1);
        }

        /// <summary>
        ///     Subtracts a complex-valued sequence from a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Subtract(this Complex scalar, IEnumerable<Complex> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => scalar - c);
        }
    }
}