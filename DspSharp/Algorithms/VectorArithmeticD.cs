// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VectorArithmeticD.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Algorithms
{
    public static class VectorArithmeticD
    {
        /// <summary>
        ///     Adds two real-valued sequences element-wise. The longer sequence is truncated to the length of the shorter
        ///     sequence.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> Add(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input2 == null)
                throw new ArgumentNullException(nameof(input2));

            return input.Zip(input2, (d, d1) => d + d1);
        }

        /// <summary>
        ///     Adds a scalar to a real-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<double> Add(this IEnumerable<double> input, double scalar)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => c + scalar);
        }

        /// <summary>
        ///     Adds two real-valued sequences element-wise. The shorter sequence is zero-padded to the length of the longer
        ///     sequence.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> AddFull(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input2 == null)
                throw new ArgumentNullException(nameof(input2));

            using (var enumerator = input.GetEnumerator())
            using (var enumerator2 = input2.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator2.MoveNext())
                        yield return enumerator.Current + enumerator2.Current;
                    else
                    {
                        yield return enumerator.Current;
                        while (enumerator.MoveNext())
                        {
                            yield return enumerator.Current;
                        }

                        yield break;
                    }
                }

                while (enumerator2.MoveNext())
                {
                    yield return enumerator2.Current;
                }
            }
        }

        /// <summary>
        ///     Adds two real-valued sequences element-wise with an offset. The offset is achieved through zero-padding at the
        ///     beginning of the second sequence. The shorter sequence (including the offset to the second sequence) is zero-padded
        ///     to match the length of the
        ///     longer sequence.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        /// <param name="offset">
        ///     The offset. Positive values delay the second sequence relative to the first sequence and
        ///     vice-versa.
        /// </param>
        /// <returns></returns>
        public static IEnumerable<double> AddFullWithOffset(
            this IEnumerable<double> input,
            IEnumerable<double> input2,
            int offset)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input2 == null)
                throw new ArgumentNullException(nameof(input2));

            using (var enumerator = input.GetEnumerator())
            using (var enumerator2 = input2.GetEnumerator())
            {
                int c;

                if (offset < 0)
                {
                    c = offset;
                    while ((c++ < 0) && enumerator2.MoveNext())
                    {
                        yield return enumerator2.Current;
                    }

                    c--;
                    while (c++ < 0)
                    {
                        yield return 0d;
                    }
                }

                c = 0;
                while (enumerator.MoveNext())
                {
                    if (c++ < offset)
                        yield return enumerator.Current;
                    else
                    {
                        if (enumerator2.MoveNext())
                            yield return enumerator.Current + enumerator2.Current;
                        else
                        {
                            yield return enumerator.Current;
                            while (enumerator.MoveNext())
                            {
                                yield return enumerator.Current;
                            }

                            yield break;
                        }
                    }
                }

                while (c++ < offset)
                {
                    yield return 0.0;
                }

                while (enumerator2.MoveNext())
                {
                    yield return enumerator2.Current;
                }
            }
        }

        /// <summary>
        ///     Divides two real-valued sequences element-wise. The longer sequence is truncated to the length of the shorter
        ///     sequence.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> Divide(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input2 == null)
                throw new ArgumentNullException(nameof(input2));

            return input.Zip(input2, (d, d1) => d / d1);
        }

        /// <summary>
        ///     Divides a scalar by a real-valued sequence.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <param name="scalar">The sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> Divide(this double scalar, IEnumerable<double> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => scalar / c);
        }

        /// <summary>
        ///     Multiplies a real-valued sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<double> Divide(this IEnumerable<double> input, double scalar)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Multiply(1 / scalar);
        }

        /// <summary>
        ///     Multiplies two real-valued sequences element-wise. The longer sequence is truncated to the length of the shorter
        ///     sequence.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> Multiply(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input2 == null)
                throw new ArgumentNullException(nameof(input2));

            return input.Zip(input2, (d, d1) => d * d1);
        }

        /// <summary>
        ///     Multiplies a real-valued sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<double> Multiply(this IEnumerable<double> input, double scalar)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => c * scalar);
        }

        /// <summary>
        ///     Negates a real-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> Negate(this IEnumerable<double> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => -c);
        }

        /// <summary>
        ///     Subtracts two real-valued sequences element-wise. The longer sequence is truncated to the length of the shorter
        ///     sequence.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> Subtract(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input2 == null)
                throw new ArgumentNullException(nameof(input2));

            return input.Zip(input2, (d, d1) => d - d1);
        }

        /// <summary>
        ///     Subtracts a real-valued sequence from a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<double> Subtract(this double scalar, IEnumerable<double> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => scalar - c);
        }

        /// <summary>
        ///     Subtracts a real-valued sequence from a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<double> Subtract(this IEnumerable<double> input, double scalar)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(c => c - scalar);
        }

        /// <summary>
        ///     Subtracts two real-valued sequences element-wise. The shorter sequence is zero-padded to the length of the longer
        ///     sequence.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> SubtractFull(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input2 == null)
                throw new ArgumentNullException(nameof(input2));

            using (var enumerator = input.GetEnumerator())
            using (var enumerator2 = input2.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator2.MoveNext())
                        yield return enumerator.Current - enumerator2.Current;
                    else
                    {
                        yield return enumerator.Current;
                        while (enumerator.MoveNext())
                        {
                            yield return enumerator.Current;
                        }
                        yield break;
                    }
                }

                while (enumerator2.MoveNext())
                {
                    yield return -enumerator2.Current;
                }
            }
        }
    }
}