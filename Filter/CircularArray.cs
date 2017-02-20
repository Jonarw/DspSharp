﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Filter
{
    public class CircularArray<T>
    {
        public CircularArray(IReadOnlyList<T> items)
        {
            this.Items = items.ToArray();
            this.Length = this.Items.Length;
        }

        public int Position { get; private set; }
        private T[] Items { get; }
        private int Length { get; }

        public event EventHandler Wrap;

        public T[] GetRange(int count)
        {
            var ret = new T[count];

            if (count + this.Position < this.Length)
            {
                Array.Copy(this.Items, this.Position, ret, 0, count);
                this.Position += count;
            }
            else
            {
                var tmp = this.Length - this.Position;
                Array.Copy(this.Items, this.Position, ret, 0, tmp);
                this.Wrap?.Invoke(this, EventArgs.Empty);

                while (count - tmp > this.Length)
                {
                    Array.Copy(this.Items, 0, ret, tmp, this.Length);
                    tmp += this.Length;
                    this.Wrap?.Invoke(this, EventArgs.Empty);
                }

                Array.Copy(this.Items, 0, ret, tmp, count - tmp);
                this.Position = count - tmp;
            }

            return ret;
        }

        public T GetValue()
        {
            var ret = this.Items[this.Position++];

            if (this.Position > this.Items.Length)
            {
                this.Position -= this.Items.Length;
                this.Wrap?.Invoke(this, EventArgs.Empty);
            }

            return ret;
        }
    }
}