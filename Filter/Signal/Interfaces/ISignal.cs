using System.Collections.Generic;

namespace Filter.Signal
{
    public interface ISignal
    {
        IEnumerable<double> GetWindowedSignal(int start, int length);
        double SampleRate { get; }
        string Name { get; set; }
    }
}