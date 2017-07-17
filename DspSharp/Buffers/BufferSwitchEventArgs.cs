// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferSwitchEventArgs.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace DspSharp.Buffers
{
    /// <summary>
    ///     Represents event data for a BufferSwitchEvent.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public unsafe class BufferSwitchEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BufferSwitchEventArgs" /> class.
        /// </summary>
        /// <param name="newWorkBuffer">The new work buffer after the buffer switch.</param>
        /// <param name="newInputBuffer">The new input buffer after the buffer switch.</param>
        public BufferSwitchEventArgs(byte* newWorkBuffer, byte* newInputBuffer)
        {
            this.NewWorkBuffer = newWorkBuffer;
            this.NewInputBuffer = newInputBuffer;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the buffer switch operation should be canceled. In that case, the buffers
        ///     will not be switched; the <see cref="DoubleBlockBuffer" /> will continue its operation overwriting the current
        ///     input block.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        ///     Gets the new input buffer (after the buffer switch).
        /// </summary>
        public byte* NewInputBuffer { get; }

        /// <summary>
        ///     Gets the new work buffer (after the buffer switch).
        /// </summary>
        public byte* NewWorkBuffer { get; }
    }
}