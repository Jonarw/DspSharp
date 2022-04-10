using DspSharp.Extensions;
using DspSharp.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DspSharp.Algorithms
{
    public static partial class ComplexVectors
    {
        /// <summary>
        /// Calculates the real part of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<double> Real(this IEnumerable<Complex> input)
        {
            return input.Select(c => c.Real);
        }

        /// <summary>
        /// Calculates the real part of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<double> Real(this IReadOnlyCollection<Complex> input)
        {
            return input.SelectWithCount(c => c.Real);
        }

        /// <summary>
        /// Calculates the real part of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<double> Real(this IReadOnlyList<Complex> input)
        {
            return input.SelectIndexed(c => c.Real);
        }

        /// <summary>
        /// Calculates the imaginary part of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<double> Imaginary(this IEnumerable<Complex> input)
        {
            return input.Select(c => c.Imaginary);
        }

        /// <summary>
        /// Calculates the imaginary part of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<double> Imaginary(this IReadOnlyCollection<Complex> input)
        {
            return input.SelectWithCount(c => c.Imaginary);
        }

        /// <summary>
        /// Calculates the imaginary part of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<double> Imaginary(this IReadOnlyList<Complex> input)
        {
            return input.SelectIndexed(c => c.Imaginary);
        }

        /// <summary>
        /// Calculates the magnitude of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<double> Magnitude(this IEnumerable<Complex> input)
        {
            return input.Select(c => c.Magnitude);
        }

        /// <summary>
        /// Calculates the magnitude of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<double> Magnitude(this IReadOnlyCollection<Complex> input)
        {
            return input.SelectWithCount(c => c.Magnitude);
        }

        /// <summary>
        /// Calculates the magnitude of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<double> Magnitude(this IReadOnlyList<Complex> input)
        {
            return input.SelectIndexed(c => c.Magnitude);
        }

        /// <summary>
        /// Calculates the phase of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<double> Phase(this IEnumerable<Complex> input)
        {
            return input.Select(c => c.Phase);
        }

        /// <summary>
        /// Calculates the phase of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<double> Phase(this IReadOnlyCollection<Complex> input)
        {
            return input.SelectWithCount(c => c.Phase);
        }

        /// <summary>
        /// Calculates the phase of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<double> Phase(this IReadOnlyList<Complex> input)
        {
            return input.SelectIndexed(c => c.Phase);
        }

        /// <summary>
        /// Calculates the complex conjugate of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static IEnumerable<Complex> ComplexConjugate(this IEnumerable<Complex> input)
        {
            return input.Select(c => new Complex(c.Real, -c.Imaginary));
        }

        /// <summary>
        /// Calculates the complex conjugate of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyCollection<Complex> ComplexConjugate(this IReadOnlyCollection<Complex> input)
        {
            return input.SelectWithCount(c => new Complex(c.Real, -c.Imaginary));
        }

        /// <summary>
        /// Calculates the complex conjugate of a complex-valued sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        public static ILazyReadOnlyList<Complex> ComplexConjugate(this IReadOnlyList<Complex> input)
        {
            return input.SelectIndexed(c => new Complex(c.Real, -c.Imaginary));
        }

    }
}