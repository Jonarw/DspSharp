using System.Collections.Generic;

namespace Filter.Algorithms
{
    public static class Experimental
    {
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
            int c = 0;

            double currentEnergy = 0.0;
            double previousEnergy = 0.0;

            using (var e = input.GetEnumerator())
            {
                while (e.MoveNext() && c < initialLength)
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