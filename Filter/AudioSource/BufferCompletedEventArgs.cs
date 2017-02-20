using System;
using System.Collections.Generic;

namespace Filter.AudioSource
{
    public class BufferCompletedEventArgs : EventArgs
    {
        public IReadOnlyList<double[]> Inputs { get; }
        public IReadOnlyList<double[]> Outputs { get; }

        public BufferCompletedEventArgs(IReadOnlyList<double[]> inputs, IReadOnlyList<double[]> outputs)
        {
            this.Inputs = inputs;
            this.Outputs = outputs;
        }
    }
}