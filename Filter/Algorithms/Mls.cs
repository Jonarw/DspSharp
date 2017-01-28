using System;
using System.Collections.Generic;

namespace Filter.Algorithms
{
    /// <summary>
    ///     Provides functions for generating maximum length sequences.
    /// </summary>
    /// <remarks>https://en.wikipedia.org/wiki/Linear-feedback_shift_register</remarks>
    public static class Mls
    {
        /// <summary>
        ///     Feedback taps for MLS orders 2 to 31.
        /// </summary>
        public static readonly IReadOnlyList<uint> FeedbackTaps = new List<uint>
        {
            0,
            0,
            (1 << 1) | (1 << 0),
            (1 << 2) | (1 << 1),
            (1 << 3) | (1 << 2),
            (1 << 4) | (1 << 2),
            (1 << 5) | (1 << 4),
            (1 << 6) | (1 << 5),
            (1 << 7) | (1 << 5) | (1 << 4) | (1 << 3),
            (1 << 8) | (1 << 4),
            (1 << 9) | (1 << 6),
            (1 << 10) | (1 << 8),
            (1 << 11) | (1 << 10) | (1 << 9) | (1 << 3),
            (1 << 12) | (1 << 11) | (1 << 10) | (1 << 7),
            (1 << 13) | (1 << 12) | (1 << 11) | (1 << 1),
            (1 << 14) | (1 << 13),
            (1 << 15) | (1 << 14) | (1 << 12) | (1 << 3),
            (1 << 16) | (1 << 13),
            (1 << 17) | (1 << 10),
            (1 << 18) | (1 << 17) | (1 << 16) | (1 << 13),
            (1 << 19) | (1 << 16),
            (1 << 20) | (1 << 18),
            (1 << 21) | (1 << 20),
            (1 << 22) | (1 << 17),
            (1 << 23) | (1 << 22) | (1 << 21) | (1 << 16),
            (1 << 24) | (1 << 21),
            (1 << 25) | (1 << 5) | (1 << 1) | (1 << 0),
            (1 << 26) | (1 << 4) | (1 << 1) | (1 << 0),
            (1 << 27) | (1 << 24),
            (1 << 28) | (1 << 26),
            (1 << 29) | (1 << 5) | (1 << 3) | (1 << 0),
            (1 << 30) | (1 << 27)
        }.AsReadOnly();

        /// <summary>
        ///     Generates a maximum length sequence of the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>The maximum length sequence.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static IEnumerable<double> GenerateMls(int order)
        {
            if ((order < 2) || (order > FeedbackTaps.Count - 1))
            {
                throw new ArgumentOutOfRangeException(nameof(order));
            }

            var taps = FeedbackTaps[order];
            const uint startState = 1 << 1;
            uint state = startState;

            do
            {
                uint lsb = 1 & state;
                state >>= 1;

                if (lsb > 0)
                {
                    state ^= taps;
                    yield return 1;
                }
                else
                {
                    yield return -1;
                }
            }
            while (state != startState);
        }
    }
}