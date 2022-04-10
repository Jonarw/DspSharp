// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularBuffer.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Algorithms;
using System;

namespace DspSharp.Buffers
{
    /// <summary>
    /// Represents a circular buffer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularBuffer<T>
    {
        private readonly T[] storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBuffer{T}" /> class.
        /// </summary>
        /// <param name="length">The length of the buffer.</param>
        public CircularBuffer(int length)
        {
            this.storage = new T[length];
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        public int Length => this.storage.Length;

        private int Position { get; set; }

        /// <summary>
        /// Retrieves an item without changing the current position.
        /// </summary>
        /// <param name="position">
        /// The position of the item. 0 is the current item, -1 the previous item and +1 the next item.
        /// </param>
        public T Peek(int position)
        {
            return this.storage[Mathematic.Mod(this.Position + position, this.Length)];
        }

        /// <summary>
        /// Retrieves the current value and increments the current position.
        /// </summary>
        public T Retrieve()
        {
            var ret = this.storage[this.Position];
            this.Increment(1);
            return ret;
        }

        /// <summary>
        /// Stores a new value at the current position and increments the current position.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Store(T item)
        {
            this.storage[this.Position] = item;
            this.Increment(1);
        }

        /// <summary>
        /// Stores the specified items starting at the current position and increments the current position be the number of items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void Store(T[] items)
        {
            var length = items.Length;

            if (length + this.Position <= this.Length)
            {
                Array.Copy(items, 0, this.storage, this.Position, length);
            }
            else
            {
                if (length <= this.Length)
                {
                    var remaining = this.Length - this.Position;

                    Array.Copy(items, 0, this.storage, this.Position, remaining);
                    Array.Copy(items, remaining, this.storage, 0, length - remaining);
                }
                else
                {
                    var remaining = this.Length - this.Position;
                    var difference = length - this.Length;

                    Array.Copy(items, difference, this.storage, this.Position, remaining);

                    if (remaining < this.Length)
                        Array.Copy(items, difference + remaining, this.storage, 0, this.Length - remaining);
                }
            }

            this.Increment(length);
        }

        /// <summary>
        /// Retrieves the current value, stores a new value at this position and increments the current position.
        /// </summary>
        /// <param name="item">The item to store.</param>
        public T StoreAndRetrieve(T item)
        {
            var ret = this.storage[this.Position];
            this.storage[this.Position] = item;
            this.Increment(1);
            return ret;
        }

        /// <summary>
        /// Retrieves the number of provided items starting from the current position, stores the provided items to that range and increments the current position by the number of items.
        /// </summary>
        /// <param name="itemsToStore">The items to store.</param>
        /// <param name="retrieveBuffer">The buffer for retrieving items. Must be the same length as <paramref name="itemsToStore"/>.</param>
        public void StoreAndRetrieve(T[] itemsToStore, T[] retrieveBuffer)
        {
            if (retrieveBuffer.Length != itemsToStore.Length)
                throw new InvalidOperationException("The retrieve buffer must be the same length as the store buffer.");

            var length = itemsToStore.Length;

            if (length + this.Position <= this.Length)
            {
                Array.Copy(this.storage, this.Position, retrieveBuffer, 0, length);
                Array.Copy(itemsToStore, 0, this.storage, this.Position, length);
            }
            else
            {
                if (length <= this.Length)
                {
                    var remaining = this.Length - this.Position;

                    Array.Copy(this.storage, this.Position, retrieveBuffer, 0, remaining);
                    Array.Copy(itemsToStore, 0, this.storage, this.Position, remaining);

                    Array.Copy(this.storage, 0, retrieveBuffer, remaining, length - remaining);
                    Array.Copy(itemsToStore, remaining, this.storage, 0, length - remaining);
                }
                else
                {
                    var remaining = this.Length - this.Position;
                    var difference = length - this.Length;

                    Array.Copy(this.storage, this.Position, retrieveBuffer, 0, remaining);
                    Array.Copy(itemsToStore, difference, this.storage, this.Position, remaining);

                    if (remaining < this.Length)
                    {
                        Array.Copy(this.storage, 0, retrieveBuffer, remaining, this.Length - remaining);
                        Array.Copy(itemsToStore, difference + remaining, this.storage, 0, this.Length - remaining);
                    }

                    Array.Copy(itemsToStore, 0, retrieveBuffer, this.Length, difference);
                }
            }

            this.Increment(length);
        }

        private void Increment(int amount)
        {
            this.Position = (this.Position + amount) % this.Length;
        }
    }
}