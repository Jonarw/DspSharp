// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleBlockBuffer.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace DspSharp.Buffers
{
    /// <summary>
    /// Represents a double buffer using unmanaged memory.
    /// </summary>
    public class DoubleBuffer<T>
    {
        private T[] buffer0;
        private T[] buffer1;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleBlockBuffer" /> class.
        /// </summary>
        /// <param name="bufferSize">The size of the double buffers.</param>
        public DoubleBuffer(int bufferSize)
        {
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            this.buffer0 = new T[bufferSize];
            this.buffer1 = new T[bufferSize];
            this.BufferSize = bufferSize;
        }

        /// <summary>
        ///     Gets or sets the current buffer position.
        /// </summary>
        public int BufferPosition { get; set; }

        /// <summary>
        ///     Gets the size of the double buffers.
        /// </summary>
        public int BufferSize { get; }

        /// <summary>
        ///     Occurs when the current input buffer is completely filled and a buffer switch occurs.
        /// </summary>
        public event EventHandler<BufferSwitchEventArgs<T>>? BufferSwitch;

        /// <summary>
        /// Transfers an input block to the current input buffer at the current <see cref="BufferPosition" /> and advances the <see cref="BufferPosition" />. If the current input buffer is completely filled, a <see cref="BufferSwitch" /> event will be invoked.
        /// </summary>
        /// <param name="block">The block.</param>
        public void InputBlock(T[] block)
        {
            var length = block.Length;
            if (length + this.BufferPosition < this.BufferSize)
            {
                Array.Copy(block, 0, this.buffer0, this.BufferPosition, block.Length);
                this.BufferPosition += length;
            }
            else
            {
                var c = this.BufferSize - this.BufferPosition;
                Array.Copy(block, 0, this.buffer0, this.BufferPosition, c);

                this.SwitchBuffers();

                while (length - c >= this.BufferSize)
                {
                    Array.Copy(block, c, this.buffer0, 0, this.BufferSize);
                    c += this.BufferSize;
                    this.SwitchBuffers();
                }

                this.BufferPosition = length - c;
                Array.Copy(block, c, this.buffer0, 0, this.BufferPosition);
            }
        }

        private void SwitchBuffers()
        {
            var e = new BufferSwitchEventArgs<T>(this.buffer0, this.buffer1);
            this.BufferSwitch?.Invoke(this, e);
            if (e.Cancel)
                return;

            var tmp = this.buffer0;
            this.buffer0 = this.buffer1;
            this.buffer1 = tmp;
        }
    }
}