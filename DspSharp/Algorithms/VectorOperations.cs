// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VectorOperations.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Algorithms
{
    public static class VectorOperations
    {
        /// <summary>
        ///     Appends the specified element to the sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <param name="element">The element.</param>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> sequence, T element)
        {
            foreach (var item in sequence)
            {
                yield return item;
            }

            yield return element;
        }

        /// <summary>
        ///     Appends the specified elements to the sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <param name="elements">The elements.</param>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> sequence, params T[] elements)
        {
            foreach (var item in sequence)
            {
                yield return item;
            }

            foreach (var item in elements)
            {
                yield return item;
            }
        }

        /// <summary>
        ///     Performs a circular shift on an array.
        /// </summary>
        /// <param name="input">The array to be circularly shifted.</param>
        /// <param name="offset">
        ///     The amount of samples the array should be shifted. Positive offsets are used for left-shifts while negative
        ///     offsets are used for right-shifts.
        /// </param>
        /// <returns>An array of the same length as <paramref name="input" /> containing the result.</returns>
        public static IEnumerable<T> CircularShift<T>(this IReadOnlyList<T> input, int offset)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Count == 0)
                return Enumerable.Empty<T>();

            offset = Mathematic.Mod(offset, input.Count);

            return input.Skip(offset).Concat(input.Take(offset));
        }

        /// <summary>
        ///     Gets a range from a sequence, wrapping around to zero if reaching the end of the sequence.
        /// </summary>
        /// <param name="fftResult">The sequence.</param>
        /// <param name="start">The start of the range.</param>
        /// <param name="length">The length of the range.</param>
        /// <returns></returns>
        public static IEnumerable<double> GetCircularRange(this IEnumerable<double> fftResult, int start, int length)
        {
            if (fftResult == null)
                throw new ArgumentNullException(nameof(fftResult));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var list = fftResult.ToReadOnlyList();

            start = Mathematic.Mod(start, list.Count);

            int stop;
            if (length > list.Count)
            {
                stop = start == 0 ? list.Count - 1 : start - 1;
                return
                    list.GetRangeOptimized(start, list.Count - start)
                        .Concat(list.Take(stop))
                        .PadRight(length - list.Count);
            }

            stop = Mathematic.Mod(start + length, list.Count);

            if (start < stop)
                return list.GetRangeOptimized(start, stop - start);

            return list.GetRangeOptimized(start, list.Count - start).Concat(list.Take(stop));
        }

        /// <summary>
        ///     Gets a range from a sequence, zero-padding at the start and end if necessary.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="start">The start of the range.</param>
        /// <param name="length">The length of the range.</param>
        /// <returns></returns>
        public static IEnumerable<double> GetPaddedRange(this IEnumerable<double> input, int start, int length)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var c = start;
            var i = 0;
            while (c < 0 && i < length)
            {
                yield return 0.0;
                i++;
                c++;
            }

            c = 0;
            using (var e = input.GetEnumerator())
            {
                while (c < start && e.MoveNext())
                {
                    c++;
                }

                while (c < start)
                {
                    c++;
                }

                while (e.MoveNext() && i < length)
                {
                    yield return e.Current;
                    i++;
                    c++;
                }
            }

            while (i < length)
            {
                yield return 0.0;
                i++;
            }
        }

        /// <summary>
        ///     Gets a range from a sequence, taking advantage of indexed access if the sequence type supports it. If the original
        ///     sequence runs out of elements, the output sequence can be shorter than <see cref="length" />.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="startindex">The start of the range.</param>
        /// <param name="length">The length of the range.</param>
        /// <returns></returns>
        public static IEnumerable<double> GetRangeOptimized(this IEnumerable<double> input, int startindex, int length)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (startindex < 0)
                throw new ArgumentOutOfRangeException(nameof(startindex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var inputlist = input as IReadOnlyList<double>;

            if (inputlist == null)
            {
                var c = 0;

                using (var e = input.Skip(startindex).GetEnumerator())
                {
                    while (e.MoveNext() && c++ < length)
                    {
                        yield return e.Current;
                    }
                }

                yield break;
            }

            var stop = Math.Min(startindex + length, inputlist.Count);

            for (var i = startindex; i < stop; i++)
            {
                yield return inputlist[i];
            }
        }

        /// <summary>
        ///     Interleaves two sequences to a single sequence containing both vectors in an alternating pattern.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <returns></returns>
        public static IEnumerable<T> InterleaveEnumerations<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            using (var enumerator1 = first.GetEnumerator())
            using (var enumerator2 = second.GetEnumerator())
            {
                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    yield return enumerator1.Current;
                    yield return enumerator2.Current;
                }
            }
        }

        /// <summary>
        ///     Loops the provided sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The sequence.</param>
        /// <param name="loops">The number of loops. If smaller than 0, the source is looped indefinitely.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static IEnumerable<T> Loop<T>(this IReadOnlyList<T> source, int loops)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (loops < 0)
                throw new ArgumentOutOfRangeException(nameof(loops));

            if (loops == 0)
                yield break;

            for (var i = 0; i != loops; i++)
            {
                foreach (var element in source)
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        ///     Pads a sequence at the left side with the specified number of pad elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input sequence.</param>
        /// <param name="count">The number of pad elements.</param>
        /// <param name="padElement">The pad element.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T> PadLeft<T>(this IEnumerable<T> input, int count, T padElement = default(T))
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return Enumerable.Repeat(padElement, count).Concat(input);
        }

        /// <summary>
        ///     Pads a sequence at the right side with the specified number of pad elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input sequence.</param>
        /// <param name="count">The number of pad elements.</param>
        /// <param name="padElement">The pad element.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T> PadRight<T>(this IEnumerable<T> input, int count, T padElement = default(T))
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return input.Concat(Enumerable.Repeat(padElement, count));
        }

        /// <summary>
        ///     Prepends the specified elements to the sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <param name="elements">The elements.</param>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> sequence, params T[] elements)
        {
            foreach (var item in elements)
            {
                yield return item;
            }

            foreach (var item in sequence)
            {
                yield return item;
            }
        }

        /// <summary>
        ///     Prepends the specified element to the sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <param name="element">The element.</param>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> sequence, T element)
        {
            yield return element;

            foreach (var item in sequence)
            {
                yield return item;
            }
        }

        /// <summary>
        ///     Computes a sub-portion of a sequence by only including every xth element.
        /// </summary>
        /// <param name="series">The sequence.</param>
        /// <param name="sparseFactor">The sparse factor.</param>
        /// <returns></returns>
        public static IEnumerable<T> SparseSequence<T>(this IEnumerable<T> series, int sparseFactor)
        {
            if (series == null)
                throw new ArgumentNullException(nameof(series));

            using (var e = series.GetEnumerator())
            {
                while (true)
                {
                    for (var i = 0; i < sparseFactor; i++)
                    {
                        if (!e.MoveNext())
                            yield break;
                    }

                    yield return e.Current;
                }
            }
        }

        /// <summary>
        ///     Takes the specified amount of items from a sequence. If the end of the sequence is reached, it is zero-padded.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="length">The amount of items to take.</param>
        /// <returns></returns>
        public static IEnumerable<double> TakeFull(this IEnumerable<double> input, int length)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            using (var e = input.GetEnumerator())
            {
                var c = 0;
                while (e.MoveNext() && c < length)
                {
                    yield return e.Current;
                    c++;
                }

                while (c < length)
                {
                    yield return 0.0;
                    c++;
                }
            }
        }
    }
}