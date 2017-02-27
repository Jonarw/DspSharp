using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Filter.Extensions;

namespace Filter.Algorithms
{
    public static class VectorOperations
    {

        /// <summary>
        ///     Applies a circular shift to the provided sequence.
        /// </summary>
        /// <param name="series">The sequence.</param>
        /// <param name="amount">The shift amount.</param>
        /// <returns></returns>
        public static IEnumerable<double> CircularShift(this IEnumerable<double> series, int amount)
        {
            if (series == null)
                throw new ArgumentNullException(nameof(series));

            var list = series.ToReadOnlyList();
            amount = Mathematic.Mod(amount, list.Count);
            return list.Skip(amount).Concat(list.Take(amount));
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
                return Enumerable.Concat(list.GetRangeOptimized(start, list.Count - start), list.Take(stop)).ZeroPad(length - list.Count);
            }

            stop = Mathematic.Mod(start + length, list.Count);

            if (start < stop)
                return list.GetRangeOptimized(start, stop);

            return Enumerable.Concat(list.GetRangeOptimized(start, list.Count - start), list.Take(stop));
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
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            int c = start;
            int i = 0;
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
        ///     Gets a range from a sequence, taking advantage of indexed access if the sequence type supports it.
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
                int c = 0;

                using (var e = input.Skip(startindex).GetEnumerator())
                {
                    while (e.MoveNext() && c++ < length)
                    {
                        yield return e.Current;
                    }
                }
                yield break;
            }

            int stop = Math.Min(startindex + length, inputlist.Count);

            for (int i = startindex; i < stop; i++)
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
        ///     Zeropads a sequence at the start.
        /// </summary>
        /// <param name="d">The sequence.</param>
        /// <param name="n">The number of leading zeros.</param>
        /// <returns></returns>
        public static IEnumerable<double> RightShift(this IEnumerable<double> d, int n)
        {
            if (d == null)
                throw new ArgumentNullException(nameof(d));

            return Enumerable.Repeat(0.0, n).Concat(d);
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

            for (int i = 0; i != loops; i++)
            {
                foreach (var element in source)
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        ///     Computes a sub-portion of a sequence by only including every xth element.
        /// </summary>
        /// <param name="series">The sequence.</param>
        /// <param name="sparseFactor">The sparse factor.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> SparseSeries(this IEnumerable<Complex> series, int sparseFactor)
        {
            if (series == null)
                throw new ArgumentNullException(nameof(series));

            using (var e = series.GetEnumerator())
            {
                while (true)
                {
                    for (int i = 0; i < sparseFactor; i++)
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
                int c = 0;
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

        /// <summary>
        ///     Zeropads a sequence.
        /// </summary>
        /// <param name="d">The sequence.</param>
        /// <param name="n">The total length of the resulting sequence.</param>
        /// <returns></returns>
        public static IEnumerable<double> ZeroPad(this IEnumerable<double> d, int n)
        {
            if (d == null)
                throw new ArgumentNullException(nameof(d));

            int i = 0;

            using (var enumerator = d.GetEnumerator())
            {
                while (enumerator.MoveNext() && i++ < n)
                {
                    yield return enumerator.Current;
                }
            }

            while (i++ < n)
            {
                yield return 0;
            }
        }

        /// <summary>
        ///     Zeropads a sequence.
        /// </summary>
        /// <param name="d">The sequence.</param>
        /// <param name="n">The total length of the resulting sequence.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> ZeroPad(this IEnumerable<Complex> d, int n)
        {
            if (d == null)
                throw new ArgumentNullException(nameof(d));

            int i = 0;

            using (var enumerator = d.GetEnumerator())
            {
                while (enumerator.MoveNext() && i++ < n)
                {
                    yield return enumerator.Current;
                }
            }

            while (i++ < n)
            {
                yield return 0;
            }
        }
    }
}
