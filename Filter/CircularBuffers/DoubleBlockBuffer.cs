using System;
using Filter.Algorithms.FftwProvider;

namespace Filter.CircularBuffers
{
    public unsafe class DoubleBlockBuffer
    {
        public delegate void BufferSwitchEventHandler(DoubleBlockBuffer sender, byte* buffer);
        private byte* inputbuffer;
        private byte* workbuffer;

        public DoubleBlockBuffer(int bufferSize, int inputBufferSize)
        {
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            if (inputBufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(inputBufferSize));
            if (inputBufferSize > bufferSize)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            this.BufferSize = bufferSize;
            this.InputBufferSize = inputBufferSize;

            this.workbuffer = (byte*)FftwInterop.malloc(this.BufferSize);
            this.inputbuffer = (byte*)FftwInterop.malloc(this.BufferSize);
        }

        public int BufferPosition { get; set; }
        public int BufferSize { get; }
        public int InputBufferSize { get; }
        public event BufferSwitchEventHandler BufferSwitch;

        public void InputBlock(byte* block)
        {
            if (this.InputBufferSize + this.BufferPosition < this.BufferSize)
            {
                Interop.memcpy(this.inputbuffer + this.BufferPosition, block, this.InputBufferSize);
                this.BufferPosition += this.InputBufferSize;
            }
            else
            {
                var c = this.BufferSize - this.BufferPosition;
                Interop.memcpy(this.inputbuffer + this.BufferPosition, block, c);

                this.OnBufferSwitch();

                this.BufferPosition = this.InputBufferSize - c;
                Interop.memcpy(this.inputbuffer, block + c, this.BufferPosition);
            }
        }

        private void OnBufferSwitch()
        {
            var tmp = this.workbuffer;
            this.workbuffer = this.inputbuffer;
            this.inputbuffer = tmp;
            this.BufferSwitch?.Invoke(this, this.workbuffer);
        }

        ~DoubleBlockBuffer()
        {
            FftwInterop.free(this.inputbuffer);
            FftwInterop.free(this.workbuffer);
        }
    }
}