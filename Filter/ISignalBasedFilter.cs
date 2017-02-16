using Filter.Signal;
using UmtUtilities.Collections;

namespace Filter
{
    /// <summary>
    ///     Describes a filter that uses a predefined signal from a list of available signals.
    /// </summary>
    public interface ISignalBasedFilter
    {
        /// <summary>
        ///     Gets or sets the available signals.
        /// </summary>
        IReadOnlyObservableList<ISignal> AvailableSignals { get; set; }
    }
}