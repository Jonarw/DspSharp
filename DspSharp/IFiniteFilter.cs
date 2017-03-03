using System.Collections.Generic;

namespace DspSharp
{
    /// <summary>
    ///     Describes a digital filter with a finite impulse response.
    /// </summary>
    /// <seealso cref="IFilter" />
    public interface IFiniteFilter : IFilter
    {
        /// <summary>
        ///     Processes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        IReadOnlyList<double> Process(IReadOnlyList<double> input);
    }
}