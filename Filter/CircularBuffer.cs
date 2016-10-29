using System;
using System.Collections.Generic;
using System.Linq;

namespace Filter
{
    public class CircularBuffer<T>
    {
        private T[] storage;

        public CircularBuffer(int length)
        {
            this.storage = new T[length];
        }

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

        public T StoreAndRetrieve(T item)
        {
            T ret = this.storage[this.Position];
            this.storage[this.Position] = item;
            this.Position = (this.Position + 1) % this.Length;
            return ret;
        }

        public void Store(T item)
        {
            this.storage[this.Position] = item;
            this.Position = (this.Position + 1) % this.Length;
        }

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

        public T Peek(int position)
        {
            return this.storage[(this.Position - position + this.Length * 1000) % this.Length];
        }

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
    }
}