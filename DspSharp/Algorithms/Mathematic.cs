// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mathematic.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DspSharp.Algorithms
{
    public static class Mathematic
    {
        /// <summary>
        ///     Uses a simple iterative algorithm to find the root of a (locally) monotonous function.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="startValue">The start value.</param>
        /// <param name="initialStepSize">The initial step size.</param>
        /// <param name="threshold">The threshold where the iteration stops.</param>
        /// <returns>The x coordinate of the root.</returns>
        public static double FindRoot(
            Func<double, double> function,
            double startValue,
            double initialStepSize,
            double threshold = 1e-16)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));

            if (initialStepSize == 0)
                throw new ArgumentOutOfRangeException(nameof(initialStepSize));

            if (threshold < 0)
                throw new ArgumentOutOfRangeException(nameof(threshold));

            var e1 = function(startValue);
            var x = startValue + initialStepSize;
            var stepsize = initialStepSize;
            double e;

            var i = 0;

            while ((e = Math.Abs(function(x))) > threshold)
            {
                if (Math.Abs(e - e1) < threshold)
                    break;

                stepsize = stepsize * (e / (e1 - e));

                e1 = e;
                x += stepsize;

                if (i++ > 100)
                    throw new Exception("Not converging.");
            }

            return x;
        }

        /// <summary>
        ///     Approximates the Lambert W function.
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/Lambert_W_function#Numerical_evaluation</remarks>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static double LambertW(double input)
        {
            if (input < -1 / Math.E)
                throw new ArgumentOutOfRangeException(nameof(input));

            double wj = 0;
            double pwj;
            var i = 0;

            double ewj;

            do
            {
                ewj = Math.Exp(wj);
                pwj = wj;
                wj = wj - (wj * ewj - input) / (ewj * (wj + 1) - (wj + 2) * (wj * ewj - input) / (2 * wj + 2));
                i++;
                if (i > 1000)
                    throw new Exception("Not converging...");
            }
            while (Math.Abs(wj - pwj) > 1e-15);

            ewj = Math.Exp(wj);
            wj = wj - (wj * ewj - input) / (ewj * (wj + 1) - (wj + 2) * (wj * ewj - input) / (2 * wj + 2));

            return wj;
        }

        /// <summary>
        ///     Finds the minimum distance between two neighbouring points of an array.
        /// </summary>
        /// <param name="input">The array.</param>
        /// <returns>The result.</returns>
        public static double MinimumDistance(IEnumerable<double> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var inputlist = input.ToReadOnlyList();

            var ret = double.PositiveInfinity;
            for (var c = 1; c < inputlist.Count; c++)
            {
                ret = Math.Min(ret, inputlist[c] - inputlist[c - 1]);
            }

            return ret;
        }

        /// <summary>
        ///     Calculates the modulus (remainder of a division).
        /// </summary>
        /// <param name="x">The dividend.</param>
        /// <param name="m">The divisor.</param>
        /// <returns>The remainder of the division.</returns>
        public static int Mod(int x, int m)
        {
            if (x == 0)
                return 0;

            if (m == 0)
                throw new ArgumentOutOfRangeException(nameof(m));

            return (x % m + m) % m;
        }

        /// <summary>
        ///     Calculates the modulus (remainder of a division).
        /// </summary>
        /// <param name="x">The dividend.</param>
        /// <param name="m">The divisor.</param>
        /// <returns>The remainder of the division.</returns>
        public static double Mod(double x, double m)
        {
            if (x == 0)
                return 0;

            if (m == 0)
                return double.NaN;

            return (x % m + m) % m;
        }

        /// <summary>
        ///     Calculates the modified bessel function of the first kind for a single value.
        /// </summary>
        /// <param name="x">The value.</param>
        /// <returns>The result.</returns>
        //TODO: Find better algorithm
        public static double ModBessel0(double x)
        {
            if (x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (x < 4.9)
            {
                return 1 + Math.Pow(x, 2) / 4 + Math.Pow(x, 4) / 64 + Math.Pow(x, 6) / 2304 + Math.Pow(x, 8) / 147456 +
                       Math.Pow(x, 10) / 14745600;
            }

            if (x > 5.1)
            {
                return Math.Pow(Math.E, x) / Math.Sqrt(2 * Math.PI * x) *
                       (1 + 1 / (8 * x) + 9 / (128 * Math.Pow(x, 2)) + 225 / (3072 * Math.Pow(x, 3)) + 11025 / (98304 * Math.Pow(x, 4)) +
                        893025 / (3932160 * x));
            }

            var t1 = 1 + Math.Pow(x, 2) / 4 + Math.Pow(x, 4) / 64 + Math.Pow(x, 6) / 2304 + Math.Pow(x, 8) / 147456 +
                     Math.Pow(x, 10) / 14745600;
            var t2 = Math.Pow(Math.E, x) / Math.Sqrt(2 * Math.PI * x) *
                     (1 + 1 / (8 * x) + 9 / (128 * Math.Pow(x, 2)) + 225 / (3072 * Math.Pow(x, 3)) + 11025 / (98304 * Math.Pow(x, 4)) +
                      893025 / (3932160 * x));
            return t1 * (5.1 - x) / 0.2 + t2 * (x - 4.9) / 0.2;
        }

        /// <summary>
        ///     Calculates the sinc = sin(pi * x) / (pi * x) of a single value.
        /// </summary>
        /// <param name="x">The value.</param>
        /// <returns>The result.</returns>
        public static double Sinc(double x)
        {
            if (x == 0)
                return 1;

            return Math.Sin(Math.PI * x) / (Math.PI * x);
        }
    }
}