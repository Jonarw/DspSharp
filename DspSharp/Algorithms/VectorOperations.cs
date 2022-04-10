// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VectorOperations.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Exceptions;
using DspSharp.Extensions;
using DspSharp.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Algorithms
{
    public static class VectorOperations
    {
        /// <summary>
        /// Performs a circular shift.
        /// </summary>
        /// <param name="input">The sequence to be circularly shifted.</param>
        /// <param name="offset">
        /// The amount of samples the sequence should be shifted. Positive offsets are used for left-shifts while negative offsets are used for right-shifts.
        /// </param>
        public static IReadOnlyList<T> CircularShift<T>(this IReadOnlyList<T> input, int offset)
        {
            if (input.Count == 0)
                return Array.Empty<T>();

            offset = Mathematic.Mod(offset, input.Count);
            var ret = new T[input.Count];
            var c = 0;

            for (var i = offset; i < input.Count; i++)
                ret[c++] = input[i];

            for (var i = 0; i < offset; i++)
                ret[c++] = input[i];

            return ret;
        }

        /// <summary>
        /// Gets a range from a sequence, wrapping around to zero if reaching the end of the sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="start">The start of the range.</param>
        /// <param name="length">The length of the range.</param>
        /// <remarks>This is evaluated lazily.</remarks>
        public static IReadOnlyCollection<double> GetCircularRange(this IReadOnlyList<double> input, int start, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (input.Count == 0 && length > 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            return Iterator().WithCount(length);

            IEnumerable<double> Iterator()
            {
                var c = 0;
                var i = Mathematic.Mod(start, input.Count);

                while (c < length)
                {
                    for (; i < input.Count && c < length; i++)
                    {
                        yield return input[i];
                        c++;
                    }

                    i = 0;
                }
            }
        }

        /// <summary>
        /// Gets a range from a sequence, zero-padding at the start and end if necessary.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <param name="start">The start of the range.</param>
        /// <param name="length">The length of the range.</param>
        /// <remarks>This is evaluated lazily.</remarks>
        public static IReadOnlyCollection<double> GetPaddedRange(this IReadOnlyList<double> input, int start, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            return Iterator().WithCount(length);

            IEnumerable<double> Iterator()
            {
                var c = 0;
                var i = start;
                while (i < 0 && c < length)
                {
                    yield return 0.0;
                    i++;
                    c++;
                }

                while (i < input.Count && c < length)
                {
                    yield return input[i];
                    i++;
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
        /// Gets a range from a sequence.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="startindex">The start of the range.</param>
        /// <param name="length">The length of the range.</param>
        /// <remarks>This is evaluated lazily.</remarks>
        public static IReadOnlyCollection<double> GetRange(this IReadOnlyList<double> input, int startindex, int length)
        {
            if (startindex < 0)
                throw new ArgumentOutOfRangeException(nameof(startindex));
            if (length < 0 || length > input.Count - startindex)
                throw new ArgumentOutOfRangeException(nameof(length));

            return Iterator().WithCount(length);

            IEnumerable<double> Iterator()
            {
                for (var i = 0; i < length; i++)
                {
                    yield return input[i + startindex];
                }
            }
        }

        /// <summary>
        /// Gets a range from a sequence.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="startindex">The start of the range.</param>
        /// <param name="length">The length of the range.</param>
        /// <remarks>This is evaluated lazily.</remarks>
        public static IReadOnlyCollection<double> GetRange(this IEnumerable<double> input, int startindex, int length)
        {
            if (startindex < 0)
                throw new ArgumentOutOfRangeException(nameof(startindex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            return Iterator().WithCount(length);

            IEnumerable<double> Iterator()
            {
                using var e = input.GetEnumerator();
                for (var i = 0; i < startindex; i++)
                {
                    if (!e.MoveNext())
                        throw new Exception("Sequence ran out of elements");
                }

                for (var i = 0; i < length; i++)
                {
                    if (!e.MoveNext())
                        throw new Exception("Sequence ran out of elements");

                    yield return e.Current;
                }
            }
        }

        /// <summary>
        /// Interleaves two sequences to a single sequence by returning elements from both sequences in an alternating pattern.
        /// </summary>
        /// <param name="first">The first sequence.</param>
        /// <param name="second">The second sequence.</param>
        public static IEnumerable<T> Interleave<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            using var enumerator1 = first.GetEnumerator();
            using var enumerator2 = second.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                if (!enumerator2.MoveNext())
                    throw new LengthMismatchException();

                yield return enumerator1.Current;
                yield return enumerator2.Current;
            }
        }

        /// <summary>
        /// Interleaves two sequences to a single sequence by returning elements from both sequences in an alternating pattern.
        /// </summary>
        /// <param name="first">The first sequence.</param>
        /// <param name="second">The second sequence.</param>
        /// <remarks>This is evaluated lazily.</remarks>
        public static IReadOnlyCollection<T> Interleave<T>(this IReadOnlyCollection<T> first, IReadOnlyCollection<T> second)
        {
            if (first.Count != second.Count)
                throw new LengthMismatchException();

            return Iterator().WithCount(first.Count * 2);

            IEnumerable<T> Iterator()
            {
                using var enumerator1 = first.GetEnumerator();
                using var enumerator2 = second.GetEnumerator();
                for (var i = 0; i < first.Count; i++)
                {
                    enumerator1.MoveNext();
                    yield return enumerator1.Current;
                    enumerator2.MoveNext();
                    yield return enumerator2.Current;
                }
            }
        }

        /// <summary>
        /// Loops a sequence for the specified number of cycles.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="loopCount">The number of cycles. If this is <0, the sequence is looped indefinitely.</param>
        public static ILazyReadOnlyList<T> Loop<T>(this IReadOnlyList<T> sequence, int loopCount)
        {
            return new LoopIterator<T>(sequence, loopCount);
        }

        /// <summary>
        /// Pads a sequence at the end with the specified number of pad elements.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="count">The number of pad elements.</param>
        /// <param name="padElement">The pad element.</param>
        public static IEnumerable<T> PadEnd<T>(this IEnumerable<T> input, int count, T padElement = default)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return input.Concat(Enumerable.Repeat(padElement, count));
        }

        /// <summary>
        /// Pads a sequence at the start with the specified number of pad elements.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="count">The number of pad elements.</param>
        /// <param name="padElement">The pad element.</param>
        /// <remarks>This is evaluated lazily.</remarks>
        public static IReadOnlyCollection<T> PadEnd<T>(this IReadOnlyCollection<T> input, int count, T padElement = default)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return input
                .Concat(Enumerable.Repeat(padElement, count))
                .WithCount(input.Count + count);
        }

        /// <summary>
        /// Pads a sequence at the start with the specified number of pad elements.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="count">The number of pad elements.</param>
        /// <param name="padElement">The pad element.</param>
        public static IEnumerable<T> PadStart<T>(this IEnumerable<T> input, int count, T padElement = default)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return Enumerable.Repeat(padElement, count).Concat(input);
        }

        /// <summary>
        /// Pads a sequence at the start with the specified number of pad elements.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="count">The number of pad elements.</param>
        /// <param name="padElement">The pad element.</param>
        /// <remarks>This is evaluated lazily.</remarks>
        public static IReadOnlyCollection<T> PadStart<T>(this IReadOnlyCollection<T> input, int count, T padElement = default)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return Enumerable
                .Repeat(padElement, count)
                .Concat(input)
                .WithCount(input.Count + count);
        }

        /// <summary>
        /// Pads a sequence at the end with the specified number of pad elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input sequence.</param>
        /// <param name="totalLength">The total length of the resulting sequence.</param>
        /// <param name="padElement">The pad element.</param>
        /// <remarks>This is evaluated lazily.</remarks>
        public static IReadOnlyCollection<T> PadToLength<T>(this IEnumerable<T> input, int totalLength, T padElement = default)
        {
            if (totalLength < 0)
                throw new ArgumentOutOfRangeException(nameof(totalLength));

            return PadToLengthIterator().WithCount(totalLength);

            IEnumerable<T> PadToLengthIterator()
            {
                using var e = input.GetEnumerator();
                var c = 0;
                while (e.MoveNext())
                {
                    yield return e.Current;
                    c++;
                    if (c == totalLength)
                        yield break;
                }

                while (c < totalLength)
                {
                    yield return padElement;
                }
            }
        }

        /// <summary>
        /// Computes a sub-portion of a sequence by only including every xth element.
        /// </summary>
        /// <param name="series">The sequence.</param>
        /// <param name="sparseFactor">The sparse factor.</param>
        public static IEnumerable<T> SparseSequence<T>(this IEnumerable<T> series, int sparseFactor)
        {
            if (series == null)
                throw new ArgumentNullException(nameof(series));

            return Iterator();

            IEnumerable<T> Iterator()
            {
                using var e = series.GetEnumerator();
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
        /// Computes a sub-portion of a sequence by only including every xth element.
        /// </summary>
        /// <param name="series">The sequence.</param>
        /// <param name="sparseFactor">The sparse factor.</param>
        /// <remarks>This is evaluated lazily.</remarks>
        public static IReadOnlyCollection<T> SparseSequence<T>(this IReadOnlyCollection<T> series, int sparseFactor)
        {
            return ((IEnumerable<T>)series).SparseSequence(sparseFactor).WithCount(series.Count / sparseFactor);
        }

        private class LoopIterator<T> : ILazyReadOnlyList<T>
        {
            private readonly int loopCount;
            private readonly IReadOnlyList<T> sequence;

            public LoopIterator(IReadOnlyList<T> sequence, int loopCount)
            {
                this.sequence = sequence;
                this.loopCount = loopCount;
            }

            public int Count => this.sequence.Count * this.loopCount;
            public T this[int index] => this.sequence[index % this.sequence.Count];

            public IEnumerator<T> GetEnumerator()
            {
                if (this.loopCount < 0)
                {
                    while (true)
                    {
                        for (var j = 0; j < this.sequence.Count; j++)
                        {
                            yield return this.sequence[j];
                        }
                    }
                }

                for (var i = 0; i < this.loopCount; i++)
                {
                    for (var j = 0; j < this.sequence.Count; j++)
                    {
                        yield return this.sequence[j];
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}