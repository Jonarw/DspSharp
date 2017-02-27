using System;
using System.Collections.Generic;

namespace Filter.AudioSource
{
    public class BufferCompletedEventArgs : EventArgs
    {
        public IReadOnlyList<IntPtr> Inputs { get; }
        public IReadOnlyList<IntPtr> Outputs { get; }

        public BufferCompletedEventArgs(IReadOnlyList<IntPtr> inputs, IReadOnlyList<IntPtr> outputs)
        {
            this.Inputs = inputs;
            this.Outputs = outputs;
        }
    }
}