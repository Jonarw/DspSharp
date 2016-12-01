using System;
using System.Collections.Generic;
using System.Linq;

namespace Filter
{
    /// <summary>
    ///     Represents a circular buffer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularBuffer<T>
    {
        private T[] storage;

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
        public int Length
        {
            get { return this.storage.Length; }
            set
            {
                if (value != this.Length)
                {
                    this.storage = this.PeekRange(value).ToArray();
                    this.Position = 0;
                }
            }
        }

        private int Position { get; set; }

        /// <summary>
        ///     Retrieves an item without changing the current position.
        /// </summary>
        /// <param name="position">The position of the item where 0 is the current item, 1 the item before that and so on.</param>
        /// <returns></returns>
        public T Peek(int position)
        {
            return this.storage[(this.Position - position + this.Length * 1000) % this.Length];
        }

        /// <summary>
        ///     Retrieves a range of the last items, stopping at the item before the current item, without changing the current
        ///     position.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public T[] PeekRange(int length)
        {
            var ret = new T[length];

            int actualLength = length % this.Length;

            if (this.Position < actualLength - 1)
            {
                int remaining = actualLength - this.Position - 1;
                Array.Copy(this.storage, 0, ret, remaining, this.Position + 1);
                Array.Copy(this.storage, this.Length - remaining - 1, ret, 0, remaining);
            }
            else
            {
                if (actualLength == 0)
                {
                    Array.Copy(this.storage, this.Position - actualLength, ret, 0, this.Length);
                }
                else
                {
                    Array.Copy(this.storage, this.Position - actualLength, ret, 0, actualLength);
                }
            }

            return ret;
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
            {
                Array.Copy(itemarray, 0, this.storage, this.Position, itemarray.Length);
            }
            else if (itemarray.Length <= this.Length)
            {
                int remaining = this.Length - this.Position;

                Array.Copy(itemarray, 0, this.storage, this.Position, remaining);

                Array.Copy(itemarray, remaining, this.storage, 0, itemarray.Length - remaining);
            }
            else
            {
                int remaining = this.Length - this.Position;
                int difference = itemarray.Length - this.Length;

                Array.Copy(itemarray, difference, this.storage, this.Position, remaining);

                if (remaining < this.Length)
                {
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
            T ret = this.storage[this.Position];
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

            T[] ret = new T[itemarray.Length];

            if (itemarray.Length + this.Position <= this.Length)
            {
                Array.Copy(this.storage, this.Position, ret, 0, itemarray.Length);
                Array.Copy(itemarray, 0, this.storage, this.Position, itemarray.Length);
            }
            else if (itemarray.Length <= this.Length)
            {
                int remaining = this.Length - this.Position;

                Array.Copy(this.storage, this.Position, ret, 0, remaining);
                Array.Copy(itemarray, 0, this.storage, this.Position, remaining);

                Array.Copy(this.storage, 0, ret, remaining, itemarray.Length - remaining);
                Array.Copy(itemarray, remaining, this.storage, 0, itemarray.Length - remaining);
            }
            else
            {
                int remaining = this.Length - this.Position;
                int difference = itemarray.Length - this.Length;

                Array.Copy(this.storage, this.Position, ret, 0, remaining);
                Array.Copy(itemarray, difference, this.storage, this.Position, remaining);

                if (remaining < this.Length)
                {
                    Array.Copy(this.storage, 0, ret, remaining, this.Length - remaining);
                    Array.Copy(itemarray, difference + remaining, this.storage, 0, this.Length - remaining);
                }

                Array.Copy(itemarray, 0, ret, this.Length, difference);
            }

            this.Position = (this.Position + itemarray.Length) % this.Length;

            return ret;
        }
    }
}