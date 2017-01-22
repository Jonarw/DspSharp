using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Filter.Algorithms;

namespace Filter.Extensions
{
    /// <summary>
    ///     Provides static extensions for vector types implementing basic vector operations.
    /// </summary>
    public static class VectorOperations
    {
        /// <summary>
        ///     Adds two real-valued vectors element-wise. The longer vector is truncated to the length of the shorter vector.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="input2">The second vector.</param>
        /// <returns></returns>
        public static IEnumerable<double> Add(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            return input.Zip(input2, (d, d1) => d + d1);
        }

        /// <summary>
        ///     Adds two complex-valued vectors element-wise. The longer vector is truncated to the length of the shorter vector.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="input2">The second vector.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Add(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            return input.Zip(input2, (d, d1) => d + d1);
        }

        /// <summary>
        ///     Adds a scalar to a complex-valued vector.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Add(this IEnumerable<Complex> input, Complex scalar)
        {
            return input.Select(c => c + scalar);
        }

        /// <summary>
        ///     Adds a scalar to a real-valued vector.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<double> Add(this IEnumerable<double> input, double scalar)
        {
            return input.Select(c => c + scalar);
        }

        /// <summary>
        ///     Adds two real-valued vectors element-wise. The shorter vector is zero-padded to the length of the longer vector.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="input2">The second vector.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Adds two real-valued vectors element-wise with an offset. The offset is achieved through zero-padding at the
        ///     beginning of one of the vectors. The shorter vector (including the offset) is zero-padded to the length of the
        ///     longer vector.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="input2">The second vector.</param>
        /// <param name="offset">The offset. Positive values delay the second vector relative to the first vector.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Calculates the energy of a vector by summing up its squared elements.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static double CalculateEnergy(this IEnumerable<double> input)
        {
            return input.Aggregate((d, d1) => d + d1 * d1);
        }

        /// <summary>
        ///     Applies a circular shift to the provided vector.
        /// </summary>
        /// <param name="series">The vector.</param>
        /// <param name="amount">The shift amount.</param>
        /// <returns></returns>
        public static IEnumerable<double> CircularShift(this IEnumerable<double> series, int amount)
        {
            var list = series.ToReadOnlyList();
            amount = Dsp.Mod(amount, list.Count);
            return list.Skip(amount).Concat(list.Take(amount));
        }

        /// <summary>
        ///     Convolves two vectors.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="input2">The second vector.</param>
        /// <returns></returns>
        public static IEnumerable<double> Convolve(this IEnumerable<double> input, IReadOnlyList<double> input2)
        {
            return Dsp.Convolve(input, input2);
        }

        /// <summary>
        ///     Divides two real-valued vectors element-wise. The longer vector is truncated to the length of the shorter vector.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="input2">The second vector.</param>
        /// <returns></returns>
        public static IEnumerable<double> Divide(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            return input.Zip(input2, (d, d1) => d / d1);
        }

