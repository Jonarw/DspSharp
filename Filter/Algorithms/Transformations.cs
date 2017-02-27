using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Filter.Extensions;

namespace Filter.Algorithms
{
    public static class Transformations
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
            while (signal.Skip(currentLength).Take(currentLength).CalculateEnergy() / signal.Take(currentLength).CalculateEnergy() > energyRatio)
            {
                currentLength *= 2;
                if (currentLength > maximumLength)
                {
                    break;
                }
            }

            return Fft.RealFft(signal.Take(currentLength));
            // ReSharper restore PossibleMultipleEnumeration
        }
    }
}