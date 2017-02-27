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
    }
}