using DspSharp.Exceptions;
using DspSharp.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Extensions
{
    public static class IEnumerableExtensions
    {
        public static T[] CastOrToArray<T>(this IEnumerable<T> sequence)
        {
            if (sequence is T[] array)
                return array;

            if (sequence is IReadOnlyCollection<T> irc)
            {
                var ret = new T[irc.Count];
                var c = 0;
                foreach (var item in irc)
                    ret[c++] = item;
            }

            return sequence.ToArray();
        }

        public static IReadOnlyList<T> CastOrToList<T>(this IEnumerable<T> sequence)
        {
            if (sequence is IReadOnlyList<T> irot)
                return irot;

            if (sequence is IReadOnlyCollection<T> irc)
            {
                var ret = new T[irc.Count];
                var c = 0;
                foreach (var item in irc)
                    ret[c++] = item;
            }

            return sequence.ToList();
        }

        public static ILazyReadOnlyList<T> Range<T>(this IReadOnlyList<T> source, int skip, int take)
        {
            if (skip < 0)
                throw new ArgumentOutOfRangeException(nameof(skip));
            if (take < 0)
                throw new ArgumentOutOfRangeException(nameof(take));

            return new RangeIndexer<T>(source, skip, take);
        }

        public static ILazyReadOnlyList<T> ReverseIndexed<T>(this IReadOnlyList<T> source)
        {
            return new ReverseIndexer<T>(source);
        }

        public static ILazyReadOnlyList<TResult> SelectIndexed<T, TResult>(this IReadOnlyList<T> source, Func<T, TResult> selector)
        {
            return new SelectIndexer<T, TResult>(source, selector);
        }

        public static ILazyReadOnlyCollection<TResult> SelectWithCount<T, TResult>(this IReadOnlyCollection<T> source, Func<T, TResult> selector)
        {
            return source.Select(selector).WithCount(source.Count);
        }

        public static ILazyReadOnlyCollection<TResult> SelectWithCount<T, TResult>(this IReadOnlyCollection<T> source, Func<T, int, TResult> selector)
        {
            return source.Select(selector).WithCount(source.Count);
        }

        public static T[] ToArray<T>(this IReadOnlyCollection<T> collection)
        {
            var ret = new T[collection.Count];
            var i = 0;
            foreach (var item in collection)
                ret[i++] = item;

            return ret;
        }

        public static List<T> ToList<T>(this IReadOnlyCollection<T> collection)
        {
            var ret = new List<T>(collection.Count);
            ret.AddRange(collection);
            return ret;
        }

        public static ILazyReadOnlyCollection<T> WithCount<T>(this IEnumerable<T> source, int length)
        {
            return new EnumerableWithLength<T>(source, length);
        }

        public static IEnumerable<TResult> ZipExact<T, TResult>(this IEnumerable<T> source, IEnumerable<T> other, Func<T, T, TResult> selector)
        {
            using var e1 = source.GetEnumerator();
            using var e2 = other.GetEnumerator();
            while (e1.MoveNext())
            {
                if (!e2.MoveNext())
                    throw new LengthMismatchException();

                yield return selector(e1.Current, e2.Current);
            }

            if (e2.MoveNext())
                throw new LengthMismatchException();
        }

        public static ILazyReadOnlyList<TResult> ZipIndexed<T, TResult>(this IReadOnlyList<T> source, IReadOnlyList<T> other, Func<T, T, TResult> selector)
        {
            return new ZipIndexer<T, TResult>(source, other, selector);
        }

        public static ILazyReadOnlyCollection<TResult> ZipWithCount<T, TResult>(this IReadOnlyCollection<T> source, IReadOnlyCollection<T> other, Func<T, T, TResult> selector)
        {
            if (source.Count != other.Count)
                throw new LengthMismatchException();

            return source.Zip(other, selector).WithCount(source.Count);
        }

        private class EnumerableWithLength<T> : ILazyReadOnlyCollection<T>
        {
            private readonly IEnumerable<T> source;

            public EnumerableWithLength(IEnumerable<T> source, int length)
            {
                this.source = source;
                this.Count = length;
            }

            public int Count { get; }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return this.source.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.source.GetEnumerator();
            }
        }

        private class RangeIndexer<T> : ILazyReadOnlyList<T>
        {
            private readonly int skip;
            private readonly IReadOnlyList<T> source;
            private readonly int take;

            public RangeIndexer(IReadOnlyList<T> source, int skip, int take)
            {
                this.source = source;
                this.skip = skip;
                this.take = take;
            }

            public int Count => this.take;
            public T this[int index] => this.source[index + this.skip];

            public IEnumerator<T> GetEnumerator()
            {
                var max = this.skip + this.take;
                for (var i = this.skip; i < max; i++)
                {
                    yield return this.source[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        private class ReverseIndexer<T> : ILazyReadOnlyList<T>
        {
            public ReverseIndexer(IReadOnlyList<T> source)
            {
                this.Source = source;
            }

            public int Count => this.Source.Count;
            public IReadOnlyList<T> Source { get; }
            public T this[int index] => this.Source[^(index + 1)];

            public IEnumerator<T> GetEnumerator()
            {
                for (var i = this.Source.Count - 1; i >= 0; i--)
                    yield return this.Source[i];
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        private class SelectIndexer<T, TResult> : ILazyReadOnlyList<TResult>
        {
            public SelectIndexer(IReadOnlyList<T> source, Func<T, TResult> func)
            {
                this.Source = source;
                this.Func = func;
            }

            public int Count => this.Source.Count;
            public Func<T, TResult> Func { get; }
            public IReadOnlyList<T> Source { get; }
            public TResult this[int index] => this.Func(this.Source[index]);

            public IEnumerator<TResult> GetEnumerator()
            {
                return this.Source.Select(this.Func).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        private class ZipIndexer<T, TResult> : ILazyReadOnlyList<TResult>
        {
            public ZipIndexer(IReadOnlyList<T> source, IReadOnlyList<T> other, Func<T, T, TResult> func)
            {
                if (source.Count != other.Count)
                    throw new LengthMismatchException();

                this.Source = source;
                this.Other = other;
                this.Func = func;
            }

            public int Count => this.Source.Count;
            public Func<T, T, TResult> Func { get; }
            public IReadOnlyList<T> Other { get; }
            public IReadOnlyList<T> Source { get; }
            public TResult this[int index] => this.Func(this.Source[index], this.Other[index]);

            public IEnumerator<TResult> GetEnumerator()
            {
                for (var i = 0; i < this.Source.Count; i++)
                {
                    yield return this.Func(this.Source[i], this.Other[i]);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}