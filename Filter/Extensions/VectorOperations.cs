using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Filter.Algorithms;
using Filter.Series;

namespace Filter.Extensions
{
    public static class VectorOperations
    {
        public static IEnumerable<Complex> ToComplex(this IEnumerable<double> input)
        {
            return input.Select(d => new Complex(d, 0));
        } 

        public static IEnumerable<double> Add(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            return input.Zip(input2, (d, d1) => d + d1);
        }

        public static IEnumerable<Complex> Add(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            return input.Zip(input2, (d, d1) => d + d1);
        }

        public static IEnumerable<Complex> Add(this IEnumerable<Complex> input, Complex scalar)
        {
            return input.Select(c => c + scalar);
        }

        public static IEnumerable<double> Add(this IEnumerable<double> input, double scalar)
        {
            return input.Select(c => c + scalar);
        }

        public static IEnumerable<double> AddFull(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            IEnumerator<double> enumerator = input.GetEnumerator();
            IEnumerator<double> enumerator2 = input2.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator2.MoveNext())
                {
                    yield return enumerator.Current + enumerator2.Current;
                }
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

        public static IEnumerable<double> AddFullWithOffset(this IEnumerable<double> input, IEnumerable<double> input2, int offset)
        {
            IEnumerator<double> enumerator = input.GetEnumerator();
            IEnumerator<double> enumerator2 = input2.GetEnumerator();
            int c = offset;

            while (enumerator2.MoveNext() && (c < 0))
            {
                yield return enumerator2.Current;
            }

            c = 0;

            while (enumerator.MoveNext())
            {
                if (c++ < offset)
                {
                    yield return enumerator.Current;
                }
                else if (enumerator2.MoveNext())
                {
                    yield return enumerator.Current + enumerator2.Current;
                }
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

            while (c++ < offset)
            {
                yield return 0.0;
            }

            while (enumerator2.MoveNext())
            {
                yield return enumerator2.Current;
            }
        }

        public static IEnumerable<double> CircularShift(this IEnumerable<double> series, int amount)
        {
            var list = series.ToReadOnlyList();
            amount = Dsp.Mod(amount, list.Count);
            return list.Skip(amount).Concat(list.Take(amount));
        }

        public static IEnumerable<double> Convolve(this IEnumerable<double> input, IReadOnlyList<double> input2)
        {
            return Dsp.Convolve(input, input2);
        }

        public static IEnumerable<double> Divide(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            return input.Zip(input2, (d, d1) => d / d1);
        }

        public static IEnumerable<Complex> Divide(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            return input.Zip(input2, (d, d1) => d / d1);
        }

        public static IEnumerable<Complex> Divide(this Complex scalar, IEnumerable<Complex> input)
        {
            return input.Select(c => scalar / c);
        }

        public static IEnumerable<double> Divide(this double scalar, IEnumerable<double> input)
        {
            return input.Select(c => scalar / c);
        }

        public static bool Equals(this IEnumerable<double> e1, IEnumerable<double> e2)
        {
            var s1 = e1 as IReadOnlyCollection<double>;
            if (s1 != null)
            {
                var s2 = e2 as IReadOnlyCollection<double>;
                if (s2 != null)
                {
                    if (s1.Count != s2.Count)
                    {
                        return false;
                    }
                }
            }

            return e1.SequenceEqual(e2);
        }

        public static IEnumerable<Complex> EvaluateFrequencies(this Func<double, Complex> input, ISeries frequencies)
        {
            return frequencies.Values.Select(input.Invoke);
        }

        public static IEnumerable<double> EvaluateSamples(this Func<int, double> input, int start, int length)
        {
            return Enumerable.Range(start, length).Select(input.Invoke);
        }

        public static IEnumerable<double> GetCircularRange(this IEnumerable<double> fftResult, int start, int length)
        {
            var list = fftResult.ToReadOnlyList();

            start = Dsp.Mod(start, list.Count);

            int stop;
            if (length > list.Count)
            {
                stop = start == 0 ? list.Count - 1 : start - 1;
                return list.GetRangeOptimized(start, list.Count - start).Concat(list.Take(stop)).Concat(Dsp.Zeros(length - list.Count));
            }

            stop = Dsp.Mod(start + length, list.Count);

            if (start < stop)
            {
                return list.GetRangeOptimized(start, stop);
            }

            return list.GetRangeOptimized(start, list.Count - start).Concat(list.Take(stop));
        }

        public static IEnumerable<double> GetPaddedRange(this IEnumerable<double> input, int start, int length)
        {
            int c = start;
            int i = 0;
            while ((c < 0) && (i < length))
            {
                yield return 0.0;
                i++;
                c++;
            }

            c = 0;
            var e = input.GetEnumerator();
            while ((c < start) && e.MoveNext())
            {
                c++;
            }

            while (c < start)
            {
                c++;
            }

            while (e.MoveNext() && (i < length))
            {
                yield return e.Current;
                i++;
                c++;
            }

            while (i < length)
            {
                yield return 0.0;
                i++;
            }
        }

        public static IEnumerable<double> GetRangeOptimized(this IEnumerable<double> input, int startindex, int length)
        {
            var inputlist = input as IReadOnlyList<double>;

            if (inputlist == null)
            {
                var e = input.Skip(startindex).GetEnumerator();
                int c = 0;

                while (e.MoveNext() && (c++ < length))
                {
                    yield return e.Current;
                }

                yield break;
            }

            int stop = Math.Min(startindex + length, inputlist.Count);

            for (int i = startindex; i < stop; i++)
            {
                yield return inputlist[i];
            }
        }

        public static IEnumerable<double> Imaginary(this IEnumerable<Complex> input)
        {
            return input.Select(c => c.Imaginary);
        }

        public static IEnumerable<double> InterleaveComplex(this IEnumerable<Complex> c)
        {
            foreach (var complex in c)
            {
                yield return complex.Real;
                yield return complex.Imaginary;
            }
        }

        public static IEnumerable<T> InterleaveEnumerationsOfEqualLength<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            IEnumerator<T> enumerator1 = first.GetEnumerator();
            IEnumerator<T> enumerator2 = second.GetEnumerator();
            while (enumerator1.MoveNext() && enumerator2.MoveNext())
            {
                yield return enumerator1.Current;
                yield return enumerator2.Current;
            }
        }

