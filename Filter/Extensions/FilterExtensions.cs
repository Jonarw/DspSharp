using Filter.Exceptions;
using Filter.Signal;

namespace Filter.Extensions
{
    /// <summary>
    ///     Provides static extension for the IFilter interface.
    /// </summary>
    public static class FilterExtensions
    {
        /// <summary>
        ///     Computes the impulse response of the filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The impulse response.</returns>
        public static IEnumerableSignal GetImpulseResponse(this IFilter filter)
        {
            return new EnumerableSignal(filter.Process(1.0.ToEnumerable()), filter.Samplerate);
        }

        /// <summary>
        ///     Processes the specified signal with the provided filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="signal">The signal.</param>
        /// <returns>The filtered signal</returns>
        /// <exception cref="SamplerateMismatchException"></exception>
        public static IEnumerableSignal Process(this IFilter filter, IEnumerableSignal signal)
        {
            if (filter.Samplerate != signal.SampleRate)
            {
                throw new SamplerateMismatchException();
            }

            if (filter.HasInfiniteImpulseResponse || !(signal is FiniteSignal))
            {
                return new EnumerableSignal(filter.Process(signal.Signal), filter.Samplerate, signal.Start) { DisplayName = "filter result" };
            }

            return new FiniteSignal(filter.Process(signal.Signal).ToReadOnlyList(), filter.Samplerate, signal.Start) { DisplayName = "filter result" };
        }
    }
}