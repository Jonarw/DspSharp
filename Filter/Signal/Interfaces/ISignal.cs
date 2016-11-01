using System.Collections.Generic;

namespace Filter.Signal
{
    public interface ISignal
    {
        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     Gets the sample rate.
        /// </summary>
        double SampleRate { get; }

        /// <summary>
        ///     Gets a section of the signal in time domain.
        /// </summary>
        /// <param name="start">The start of the section.</param>
        /// <param name="length">The length of the section.</param>
        /// <returns>The specified section.</returns>
        IEnumerable<double> GetWindowedSignal(int start, int length);
    }
}