        public static IEnumerable<double> LeadingZeros(this IEnumerable<double> d, int n)
        {
            return Enumerable.Repeat(0.0, n).Concat(d);
        }

        /// <summary>
        ///     Logs the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="newBase"></param>
        /// <returns></returns>
        public static IEnumerable<double> Log(this IEnumerable<double> input, double newBase = 10)
        {
            return input.Select(d => Math.Log(d, newBase));
        }

        public static IEnumerable<double> Magitude(this IEnumerable<Complex> input)
        {
            return input.Select(c => c.Magnitude);
        }

        public static int MaxIndex<T>(this IEnumerable<T> sequence)
            where T : IComparable<T>
        {
            int maxIndex = -1;
            T maxValue = default(T); // Immediately overwritten anyway

            int index = 0;
            foreach (T value in sequence)
            {
                if ((value.CompareTo(maxValue) > 0) || (maxIndex == -1))
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }

        public static int MinIndex<T>(this IEnumerable<T> sequence)
            where T : IComparable<T>
        {
            int minIndex = -1;
            T minValue = default(T); // Immediately overwritten anyway

            int index = 0;
            foreach (T value in sequence)
            {
                if ((value.CompareTo(minValue) < 0) || (minIndex == -1))
                {
                    minIndex = index;
                    minValue = value;
                }
                index++;
            }
            return minIndex;
        }

        public static IEnumerable<double> Multiply(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            return input.Zip(input2, (d, d1) => d * d1);
        }

        public static IEnumerable<Complex> Multiply(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            return input.Zip(input2, (d, d1) => d * d1);
        }

        public static IEnumerable<Complex> Multiply(this IEnumerable<Complex> input, Complex scalar)
        {
            return input.Select(c => c * scalar);
        }

        public static double CalculateEnergy(this IEnumerable<double> input)
        {
            return input.Aggregate((d, d1) => d + d1 * d1);
        }

        public static int FindEnergyThreshold(this IEnumerable<double> input, double threshold = 0.00001, int initialLength = 1024, int maxLength = 524288)
        {
            var e = input.GetEnumerator();
            int c = 0;

            double currentEnergy = 0.0;
            double previousEnergy = 0.0;

            while (e.MoveNext() && (c < initialLength))
            {
                previousEnergy += e.Current * e.Current;
                c++;
            }

            var currentLength = initialLength * 2;

            while (e.MoveNext())
            {
                currentEnergy += e.Current * e.Current;
                c++;

                if (c == currentLength)
                {
                    if (currentEnergy / previousEnergy < threshold)
                    {
                        break;
                    }

                    currentLength *= 2;
                    if (currentLength >= maxLength)
                    {
                        break;
                    }

                    previousEnergy += currentEnergy;
                    currentEnergy = 0;
                }
            }

            return c;
        }

        public static IEnumerable<double> Multiply(this IEnumerable<double> input, double scalar)
        {
            return input.Select(c => c * scalar);
        }

        public static IEnumerable<Complex> Negate(this IEnumerable<Complex> input)
        {
            return input.Select(c => -c);
        }

        public static IEnumerable<double> Negate(this IEnumerable<double> input)
        {
            return input.Select(c => -c);
        }

        public static IEnumerable<double> Phase(this IEnumerable<Complex> input)
        {
            return input.Select(c => c.Phase);
        }

        /// <summary>
        ///     Logs the specified input.
        /// </summary>
        /// <param name="power"></param>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static IEnumerable<double> Pow(this IEnumerable<double> input, double power)
        {
            return input.Select(d => Math.Pow(power, d));
        }

        public static IEnumerable<double> Real(this IEnumerable<Complex> input)
        {
            return input.Select(c => c.Real);
        }

        public static IEnumerable<Complex> SparseSeries(this IEnumerable<Complex> series, int sparseFactor)
        {
            var e = series.GetEnumerator();
            while (true)
            {
                for (int i = 0; i < sparseFactor; i++)
                {
                    if (!e.MoveNext())
                    {
                        yield break;
                    }
                }

                yield return e.Current;
            }
        }

        public static IEnumerable<double> Subtract(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            return input.Zip(input2, (d, d1) => d - d1);
        }

        public static IEnumerable<Complex> Subtract(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            return input.Zip(input2, (d, d1) => d - d1);
        }

        public static IEnumerable<Complex> Subtract(this Complex scalar, IEnumerable<Complex> input)
        {
            return input.Select(c => scalar - c);
        }

        public static IEnumerable<double> Subtract(this double scalar, IEnumerable<double> input)
        {
            return input.Select(c => scalar - c);
        }

        public static IEnumerable<double> SubtractFull(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            IEnumerator<double> enumerator = input.GetEnumerator();
            IEnumerator<double> enumerator2 = input2.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator2.MoveNext())
                {
                    yield return enumerator.Current - enumerator2.Current;
                }
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

        public static IEnumerable<double> TakeFull(this IEnumerable<double> input, int length)
        {
            var e = input.GetEnumerator();
            int c = 0;
            while (e.MoveNext() && (c < length))
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

        public static IEnumerable<T> ToEnumerable<T>(this T element)
        {
            return Enumerable.Repeat(element, 1);
        }

        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> sequence)
        {
            var ilist = sequence as IList<T>;
            if (ilist != null)
            {
                return new ReadOnlyCollection<T>(ilist);
            }

            return sequence as IReadOnlyList<T> ?? new ReadOnlyCollection<T>(sequence.ToList());
        }

        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> sequence, int maximumLength)
        {
            var ilist = sequence as IList<T>;
            if (ilist != null)
            {
                if (ilist.Count <= maximumLength)
                {
                    return new ReadOnlyCollection<T>(ilist);
                }

                return ilist.Take(maximumLength).ToReadOnlyList();
            }

            var irolist = sequence as IReadOnlyList<T>;
            if (irolist != null)
            {
                if (irolist.Count <= maximumLength)
                {
                    return irolist;
                }

                return irolist.Take(maximumLength).ToReadOnlyList();
            }
            
            var e = sequence.GetEnumerator();
            int i = 0;
            var ret = new List<T>();
            while (e.MoveNext() && (i < maximumLength))
            {
                ret.Add(e.Current);
                i++;
            }

            return ret.AsReadOnly();
        }

        public static IEnumerable<Complex> UnInterleaveComplex(this IEnumerable<double> d)
        {
            IEnumerator<double> enumerator = d.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var real = enumerator.Current;
                enumerator.MoveNext();
                yield return new Complex(real, enumerator.Current);
            }
        }

        public static IEnumerable<double> ZeroPad(this IEnumerable<double> d, int n)
        {
            IEnumerator<double> enumerator = d.GetEnumerator();
            int i = 0;

            while (enumerator.MoveNext() && (i++ < n))
            {
                yield return enumerator.Current;
            }

            while (i++ < n)
            {
                yield return 0;
            }
        }
    }
}