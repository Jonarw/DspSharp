// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferSwitchEventArgs.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace DspSharp.Buffers
{
    /// <summary>
    /// Represents event data for a BufferSwitchEvent.
    /// </summary>
    public unsafe class BufferSwitchEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BufferSwitchEventArgs" /> class.
        /// </summary>
        /// <param name="newWorkBuffer">The new work buffer after the buffer switch.</param>
        /// <param name="newInputBuffer">The new input buffer after the buffer switch.</param>
        public BufferSwitchEventArgs(T[] newWorkBuffer, T[] newInputBuffer)
        {
            this.NewWorkBuffer = newWorkBuffer;
            this.NewInputBuffer = newInputBuffer;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the buffer switch operation should be canceled. In that case, the buffers
        /// will not be switched; the <see cref="DoubleBlockBuffer" /> will continue its operation overwriting the current
        /// input block.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets the new input buffer (after the buffer switch).
        /// </summary>
        public T[] NewInputBuffer { get; }

        /// <summary>
        /// Gets the new work buffer (after the buffer switch).
        /// </summary>
        public T[] NewWorkBuffer { get; }
    }
}