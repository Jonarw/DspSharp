// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularBuffer.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using DspSharp.Algorithms;

namespace DspSharp.Buffers
{
    /// <summary>
    ///     Represents a circular buffer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularBuffer<T>
    {
        private readonly T[] storage;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CircularBuffer{T}" /> class.
        /// </summary>
        /// <param name="length">The length of the buffer.</param>
        public CircularBuffer(int length)
        {
            this.storage = new T[length];
        }

        /// <summary>
        ///     Gets or sets the length.
        /// </summary>
        public int Length => this.storage.Length;

        private int Position { get; set; }

        /// <summary>
        ///     Retrieves an item without changing the current position.
        /// </summary>
        /// <param name="position">
        ///     The position of the item where 0 is the current item, -1 the item before that and +1 the next
        ///     item to come.
        /// </param>
        /// <returns></returns>
        public T Peek(int position)
        {
            return this.storage[Mathematic.Mod(this.Position + position, this.Length)];
        }

        /// <summary>
        ///     Stores a new value at the current position and increments the current position.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Store(T item)
        {
            this.storage[this.Position] = item;
            this.Position = (this.Position + 1) % this.Length;
        }

        /// <summary>
        ///     Stores the specified items starting at the current position and increments the current position be the number of
        ///     items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void Store(IEnumerable<T> items)
        {
            var itemarray = items.ToArray();

            if (itemarray.Length + this.Position <= this.Length)
                Array.Copy(itemarray, 0, this.storage, this.Position, itemarray.Length);
            else
            {
                if (itemarray.Length <= this.Length)
                {
                    var remaining = this.Length - this.Position;

                    Array.Copy(itemarray, 0, this.storage, this.Position, remaining);

                    Array.Copy(itemarray, remaining, this.storage, 0, itemarray.Length - remaining);
                }
                else
                {
                    var remaining = this.Length - this.Position;
                    var difference = itemarray.Length - this.Length;

                    Array.Copy(itemarray, difference, this.storage, this.Position, remaining);

                    if (remaining < this.Length)
                        Array.Copy(itemarray, difference + remaining, this.storage, 0, this.Length - remaining);
                }
            }

            this.Position = (this.Position + itemarray.Length) % this.Length;
        }

        /// <summary>
        ///     Retrieves the current value, stores a new value at this position and increments the current position.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public T StoreAndRetrieve(T item)
        {
            var ret = this.storage[this.Position];
            this.storage[this.Position] = item;
            this.Position = (this.Position + 1) % this.Length;
            return ret;
        }

        /// <summary>
        ///     Retrieves the number of provided items starting from the current position, stores the provided items to that range
        ///     and increments the current position by the number of provided items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public T[] StoreAndRetrieve(IEnumerable<T> items)
        {
            var itemarray = items.ToArray();

            var ret = new T[itemarray.Length];

            if (itemarray.Length + this.Position <= this.Length)
            {
                Array.Copy(this.storage, this.Position, ret, 0, itemarray.Length);
                Array.Copy(itemarray, 0, this.storage, this.Position, itemarray.Length);
            }
            else
            {
                if (itemarray.Length <= this.Length)
                {
                    var remaining = this.Length - this.Position;

                    Array.Copy(this.storage, this.Position, ret, 0, remaining);
                    Array.Copy(itemarray, 0, this.storage, this.Position, remaining);

                    Array.Copy(this.storage, 0, ret, remaining, itemarray.Length - remaining);
                    Array.Copy(itemarray, remaining, this.storage, 0, itemarray.Length - remaining);
                }
                else
                {
                    var remaining = this.Length - this.Position;
                    var difference = itemarray.Length - this.Length;

                    Array.Copy(this.storage, this.Position, ret, 0, remaining);
                    Array.Copy(itemarray, difference, this.storage, this.Position, remaining);

                    if (remaining < this.Length)
                    {
                        Array.Copy(this.storage, 0, ret, remaining, this.Length - remaining);
                        Array.Copy(itemarray, difference + remaining, this.storage, 0, this.Length - remaining);
                    }

                    Array.Copy(itemarray, 0, ret, this.Length, difference);
                }
            }

            this.Position = (this.Position + itemarray.Length) % this.Length;

            return ret;
        }
    }
}