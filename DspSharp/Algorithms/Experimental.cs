// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Experimental.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DspSharp.Algorithms
{
    public static class Experimental
    {
        /// <summary>
        ///     Experimental; approximates the spectrum of an infinite signal, tries to identify the necessary analysis window
        ///     length.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <param name="energyRatio">The energy ratio.</param>
        /// <param name="initialLength">The initial length.</param>
        /// <param name="maximumLength">The maximum length.</param>
        /// <returns></returns>
        public static IReadOnlyList<Complex> ApproximateSpectrumOfInfiniteSignal(
            IEnumerable<double> signal,
            double energyRatio = 0.00001,
            int initialLength = 1024,
            int maximumLength = 524288)
        {
            var currentLength = initialLength / 2;

            // ReSharper disable PossibleMultipleEnumeration - unavoidable with infinite signal
            while (signal.Skip(currentLength).Take(currentLength).CalculateEnergy() /
                   signal.Take(currentLength).CalculateEnergy() > energyRatio)
            {
                currentLength *= 2;
                if (currentLength > maximumLength)
                    break;
            }

            return Fft.RealFft(signal.Take(currentLength));
            // ReSharper restore PossibleMultipleEnumeration
        }

        /// <summary>
        ///     (Experimental) Finds the index of a sequence that includes the specified portion of the vector's energy.
        /// </summary>
        /// <param name="input">The sequence.</param>
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
            var c = 0;

            var currentEnergy = 0.0;
            var previousEnergy = 0.0;

            using (var e = input.GetEnumerator())
            {
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
                            break;

                        currentLength *= 2;
                        if (currentLength >= maxLength)
                            break;

                        previousEnergy += currentEnergy;
                        currentEnergy = 0;
                    }
                }

                return c;
            }
        }
    }
}