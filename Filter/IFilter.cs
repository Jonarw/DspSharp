using System.Collections.Generic;

namespace Filter
{
    public interface IFilter
    {
        double Samplerate { get; set; }
        IEnumerable<double> Process(IEnumerable<double> input);
        bool HasInfiniteImpulseResponse { get; }
    }

    public delegate void ChangeEventHandler(IFilter sender);
}