using System.Collections.Generic;

namespace Filter
{
    /// <summary>
    ///     Describes a digital filter with a finite impulse response.
    /// </summary>
    /// <seealso cref="Filter.IFilter" />
    public interface IFiniteFilter : IFilter
    {
        /// <summary>
        ///     Processes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        IReadOnlyList<double> Process(IReadOnlyList<double> input);
    }
}