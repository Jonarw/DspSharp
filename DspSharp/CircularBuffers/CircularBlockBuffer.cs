// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularBlockBuffer.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using DspSharp.Algorithms;

namespace DspSharp.CircularBuffers
{
    /// <summary>
    ///     Represents a circular block buffer using unmanaged memory.
    /// </summary>
    public unsafe class CircularBlockBuffer
    {
        private readonly byte* buffer;
        private readonly bool ownBuffer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CircularBlockBuffer" /> class from a double array.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="outputBufferSize">Block size of the output buffer (in doubles).</param>
        public CircularBlockBuffer(double[] items, int outputBufferSize) : this(items, sizeof(double), outputBufferSize)
        {
            fixed (void* pItems = items)
            {
                Interop.memcpy(this.buffer, pItems, items.Length * sizeof(double));
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CircularBlockBuffer" /> class from an int array.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="outputBufferSize">Block size of the output buffer (in ints).</param>
        public CircularBlockBuffer(int[] items, int outputBufferSize) : this(items, sizeof(int), outputBufferSize)
        {
            fixed (void* pItems = items)
            {
                Interop.memcpy(this.buffer, pItems, items.Length * sizeof(int));
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CircularBlockBuffer" /> class from a short array.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="outputBufferSize">Block size of the output buffer (in shorts).</param>
        public CircularBlockBuffer(short[] items, int outputBufferSize) : this(items, sizeof(short), outputBufferSize)
        {
            fixed (void* pItems = items)
            {
                Interop.memcpy(this.buffer, pItems, items.Length * sizeof(short));
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CircularBlockBuffer" /> class from a short array.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="outputBufferSize">Block size of the output buffer (in shorts).</param>
        public CircularBlockBuffer(float[] items, int outputBufferSize) : this(items, sizeof(float), outputBufferSize)
        {
            fixed (void* pItems = items)
            {
                Interop.memcpy(this.buffer, pItems, items.Length * sizeof(float));
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CircularBlockBuffer" /> class from a byte array.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="outputBufferSize">Block size of the output buffer (in bytes).</param>
        public CircularBlockBuffer(byte[] items, int outputBufferSize) : this(items, sizeof(byte), outputBufferSize)
        {
            fixed (void* pItems = items)
            {
                Interop.memcpy(this.buffer, pItems, items.Length * sizeof(byte));
            }
        }

        private CircularBlockBuffer(Array items, int dataTypeSize, int outputBufferSize)
            : this((byte*)0, dataTypeSize, items.Length, outputBufferSize)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            this.ownBuffer = true;
            this.buffer = Unsafe.MallocB(items.Length * dataTypeSize);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CircularBlockBuffer" /> class from an unmanaged memory location. The
        ///     caller is responsible for memory allocation and deallocation.
        /// </summary>
        /// <param name="items">The memory location containing the items.</param>
        /// <param name="dataTypeSize">Size of the data type.</param>
        /// <param name="bufferSize">Size of the circular buffer.</param>
        /// <param name="outputBufferSize">Block size of the output buffer.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// </exception>
        public CircularBlockBuffer(void* items, int dataTypeSize, int bufferSize, int outputBufferSize)
        {
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            if ((outputBufferSize <= 0) || (outputBufferSize > bufferSize))
                throw new ArgumentOutOfRangeException(nameof(outputBufferSize));

            this.ownBuffer = false;
            this.DataTypeSize = dataTypeSize;
            this.OutputBufferSize = outputBufferSize;
            this.buffer = (byte*)items;
            this.BufferSize = bufferSize;
        }

        /// <summary>
        ///     Gets the size of the circular buffer (in whatever data type the buffer was initialized with).
        /// </summary>
        public int BufferSize { get; }

        /// <summary>
        ///     Gets or sets the current position (in whatever data type the buffer was initialized with).
        /// </summary>
        public int BufferPosition { get; set; }

        /// <summary>
        ///     Gets the size of the data type the circular buffer was initialized with.
        /// </summary>
        public int DataTypeSize { get; }

        /// <summary>
        ///     Gets the size of the output buffer (in whatever data type the buffer was initialized with).
        /// </summary>
        public int OutputBufferSize { get; }

        /// <summary>
        ///     Copies <see cref="OutputBufferSize" /> items from the circular buffer to the specified memory location. If the end
        ///     of the circular buffer is reached, wraps around to the start.
        /// </summary>
        /// <param name="target">The target.</param>
        public void GetBlock(byte* target)
        {
            if (this.BufferPosition + this.OutputBufferSize < this.BufferSize)
            {
                Interop.memcpy(target, this.buffer + this.BufferPosition * this.DataTypeSize, this.OutputBufferSize * this.DataTypeSize);
                this.BufferPosition += this.OutputBufferSize;
            }
            else
            {
                var c = this.BufferSize - this.BufferPosition;

                Interop.memcpy(target, this.buffer + this.BufferPosition * this.DataTypeSize, c * this.DataTypeSize);
                this.BufferPosition = this.OutputBufferSize - c;
                Interop.memcpy(target + c * this.DataTypeSize, this.buffer, this.BufferPosition * this.DataTypeSize);
            }
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="CircularBlockBuffer" /> class.
        /// </summary>
        ~CircularBlockBuffer()
        {
            if (this.ownBuffer)
                Unsafe.Free(this.buffer);
        }
    }
}