        /// <summary>
        ///     Divides two complex-valued vectors element-wise. The longer vector is truncated to the length of the shorter
        ///     vector.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="input2">The second vector.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Divide(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            return input.Zip(input2, (d, d1) => d / d1);
        }

        /// <summary>
        ///     Divides a scalar by a complex-valued vector.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Divide(this Complex scalar, IEnumerable<Complex> input)
        {
            return input.Select(c => scalar / c);
        }

        /// <summary>
        ///     Divides a scalar by a real-valued vector.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<double> Divide(this double scalar, IEnumerable<double> input)
        {
            return input.Select(c => scalar / c);
        }

        /// <summary>
        ///     Deterimines wheather two vectors have the same values.
        /// </summary>
        /// <param name="e1">The first vector.</param>
        /// <param name="e2">The second vector.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Finds the index of a vector that includes the specified portion of the vector's energy.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <param name="threshold">The energy threshold.</param>
        /// <param name="initialLength">The initial index.</param>
        /// <param name="maxLength">The maximum index.</param>
        /// <returns></returns>
        public static int FindEnergyThreshold(
            this IEnumerable<double> input,
            double threshold = 0.00001,
            int initialLength = 1024,
            int maxLength = 524288)
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

        /// <summary>
        ///     Gets a range from a vector, wrapping around to zero if reaching the end of the vector.
        /// </summary>
        /// <param name="fftResult">The vector.</param>
        /// <param name="start">The start of the range.</param>
        /// <param name="length">The length of the range.</param>
        /// <returns></returns>
        public static IEnumerable<double> GetCircularRange(this IEnumerable<double> fftResult, int start, int length)
        {
            var list = fftResult.ToReadOnlyList();

            start = Dsp.Mod(start, list.Count);

            int stop;
            if (length > list.Count)
            {
                stop = start == 0 ? list.Count - 1 : start - 1;
                return list.GetRangeOptimized(start, list.Count - start).Concat(list.Take(stop)).ZeroPad(length - list.Count);
            }

            stop = Dsp.Mod(start + length, list.Count);

            if (start < stop)
            {
                return list.GetRangeOptimized(start, stop);
            }

            return list.GetRangeOptimized(start, list.Count - start).Concat(list.Take(stop));
        }

        /// <summary>
        ///     Gets a range from a vector, zero-padding at the start and end if necessary.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <param name="start">The start of the range.</param>
        /// <param name="length">The length of the range.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Gets a range range from a vector, taking advantage of indexed access if the vector type supports it.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="startindex">The start of the range.</param>
        /// <param name="length">The length of the range.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Extracts the imaginary part of a complex vector.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <returns></returns>
        public static IEnumerable<double> Imaginary(this IEnumerable<Complex> input)
        {
            return input.Select(c => c.Imaginary);
        }

        /// <summary>
        ///     Converts a complex-valued vector to a real-valued vector containing the real and imaginary parts in an alternating
        ///     pattern.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public static IEnumerable<double> InterleaveComplex(this IEnumerable<Complex> c)
        {
            foreach (var complex in c)
            {
                yield return complex.Real;
                yield return complex.Imaginary;
            }
        }

        /// <summary>
        ///     Interleaves two vectors to a single vector containing both vectors in an alternating pattern.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <returns></returns>
        public static IEnumerable<T> InterleaveEnumerations<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            IEnumerator<T> enumerator1 = first.GetEnumerator();
            IEnumerator<T> enumerator2 = second.GetEnumerator();
            while (enumerator1.MoveNext() && enumerator2.MoveNext())
            {
                yield return enumerator1.Current;
                yield return enumerator2.Current;
            }
        }

        /// <summary>
        ///     Zeropads a vector at the start.
        /// </summary>
        /// <param name="d">The vector.</param>
        /// <param name="n">The number of leading zeros.</param>
        /// <returns></returns>
        public static IEnumerable<double> LeadingZeros(this IEnumerable<double> d, int n)
        {
            return Enumerable.Repeat(0.0, n).Concat(d);
        }

        /// <summary>
        ///     Computes the logarithm of a vector element-wise.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="newBase">The base.</param>
        /// <returns></returns>
        public static IEnumerable<double> Log(this IEnumerable<double> input, double newBase = 10)
        {
            return input.Select(d => Math.Log(d, newBase));
        }

        /// <summary>
        ///     Extracts the magnitude from a complex-valued vector.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <returns></returns>
        public static IEnumerable<double> Magitude(this IEnumerable<Complex> input)
        {
            return input.Select(c => c.Magnitude);
        }

        /// <summary>
        ///     Finds the index with the maximum value in a vector.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The vector.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Finds the index with the minimum value in a vector.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The vector.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Multiplies two real-valued vectors element-wise. The longer vector is truncated to the length of the shorter
        ///     vector.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="input2">The second vector.</param>
        /// <returns></returns>
        public static IEnumerable<double> Multiply(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            return input.Zip(input2, (d, d1) => d * d1);
        }

        /// <summary>
        ///     Multiplies two complex-valued vectors element-wise. The longer vector is truncated to the length of the shorter
        ///     vector.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="input2">The second vector.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Multiply(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            return input.Zip(input2, (d, d1) => d * d1);
        }

        /// <summary>
        ///     Multiplies a complex-valued vector with a scalar.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Multiply(this IEnumerable<Complex> input, Complex scalar)
        {
            return input.Select(c => c * scalar);
        }

        /// <summary>
        ///     Multiplies a real-valued vector with a scalar.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<double> Multiply(this IEnumerable<double> input, double scalar)
        {
            return input.Select(c => c * scalar);
        }

        /// <summary>
        ///     Negates a complex-valued vector.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Negate(this IEnumerable<Complex> input)
        {
            return input.Select(c => -c);
        }

        /// <summary>
        ///     Negates a real-valued vector.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <returns></returns>
        public static IEnumerable<double> Negate(this IEnumerable<double> input)
        {
            return input.Select(c => -c);
        }

        /// <summary>
        ///     Extracts the phase from a complex-valued vector.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <returns></returns>
        public static IEnumerable<double> Phase(this IEnumerable<Complex> input)
        {
            return input.Select(c => c.Phase);
        }

        /// <summary>
        ///     Raises the elements of a vector to the specified power.
        /// </summary>
        /// <param name="power">The power.</param>
        /// <param name="input">The vector.</param>
        /// <returns></returns>
        public static IEnumerable<double> Pow(this IEnumerable<double> input, double power)
        {
            return input.Select(d => Math.Pow(power, d));
        }

        /// <summary>
        ///     Extracts the real part from a complex-valued vector.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <returns></returns>
        public static IEnumerable<double> Real(this IEnumerable<Complex> input)
        {
            return input.Select(c => c.Real);
        }

        /// <summary>
        ///     Computes a sub-portion of a vector by only including every xth element.
        /// </summary>
        /// <param name="series">The vector.</param>
        /// <param name="sparseFactor">The sparse factor.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Calculates the standard deviation of a sequence.
        /// </summary>
        /// <param name="values">The sequence.</param>
        /// <returns>The standard deviation of the sequence.</returns>
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var valueslist = values.ToReadOnlyList();
            return StandardDeviation(valueslist, valueslist.Average());
        }

        /// <summary>
        ///     Calculates the standard deviation of a sequence.
        /// </summary>
        /// <param name="values">The sequence.</param>
        /// <param name="mean">The mean of the sequence.</param>
        /// <returns>The standard deviation of the sequence.</returns>
        public static double StandardDeviation(this IEnumerable<double> values, double mean)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var valueslist = values.ToReadOnlyList();
            return Math.Sqrt(valueslist.Variance(mean));
        }

        /// <summary>
        ///     Subtracts two real-valued vectors element-wise. The longer vector is truncated to the length of the shorter vector.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="input2">The second vector.</param>
        /// <returns></returns>
        public static IEnumerable<double> Subtract(this IEnumerable<double> input, IEnumerable<double> input2)
        {
            return input.Zip(input2, (d, d1) => d - d1);
        }

        /// <summary>
        ///     Subtracts two complex-valued vectors element-wise. The longer vector is truncated to the length of the shorter
        ///     vector.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="input2">The second vector.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Subtract(this IEnumerable<Complex> input, IEnumerable<Complex> input2)
        {
            return input.Zip(input2, (d, d1) => d - d1);
        }

        /// <summary>
        ///     Subtracts a complex-valued vector from a scalar.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> Subtract(this Complex scalar, IEnumerable<Complex> input)
        {
            return input.Select(c => scalar - c);
        }

        /// <summary>
        ///     Subtracts a real-valued vector from a scalar.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static IEnumerable<double> Subtract(this double scalar, IEnumerable<double> input)
        {
            return input.Select(c => scalar - c);
        }

        /// <summary>
        ///     Subtracts two real-valued vectors element-wise. The shorter vector is zero-padded to the length of the longer
        ///     vector.
        /// </summary>
        /// <param name="input">The first vector.</param>
        /// <param name="input2">The second vector.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Takes the specified amount of items from a vector. If the end of the vector is reached, it is zero-padded.
        /// </summary>
        /// <param name="input">The vector.</param>
        /// <param name="length">The amount of items to take.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Creates a new complex vector from a vector of real parts.
        /// </summary>
        /// <param name="input">The real-valued input.</param>
        /// <returns></returns>
        public static IEnumerable<Complex> ToComplex(this IEnumerable<double> input)
        {
            return input.Select(d => new Complex(d, 0));
        }

        /// <summary>
        ///     Creates an enumerable containing a single element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumerable<T>(this T element)
        {
            return Enumerable.Repeat(element, 1);
        }

        /// <summary>
        ///     Returns a readonly list containing the specified sequence, evaluating it if necessary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <returns></returns>
        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> sequence)
        {
            var ilist = sequence as IList<T>;
            if (ilist != null)
            {
                return new ReadOnlyCollection<T>(ilist);
            }

            return sequence as IReadOnlyList<T> ?? new ReadOnlyCollection<T>(sequence.ToList());
        }

        /// <summary>
        ///     Returns a readonly list containing the specified sequence, evaluating it if necessary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <param name="maximumLength">The maximum evaluation length.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Creates a complex-valued vector from an interleaved real-valued vector containing real and imaginary parts in an
        ///     alternating pattern.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Calculates the variance of a sequence.
        /// </summary>
        /// <param name="values">The sequence.</param>
        /// <returns>The variance of the sequence.</returns>
        public static double Variance(this IEnumerable<double> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var valueslist = values.ToReadOnlyList();
            return Variance(valueslist, valueslist.Average());
        }

        /// <summary>
        ///     Calculates the variance of a sequence.
        /// </summary>
        /// <param name="values">The sequence.</param>
        /// <param name="mean">The mean of the sequence.</param>
        /// <returns>The variance of the sequence.</returns>
        public static double Variance(this IEnumerable<double> values, double mean)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var valueslist = values.ToReadOnlyList();

            if (valueslist.Count == 0)
            {
                return 0;
            }

            double variance = valueslist.Aggregate(0.0, (d, d1) => d + Math.Pow(d1 - mean, 2));
            return variance / valueslist.Count;
        }

        /// <summary>
        ///     Zeropads a vector.
        /// </summary>
        /// <param name="d">The vector.</param>
        /// <param name="n">The amount of zeros.</param>
        /// <returns></returns>
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