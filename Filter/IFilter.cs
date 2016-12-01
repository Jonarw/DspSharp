using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Filter
{
    public interface IFilter : INotifyPropertyChanged
    {
        double Samplerate { get; }
        IEnumerable<double> Process(IEnumerable<double> input);
        bool HasInfiniteImpulseResponse { get; }
    }

    public delegate void ChangeEventHandler(IFilter sender, FilterChangedEventArgs e);

    public class FilterChangedEventArgs : EventArgs
    {
    }
}