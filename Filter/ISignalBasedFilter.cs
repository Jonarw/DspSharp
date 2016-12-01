using System.Collections.ObjectModel;
using Filter.Signal;

namespace Filter
{
    /// <summary>
    /// Describes a filter that used a predefined signal from a list of available signals.
    /// </summary>
    public interface ISignalBasedFilter
    {
        /// <summary>
        /// Gets or sets the available signals.
        /// </summary>
        ObservableCollection<ISignal> AvailableSignals { get; set; }
    }
}