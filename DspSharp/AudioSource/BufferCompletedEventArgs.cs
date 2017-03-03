using System;
using System.Collections.Generic;

namespace DspSharp.AudioSource
{
    public class BufferCompletedEventArgs : EventArgs
    {
        public BufferCompletedEventArgs(IReadOnlyList<IntPtr> inputs, IReadOnlyList<IntPtr> outputs)
        {
            this.Inputs = inputs;
            this.Outputs = outputs;
        }

        public IReadOnlyList<IntPtr> Inputs { get; }
        public IReadOnlyList<IntPtr> Outputs { get; }
    }
}