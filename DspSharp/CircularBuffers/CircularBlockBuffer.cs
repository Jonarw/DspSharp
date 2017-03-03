using System;
using DspSharp.Algorithms.FftwProvider;

namespace DspSharp.CircularBuffers
{
    public unsafe class CircularBlockBuffer
    {
        private readonly double* buffer;

        public CircularBlockBuffer(double[] items, int outputBufferSize)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if ((outputBufferSize <= 0) || (outputBufferSize > items.Length))
                throw new ArgumentOutOfRangeException(nameof(outputBufferSize));

            this.OutputBufferSize = outputBufferSize;
            this.buffer = (double*)FftwInterop.malloc(items.Length * sizeof(double));
            this.BufferSize = items.Length;

            fixed (void* pItems = items)
            {
                Interop.memcpy(this.buffer, pItems, items.Length * sizeof(double));
            }
        }

        public int BufferSize { get; }
        public int CurrentPosition { get; set; }
        public int OutputBufferSize { get; }

        public void GetBlock(double* target)
        {
            if (this.CurrentPosition + this.OutputBufferSize < this.BufferSize)
            {
                Interop.memcpy(target, this.buffer + this.CurrentPosition, this.OutputBufferSize * sizeof(double));
                this.CurrentPosition += this.OutputBufferSize;
            }
            else
            {
                var c = this.BufferSize - this.CurrentPosition;

                Interop.memcpy(target, this.buffer + this.CurrentPosition, c * sizeof(double));
                this.CurrentPosition = this.OutputBufferSize - c;
                Interop.memcpy(target + c, this.buffer, this.CurrentPosition * sizeof(double));
            }
        }

        ~CircularBlockBuffer()
        {
            FftwInterop.free(this.buffer);
        }
    }
}