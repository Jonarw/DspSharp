// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleBlockBuffer.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using DspSharp.Algorithms;

namespace DspSharp.CircularBuffers
{
    /// <summary>
    ///     Represents a double buffer using unmanaged memory.
    /// </summary>
    public unsafe class DoubleBlockBuffer
    {
        /// <summary>
        ///     Event handler for the <see cref="BufferSwitch" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="BufferSwitchEventArgs" /> instance containing the event data.</param>
        public delegate void BufferSwitchEventHandler(DoubleBlockBuffer sender, BufferSwitchEventArgs e);

        private byte* inputbuffer;
        private readonly bool ownBuffers;
        private byte* workbuffer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DoubleBlockBuffer" /> class. The double buffer will allocate and
        ///     manage its own memory blocks.
        /// </summary>
        /// <param name="bufferSize">The size of the double buffers.</param>
        /// <param name="inputBufferSize">The size of the individual input blocks.</param>
        public DoubleBlockBuffer(int bufferSize, int inputBufferSize)
            : this(bufferSize, inputBufferSize, Unsafe.MallocB(bufferSize), Unsafe.MallocB(bufferSize))
        {
            this.ownBuffers = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DoubleBlockBuffer" /> class using preallocated memory blocks. The
        ///     caller is responsible for allocating memory blocks of appropriate size (>= <paramref name="bufferSize" />) and for
        ///     deallocating them after use.
        /// </summary>
        /// <param name="bufferSize">The size of the double buffers.</param>
        /// <param name="inputBufferSize">The size of the individual input blocks.</param>
        /// <param name="buffer0">The first memory block.</param>
        /// <param name="buffer1">The second memory block.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// </exception>
        public DoubleBlockBuffer(int bufferSize, int inputBufferSize, void* buffer0, void* buffer1)
        {
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            if (inputBufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(inputBufferSize));
            if (inputBufferSize > bufferSize)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            this.BufferSize = bufferSize;
            this.InputBufferSize = inputBufferSize;

            this.ownBuffers = false;
            this.workbuffer = (byte*)buffer0;
            this.inputbuffer = (byte*)buffer1;
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
        ///     Gets the size of the input buffer.
        /// </summary>
        public int InputBufferSize { get; }

        /// <summary>
        ///     Occurs when the current input buffer is completely filled and a buffer switch occurs.
        /// </summary>
        public event BufferSwitchEventHandler BufferSwitch;

        /// <summary>
        ///     Transfers an input block to the current input buffer at the current <see cref="BufferPosition" /> and advances the
        ///     <see cref="BufferPosition" />. If the current input buffer is completely filled, a <see cref="BufferSwitch" />
        ///     event will be invoked.
        /// </summary>
        /// <param name="block">The block.</param>
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
            var e = new BufferSwitchEventArgs(this.inputbuffer, this.workbuffer);
            this.BufferSwitch?.Invoke(this, e);
            if (e.Cancel)
                return;

            var tmp = this.workbuffer;
            this.workbuffer = this.inputbuffer;
            this.inputbuffer = tmp;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="DoubleBlockBuffer" /> class.
        /// </summary>
        ~DoubleBlockBuffer()
        {
            if (this.ownBuffers)
            {
                Unsafe.Free(this.inputbuffer);
                Unsafe.Free(this.workbuffer);
            }
        }
    }
}