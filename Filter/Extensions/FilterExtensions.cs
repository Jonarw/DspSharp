using Filter.Exceptions;
using Filter.Signal;

namespace Filter.Extensions
{
    public static class FilterExtensions
    {
        public static IEnumerableSignal GetImpulseResponse(this IFilter filter)
        {
            return new EnumerableSignal(filter.Process(1.0.ToEnumerable()), filter.Samplerate);
        }

        public static IEnumerableSignal Process(this IFilter filter, IEnumerableSignal signal)
        {
            if (filter.Samplerate != signal.SampleRate)
            {
                throw new SamplerateMismatchException();
            }

            return new EnumerableSignal(filter.Process(signal.Signal), filter.Samplerate, signal.Start);
        }
    }
}