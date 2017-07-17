// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularBlockBuffer.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using DspSharp.Algorithms;

namespace DspSharp.Buffers
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
        public CircularBlockBuffer(double[] items) : this(items, sizeof(double))
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
        public CircularBlockBuffer(int[] items) : this(items, sizeof(int))
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
        public CircularBlockBuffer(short[] items) : this(items, sizeof(short))
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
        public CircularBlockBuffer(float[] items) : this(items, sizeof(float))
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
        public CircularBlockBuffer(byte[] items) : this(items, sizeof(byte))
        {
            fixed (void* pItems = items)
            {
                Interop.memcpy(this.buffer, pItems, items.Length * sizeof(byte));
            }
        }

        private CircularBlockBuffer(Array items, int dataTypeSize)
            : this((byte*)0, dataTypeSize, items.Length)
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
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// </exception>
        public CircularBlockBuffer(void* items, int dataTypeSize, int bufferSize)
        {
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            this.ownBuffer = false;
            this.DataTypeSize = dataTypeSize;
            this.buffer = (byte*)items;
            this.BufferSize = bufferSize;
        }

        public event PeriodCompletedEventHandler PeriodCompleted;

        /// <summary>
        ///     Gets or sets the current position (in whatever data type the buffer was initialized with).
        /// </summary>
        public int BufferPosition { get; set; }

        /// <summary>
        ///     Gets the size of the circular buffer (in whatever data type the buffer was initialized with).
        /// </summary>
        public int BufferSize { get; }

        /// <summary>
        ///     Gets the size of the data type the circular buffer was initialized with.
        /// </summary>
        public int DataTypeSize { get; }

        /// <summary>
        ///     Copies <see cref="length" /> items from the circular buffer to the specified memory location. If the end
        ///     of the circular buffer is reached, wraps around to the start.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="length">The length.</param>
        public void GetBlock(byte* target, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (this.BufferPosition + length < this.BufferSize)
            {
                Interop.memcpy(target, this.buffer + this.BufferPosition * this.DataTypeSize, length * this.DataTypeSize);
                this.BufferPosition += length;
            }
            else
            {
                var c = this.BufferSize - this.BufferPosition;

                Interop.memcpy(target, this.buffer + this.BufferPosition * this.DataTypeSize, c * this.DataTypeSize);

                while (length - c > this.BufferSize)
                {
                    Interop.memcpy(target + c * this.DataTypeSize, this.buffer, this.BufferSize * this.DataTypeSize);
                    c += this.BufferSize;
                    this.PeriodCompleted?.Invoke(this, EventArgs.Empty);
                }

                this.BufferPosition = length - c;
                Interop.memcpy(target + c * this.DataTypeSize, this.buffer, this.BufferPosition * this.DataTypeSize);
                this.PeriodCompleted?.Invoke(this, EventArgs.Empty);
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

    public delegate void PeriodCompletedEventHandler(object sender, EventArgs args);
}