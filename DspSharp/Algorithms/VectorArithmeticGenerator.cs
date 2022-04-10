using DspSharp.Extensions;
using DspSharp.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DspSharp.Algorithms
{
    public static class VectorArithmetic
    {
        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<float> Add(this IEnumerable<float> input, IEnumerable<float> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<float> Add(this IReadOnlyCollection<float> input, IReadOnlyCollection<float> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<float> Add(this IReadOnlyList<float> input, IReadOnlyList<float> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<float> Add(this IEnumerable<float> input, float scalar)
        {
            return input.Select(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<float> Add(this IReadOnlyCollection<float> input, float scalar)
        {
            return input.SelectWithCount(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<float> Add(this IReadOnlyList<float> input, float scalar)
        {
            return input.SelectIndexed(c => c + scalar);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<float> Subtract(this IEnumerable<float> input, IEnumerable<float> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<float> Subtract(this IReadOnlyCollection<float> input, IReadOnlyCollection<float> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<float> Subtract(this IReadOnlyList<float> input, IReadOnlyList<float> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<float> Subtract(this IEnumerable<float> input, float scalar)
        {
            return input.Select(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<float> Subtract(this IReadOnlyCollection<float> input, float scalar)
        {
            return input.SelectWithCount(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<float> Subtract(this IReadOnlyList<float> input, float scalar)
        {
            return input.SelectIndexed(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<float> Subtract(this float scalar, IEnumerable<float> input)
        {
            return input.Select(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<float> Subtract(this float scalar, IReadOnlyCollection<float> input)
        {
            return input.SelectWithCount(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<float> Subtract(this float scalar, IReadOnlyList<float> input)
        {
            return input.SelectIndexed(c => scalar - c);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<float> Multiply(this IEnumerable<float> input, IEnumerable<float> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<float> Multiply(this IReadOnlyCollection<float> input, IReadOnlyCollection<float> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<float> Multiply(this IReadOnlyList<float> input, IReadOnlyList<float> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<float> Multiply(this IEnumerable<float> input, float scalar)
        {
            return input.Select(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<float> Multiply(this IReadOnlyCollection<float> input, float scalar)
        {
            return input.SelectWithCount(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<float> Multiply(this IReadOnlyList<float> input, float scalar)
        {
            return input.SelectIndexed(c => c * scalar);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<float> Divide(this IEnumerable<float> input, IEnumerable<float> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<float> Divide(this IReadOnlyCollection<float> input, IReadOnlyCollection<float> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<float> Divide(this IReadOnlyList<float> input, IReadOnlyList<float> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<float> Divide(this IEnumerable<float> input, float scalar)
        {
            return input.Select(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<float> Divide(this IReadOnlyCollection<float> input, float scalar)
        {
            return input.SelectWithCount(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<float> Divide(this IReadOnlyList<float> input, float scalar)
        {
            return input.SelectIndexed(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<float> Divide(this float scalar, IEnumerable<float> input)
        {
            return input.Select(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<float> Divide(this float scalar, IReadOnlyCollection<float> input)
        {
            return input.SelectWithCount(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<float> Divide(this float scalar, IReadOnlyList<float> input)
        {
            return input.SelectIndexed(c => scalar / c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<float> Negate(this IEnumerable<float> input)
        {
            return input.Select(c => -c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<float> Negate(this IReadOnlyCollection<float> input)
        {
            return input.SelectWithCount(c => -c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<float> Negate(this IReadOnlyList<float> input)
        {
            return input.SelectIndexed(c => -c);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<double> Add(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<double> Add(this IReadOnlyCollection<double> input, IReadOnlyCollection<double> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<double> Add(this IReadOnlyList<double> input, IReadOnlyList<double> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<double> Add(this IEnumerable<double> input, double scalar)
        {
            return input.Select(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<double> Add(this IReadOnlyCollection<double> input, double scalar)
        {
            return input.SelectWithCount(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<double> Add(this IReadOnlyList<double> input, double scalar)
        {
            return input.SelectIndexed(c => c + scalar);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<double> Subtract(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<double> Subtract(this IReadOnlyCollection<double> input, IReadOnlyCollection<double> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<double> Subtract(this IReadOnlyList<double> input, IReadOnlyList<double> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<double> Subtract(this IEnumerable<double> input, double scalar)
        {
            return input.Select(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<double> Subtract(this IReadOnlyCollection<double> input, double scalar)
        {
            return input.SelectWithCount(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<double> Subtract(this IReadOnlyList<double> input, double scalar)
        {
            return input.SelectIndexed(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<double> Subtract(this double scalar, IEnumerable<double> input)
        {
            return input.Select(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<double> Subtract(this double scalar, IReadOnlyCollection<double> input)
        {
            return input.SelectWithCount(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<double> Subtract(this double scalar, IReadOnlyList<double> input)
        {
            return input.SelectIndexed(c => scalar - c);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<double> Multiply(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<double> Multiply(this IReadOnlyCollection<double> input, IReadOnlyCollection<double> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<double> Multiply(this IReadOnlyList<double> input, IReadOnlyList<double> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<double> Multiply(this IEnumerable<double> input, double scalar)
        {
            return input.Select(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<double> Multiply(this IReadOnlyCollection<double> input, double scalar)
        {
            return input.SelectWithCount(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<double> Multiply(this IReadOnlyList<double> input, double scalar)
        {
            return input.SelectIndexed(c => c * scalar);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<double> Divide(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<double> Divide(this IReadOnlyCollection<double> input, IReadOnlyCollection<double> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<double> Divide(this IReadOnlyList<double> input, IReadOnlyList<double> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<double> Divide(this IEnumerable<double> input, double scalar)
        {
            return input.Select(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<double> Divide(this IReadOnlyCollection<double> input, double scalar)
        {
            return input.SelectWithCount(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<double> Divide(this IReadOnlyList<double> input, double scalar)
        {
            return input.SelectIndexed(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<double> Divide(this double scalar, IEnumerable<double> input)
        {
            return input.Select(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<double> Divide(this double scalar, IReadOnlyCollection<double> input)
        {
            return input.SelectWithCount(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<double> Divide(this double scalar, IReadOnlyList<double> input)
        {
            return input.SelectIndexed(c => scalar / c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<double> Negate(this IEnumerable<double> input)
        {
            return input.Select(c => -c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<double> Negate(this IReadOnlyCollection<double> input)
        {
            return input.SelectWithCount(c => -c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<double> Negate(this IReadOnlyList<double> input)
        {
            return input.SelectIndexed(c => -c);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<Complex> Add(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<Complex> Add(this IReadOnlyCollection<Complex> input, IReadOnlyCollection<Complex> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<Complex> Add(this IReadOnlyList<Complex> input, IReadOnlyList<Complex> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<Complex> Add(this IEnumerable<Complex> input, Complex scalar)
        {
            return input.Select(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<Complex> Add(this IReadOnlyCollection<Complex> input, Complex scalar)
        {
            return input.SelectWithCount(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<Complex> Add(this IReadOnlyList<Complex> input, Complex scalar)
        {
            return input.SelectIndexed(c => c + scalar);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<Complex> Subtract(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<Complex> Subtract(this IReadOnlyCollection<Complex> input, IReadOnlyCollection<Complex> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<Complex> Subtract(this IReadOnlyList<Complex> input, IReadOnlyList<Complex> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<Complex> Subtract(this IEnumerable<Complex> input, Complex scalar)
        {
            return input.Select(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<Complex> Subtract(this IReadOnlyCollection<Complex> input, Complex scalar)
        {
            return input.SelectWithCount(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<Complex> Subtract(this IReadOnlyList<Complex> input, Complex scalar)
        {
            return input.SelectIndexed(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<Complex> Subtract(this Complex scalar, IEnumerable<Complex> input)
        {
            return input.Select(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<Complex> Subtract(this Complex scalar, IReadOnlyCollection<Complex> input)
        {
            return input.SelectWithCount(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<Complex> Subtract(this Complex scalar, IReadOnlyList<Complex> input)
        {
            return input.SelectIndexed(c => scalar - c);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<Complex> Multiply(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<Complex> Multiply(this IReadOnlyCollection<Complex> input, IReadOnlyCollection<Complex> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<Complex> Multiply(this IReadOnlyList<Complex> input, IReadOnlyList<Complex> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<Complex> Multiply(this IEnumerable<Complex> input, Complex scalar)
        {
            return input.Select(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<Complex> Multiply(this IReadOnlyCollection<Complex> input, Complex scalar)
        {
            return input.SelectWithCount(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<Complex> Multiply(this IReadOnlyList<Complex> input, Complex scalar)
        {
            return input.SelectIndexed(c => c * scalar);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<Complex> Divide(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<Complex> Divide(this IReadOnlyCollection<Complex> input, IReadOnlyCollection<Complex> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<Complex> Divide(this IReadOnlyList<Complex> input, IReadOnlyList<Complex> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<Complex> Divide(this IEnumerable<Complex> input, Complex scalar)
        {
            return input.Select(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<Complex> Divide(this IReadOnlyCollection<Complex> input, Complex scalar)
        {
            return input.SelectWithCount(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<Complex> Divide(this IReadOnlyList<Complex> input, Complex scalar)
        {
            return input.SelectIndexed(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<Complex> Divide(this Complex scalar, IEnumerable<Complex> input)
        {
            return input.Select(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<Complex> Divide(this Complex scalar, IReadOnlyCollection<Complex> input)
        {
            return input.SelectWithCount(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<Complex> Divide(this Complex scalar, IReadOnlyList<Complex> input)
        {
            return input.SelectIndexed(c => scalar / c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<Complex> Negate(this IEnumerable<Complex> input)
        {
            return input.Select(c => -c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<Complex> Negate(this IReadOnlyCollection<Complex> input)
        {
            return input.SelectWithCount(c => -c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<Complex> Negate(this IReadOnlyList<Complex> input)
        {
            return input.SelectIndexed(c => -c);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<int> Add(this IEnumerable<int> input, IEnumerable<int> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<int> Add(this IReadOnlyCollection<int> input, IReadOnlyCollection<int> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<int> Add(this IReadOnlyList<int> input, IReadOnlyList<int> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<int> Add(this IEnumerable<int> input, int scalar)
        {
            return input.Select(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<int> Add(this IReadOnlyCollection<int> input, int scalar)
        {
            return input.SelectWithCount(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<int> Add(this IReadOnlyList<int> input, int scalar)
        {
            return input.SelectIndexed(c => c + scalar);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<int> Subtract(this IEnumerable<int> input, IEnumerable<int> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<int> Subtract(this IReadOnlyCollection<int> input, IReadOnlyCollection<int> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<int> Subtract(this IReadOnlyList<int> input, IReadOnlyList<int> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<int> Subtract(this IEnumerable<int> input, int scalar)
        {
            return input.Select(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<int> Subtract(this IReadOnlyCollection<int> input, int scalar)
        {
            return input.SelectWithCount(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<int> Subtract(this IReadOnlyList<int> input, int scalar)
        {
            return input.SelectIndexed(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<int> Subtract(this int scalar, IEnumerable<int> input)
        {
            return input.Select(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<int> Subtract(this int scalar, IReadOnlyCollection<int> input)
        {
            return input.SelectWithCount(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<int> Subtract(this int scalar, IReadOnlyList<int> input)
        {
            return input.SelectIndexed(c => scalar - c);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<int> Multiply(this IEnumerable<int> input, IEnumerable<int> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<int> Multiply(this IReadOnlyCollection<int> input, IReadOnlyCollection<int> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<int> Multiply(this IReadOnlyList<int> input, IReadOnlyList<int> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<int> Multiply(this IEnumerable<int> input, int scalar)
        {
            return input.Select(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<int> Multiply(this IReadOnlyCollection<int> input, int scalar)
        {
            return input.SelectWithCount(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<int> Multiply(this IReadOnlyList<int> input, int scalar)
        {
            return input.SelectIndexed(c => c * scalar);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<int> Divide(this IEnumerable<int> input, IEnumerable<int> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<int> Divide(this IReadOnlyCollection<int> input, IReadOnlyCollection<int> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<int> Divide(this IReadOnlyList<int> input, IReadOnlyList<int> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<int> Divide(this IEnumerable<int> input, int scalar)
        {
            return input.Select(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<int> Divide(this IReadOnlyCollection<int> input, int scalar)
        {
            return input.SelectWithCount(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<int> Divide(this IReadOnlyList<int> input, int scalar)
        {
            return input.SelectIndexed(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<int> Divide(this int scalar, IEnumerable<int> input)
        {
            return input.Select(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<int> Divide(this int scalar, IReadOnlyCollection<int> input)
        {
            return input.SelectWithCount(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<int> Divide(this int scalar, IReadOnlyList<int> input)
        {
            return input.SelectIndexed(c => scalar / c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<int> Negate(this IEnumerable<int> input)
        {
            return input.Select(c => -c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<int> Negate(this IReadOnlyCollection<int> input)
        {
            return input.SelectWithCount(c => -c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<int> Negate(this IReadOnlyList<int> input)
        {
            return input.SelectIndexed(c => -c);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<uint> Add(this IEnumerable<uint> input, IEnumerable<uint> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<uint> Add(this IReadOnlyCollection<uint> input, IReadOnlyCollection<uint> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<uint> Add(this IReadOnlyList<uint> input, IReadOnlyList<uint> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<uint> Add(this IEnumerable<uint> input, uint scalar)
        {
            return input.Select(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<uint> Add(this IReadOnlyCollection<uint> input, uint scalar)
        {
            return input.SelectWithCount(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<uint> Add(this IReadOnlyList<uint> input, uint scalar)
        {
            return input.SelectIndexed(c => c + scalar);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<uint> Subtract(this IEnumerable<uint> input, IEnumerable<uint> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<uint> Subtract(this IReadOnlyCollection<uint> input, IReadOnlyCollection<uint> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<uint> Subtract(this IReadOnlyList<uint> input, IReadOnlyList<uint> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<uint> Subtract(this IEnumerable<uint> input, uint scalar)
        {
            return input.Select(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<uint> Subtract(this IReadOnlyCollection<uint> input, uint scalar)
        {
            return input.SelectWithCount(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<uint> Subtract(this IReadOnlyList<uint> input, uint scalar)
        {
            return input.SelectIndexed(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<uint> Subtract(this uint scalar, IEnumerable<uint> input)
        {
            return input.Select(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<uint> Subtract(this uint scalar, IReadOnlyCollection<uint> input)
        {
            return input.SelectWithCount(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<uint> Subtract(this uint scalar, IReadOnlyList<uint> input)
        {
            return input.SelectIndexed(c => scalar - c);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<uint> Multiply(this IEnumerable<uint> input, IEnumerable<uint> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<uint> Multiply(this IReadOnlyCollection<uint> input, IReadOnlyCollection<uint> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<uint> Multiply(this IReadOnlyList<uint> input, IReadOnlyList<uint> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<uint> Multiply(this IEnumerable<uint> input, uint scalar)
        {
            return input.Select(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<uint> Multiply(this IReadOnlyCollection<uint> input, uint scalar)
        {
            return input.SelectWithCount(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<uint> Multiply(this IReadOnlyList<uint> input, uint scalar)
        {
            return input.SelectIndexed(c => c * scalar);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<uint> Divide(this IEnumerable<uint> input, IEnumerable<uint> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<uint> Divide(this IReadOnlyCollection<uint> input, IReadOnlyCollection<uint> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<uint> Divide(this IReadOnlyList<uint> input, IReadOnlyList<uint> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<uint> Divide(this IEnumerable<uint> input, uint scalar)
        {
            return input.Select(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<uint> Divide(this IReadOnlyCollection<uint> input, uint scalar)
        {
            return input.SelectWithCount(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<uint> Divide(this IReadOnlyList<uint> input, uint scalar)
        {
            return input.SelectIndexed(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<uint> Divide(this uint scalar, IEnumerable<uint> input)
        {
            return input.Select(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<uint> Divide(this uint scalar, IReadOnlyCollection<uint> input)
        {
            return input.SelectWithCount(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<uint> Divide(this uint scalar, IReadOnlyList<uint> input)
        {
            return input.SelectIndexed(c => scalar / c);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<long> Add(this IEnumerable<long> input, IEnumerable<long> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<long> Add(this IReadOnlyCollection<long> input, IReadOnlyCollection<long> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<long> Add(this IReadOnlyList<long> input, IReadOnlyList<long> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<long> Add(this IEnumerable<long> input, long scalar)
        {
            return input.Select(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<long> Add(this IReadOnlyCollection<long> input, long scalar)
        {
            return input.SelectWithCount(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<long> Add(this IReadOnlyList<long> input, long scalar)
        {
            return input.SelectIndexed(c => c + scalar);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<long> Subtract(this IEnumerable<long> input, IEnumerable<long> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<long> Subtract(this IReadOnlyCollection<long> input, IReadOnlyCollection<long> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<long> Subtract(this IReadOnlyList<long> input, IReadOnlyList<long> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<long> Subtract(this IEnumerable<long> input, long scalar)
        {
            return input.Select(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<long> Subtract(this IReadOnlyCollection<long> input, long scalar)
        {
            return input.SelectWithCount(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<long> Subtract(this IReadOnlyList<long> input, long scalar)
        {
            return input.SelectIndexed(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<long> Subtract(this long scalar, IEnumerable<long> input)
        {
            return input.Select(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<long> Subtract(this long scalar, IReadOnlyCollection<long> input)
        {
            return input.SelectWithCount(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<long> Subtract(this long scalar, IReadOnlyList<long> input)
        {
            return input.SelectIndexed(c => scalar - c);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<long> Multiply(this IEnumerable<long> input, IEnumerable<long> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<long> Multiply(this IReadOnlyCollection<long> input, IReadOnlyCollection<long> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<long> Multiply(this IReadOnlyList<long> input, IReadOnlyList<long> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<long> Multiply(this IEnumerable<long> input, long scalar)
        {
            return input.Select(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<long> Multiply(this IReadOnlyCollection<long> input, long scalar)
        {
            return input.SelectWithCount(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<long> Multiply(this IReadOnlyList<long> input, long scalar)
        {
            return input.SelectIndexed(c => c * scalar);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<long> Divide(this IEnumerable<long> input, IEnumerable<long> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<long> Divide(this IReadOnlyCollection<long> input, IReadOnlyCollection<long> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<long> Divide(this IReadOnlyList<long> input, IReadOnlyList<long> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<long> Divide(this IEnumerable<long> input, long scalar)
        {
            return input.Select(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<long> Divide(this IReadOnlyCollection<long> input, long scalar)
        {
            return input.SelectWithCount(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<long> Divide(this IReadOnlyList<long> input, long scalar)
        {
            return input.SelectIndexed(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<long> Divide(this long scalar, IEnumerable<long> input)
        {
            return input.Select(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<long> Divide(this long scalar, IReadOnlyCollection<long> input)
        {
            return input.SelectWithCount(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<long> Divide(this long scalar, IReadOnlyList<long> input)
        {
            return input.SelectIndexed(c => scalar / c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<long> Negate(this IEnumerable<long> input)
        {
            return input.Select(c => -c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<long> Negate(this IReadOnlyCollection<long> input)
        {
            return input.SelectWithCount(c => -c);
        }

        /// <summary>
        /// Negates a sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<long> Negate(this IReadOnlyList<long> input)
        {
            return input.SelectIndexed(c => -c);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<ulong> Add(this IEnumerable<ulong> input, IEnumerable<ulong> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<ulong> Add(this IReadOnlyCollection<ulong> input, IReadOnlyCollection<ulong> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<ulong> Add(this IReadOnlyList<ulong> input, IReadOnlyList<ulong> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 + c2);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<ulong> Add(this IEnumerable<ulong> input, ulong scalar)
        {
            return input.Select(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<ulong> Add(this IReadOnlyCollection<ulong> input, ulong scalar)
        {
            return input.SelectWithCount(c => c + scalar);
        }

        /// <summary>
        /// Adds a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<ulong> Add(this IReadOnlyList<ulong> input, ulong scalar)
        {
            return input.SelectIndexed(c => c + scalar);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<ulong> Subtract(this IEnumerable<ulong> input, IEnumerable<ulong> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<ulong> Subtract(this IReadOnlyCollection<ulong> input, IReadOnlyCollection<ulong> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<ulong> Subtract(this IReadOnlyList<ulong> input, IReadOnlyList<ulong> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 - c2);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<ulong> Subtract(this IEnumerable<ulong> input, ulong scalar)
        {
            return input.Select(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<ulong> Subtract(this IReadOnlyCollection<ulong> input, ulong scalar)
        {
            return input.SelectWithCount(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<ulong> Subtract(this IReadOnlyList<ulong> input, ulong scalar)
        {
            return input.SelectIndexed(c => c - scalar);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<ulong> Subtract(this ulong scalar, IEnumerable<ulong> input)
        {
            return input.Select(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<ulong> Subtract(this ulong scalar, IReadOnlyCollection<ulong> input)
        {
            return input.SelectWithCount(c => scalar - c);
        }

        /// <summary>
        /// Subtracts a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<ulong> Subtract(this ulong scalar, IReadOnlyList<ulong> input)
        {
            return input.SelectIndexed(c => scalar - c);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<ulong> Multiply(this IEnumerable<ulong> input, IEnumerable<ulong> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<ulong> Multiply(this IReadOnlyCollection<ulong> input, IReadOnlyCollection<ulong> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<ulong> Multiply(this IReadOnlyList<ulong> input, IReadOnlyList<ulong> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 * c2);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<ulong> Multiply(this IEnumerable<ulong> input, ulong scalar)
        {
            return input.Select(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<ulong> Multiply(this IReadOnlyCollection<ulong> input, ulong scalar)
        {
            return input.SelectWithCount(c => c * scalar);
        }

        /// <summary>
        /// Multiplies a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<ulong> Multiply(this IReadOnlyList<ulong> input, ulong scalar)
        {
            return input.SelectIndexed(c => c * scalar);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static IEnumerable<ulong> Divide(this IEnumerable<ulong> input, IEnumerable<ulong> input2)
        {
            return input.ZipExact(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyCollection<ulong> Divide(this IReadOnlyCollection<ulong> input, IReadOnlyCollection<ulong> input2)
        {
            return input.ZipWithCount(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides two sequences element-wise. The sequences must be the same length.
        /// </summary>
        /// <param name="input">The first sequence.</param>
        /// <param name="input2">The second sequence.</param>
        public static ILazyReadOnlyList<ulong> Divide(this IReadOnlyList<ulong> input, IReadOnlyList<ulong> input2)
        {
            return input.ZipIndexed(input2, (c1, c2) => c1 / c2);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static IEnumerable<ulong> Divide(this IEnumerable<ulong> input, ulong scalar)
        {
            return input.Select(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyCollection<ulong> Divide(this IReadOnlyCollection<ulong> input, ulong scalar)
        {
            return input.SelectWithCount(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="scalar">The scalar.</param>
        public static ILazyReadOnlyList<ulong> Divide(this IReadOnlyList<ulong> input, ulong scalar)
        {
            return input.SelectIndexed(c => c / scalar);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<ulong> Divide(this ulong scalar, IEnumerable<ulong> input)
        {
            return input.Select(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<ulong> Divide(this ulong scalar, IReadOnlyCollection<ulong> input)
        {
            return input.SelectWithCount(c => scalar / c);
        }

        /// <summary>
        /// Divides a sequence with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<ulong> Divide(this ulong scalar, IReadOnlyList<ulong> input)
        {
            return input.SelectIndexed(c => scalar / c);
        }

    }
}