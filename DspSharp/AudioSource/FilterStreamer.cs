// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterStreamer.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DspSharp.Algorithms;
using DspSharp.Filter;

namespace DspSharp.AudioSource
{
    public class FilterStreamer
    {
        private readonly IFilter _filter;

        public FilterStreamer(IFilter filter)
        {
            this._filter = filter;
            this.StreamEnumerator = filter.Process(this.Stream).GetEnumerator();
        }

        private IReadOnlyList<double> CurrentBlock { get; set; }

        private IEnumerable<double> Stream
        {
            get
            {
                while (true)
                {
                    foreach (var d in this.CurrentBlock)
                    {
                        yield return d;
                    }
                }
                // ReSharper disable once IteratorNeverReturns
            }
        }

        private IEnumerator<double> StreamEnumerator { get; }

        public void InputBlock(IEnumerable<double> block)
        {
            this.CurrentBlock = block.ToReadOnlyList();
        }

        public double[] OutputBlock()
        {
            if (this.CurrentBlock == null)
                throw new InvalidOperationException();

            return this.GetBlock();
        }

        public double[] StreamBlock(IEnumerable<double> block)
        {
            this.InputBlock(block);
            return this.GetBlock();
        }

        private double[] GetBlock()
        {
            lock (this.StreamEnumerator)
            {
                var ret = new double[this.CurrentBlock.Count];

                for (var i = 0; i < this.CurrentBlock.Count; i++)
                {
                    this.StreamEnumerator.MoveNext();
                    ret[i] = this.StreamEnumerator.Current;
                }

                return ret;
            }
        }
    }
}