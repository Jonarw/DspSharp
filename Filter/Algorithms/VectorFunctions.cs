using System;
using System.Collections.Generic;
using System.Linq;

namespace Filter.Algorithms
{
    public static class VectorFunctions
    {
        /// <summary>
        ///     Calculates the energy of a sequence by summing up its squared elements.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <returns></returns>
        public static double CalculateEnergy(this IEnumerable<double> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Aggregate(0.0, (d, d1) => d + d1 * d1);
        }

        /// <summary>
        ///     Computes the logarithm of a sequence element-wise.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="newBase">The base.</param>
        /// <returns></returns>
        public static IEnumerable<double> Log(this IEnumerable<double> input, double newBase = 10)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (newBase <= 0)
                throw new ArgumentOutOfRangeException(nameof(newBase));

            return input.Select(d => Math.Log(d, newBase));
        }

        /// <summary>
        ///     Finds the index with the maximum value in a sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <returns></returns>
        public static int MaxIndex<T>(this IEnumerable<T> sequence)
            where T : IComparable<T>
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            var sequencelist = sequence.ToReadOnlyList();
            if (sequencelist.Count == 0)
                return -1;

            var index = -1;
            var maxValue = sequencelist.First(); // Immediately overwritten anyway

            return sequencelist.Aggregate(
                0,
                (i, value) =>
                {
                    index++;
                    if (value.CompareTo(maxValue) > 0)
                    {
                        maxValue = value;
                        return index;
                    }

                    return i;
                });
        }

        /// <summary>
        ///     Finds the index with the minimum value in a sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <returns></returns>
        public static int MinIndex<T>(this IEnumerable<T> sequence)
            where T : IComparable<T>
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            var sequencelist = sequence.ToReadOnlyList();
            if (sequencelist.Count == 0)
                return -1;

            var index = -1;
            var minValue = sequencelist.First(); // Immediately overwritten anyway

            return sequencelist.Aggregate(
                0,
                (i, value) =>
                {
                    index++;
                    if (value.CompareTo(minValue) < 0)
                    {
                        minValue = value;
                        return index;
                    }

                    return i;
                });
        }

        /// <summary>
        ///     Raises the elements of a sequence to the specified power.
        /// </summary>
        /// <param name="power">The power.</param>
        /// <param name="input">The sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> Pow(this IEnumerable<double> input, double power)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Select(d => Math.Pow(power, d));
        }
    }
}