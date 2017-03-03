// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhiteNoise.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DspSharp.Algorithms;
using PropertyTools.DataAnnotations;

namespace DspSharp.Signal
{
    /// <summary>
    ///     Represents a white noise signal with (ideally) completely uncorrelated samples.
    /// </summary>
    /// <seealso cref="SignalBase" />
    public class WhiteNoise : SignalBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WhiteNoise" /> class.
        /// </summary>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="mean">The mean.</param>
        /// <param name="variance">The variance.</param>
        public WhiteNoise(double sampleRate, double mean = 0, double variance = 1) : base(sampleRate)
        {
            this.Mean = mean;
            this.Sigma = Math.Sqrt(variance);
            this.Variance = variance;
            this.NoiseSource = SignalGenerators.WhiteNoise().GetEnumerator();
            this.DisplayName = "white noise, µ = " + mean + ",σ² = " + variance;
        }

        private List<double> Cache { get; set; }
        private int CacheEnd { get; set; }
        private int CacheStart { get; set; }
        private IEnumerator<double> NoiseSource { get; }
        private double Sigma { get; }

        /// <summary>
        ///     Gets a section of the signal in time domain.
        /// </summary>
        /// <param name="start">The start of the section.</param>
        /// <param name="length">The length of the section.</param>
        /// <returns>
        ///     The specified section.
        /// </returns>
        public override IEnumerable<double> GetWindowedSamples(int start, int length)
        {
            if (this.CacheStart == this.CacheEnd)
            {
                this.Cache = this.GenerateNoise(length);
                this.CacheStart = start;
                this.CacheEnd = start + length;
            }
            else
            {
                if (start < this.CacheStart)
                {
                    this.Cache.InsertRange(0, this.GenerateNoise(this.CacheStart - start));
                    this.CacheStart = start;
                }

                if (length + start > this.CacheEnd)
                {
                    this.Cache.AddRange(this.GenerateNoise(length + start - this.CacheEnd));
                    this.CacheEnd = length + start;
                }
            }

            return this.Cache.GetRangeOptimized(start - this.CacheStart, length);
        }

        private List<double> GenerateNoise(int length)
        {
            var ret = new List<double>(length);
            for (var i = 0; i < length; i++)
            {
                this.NoiseSource.MoveNext();
                ret.Add(this.NoiseSource.Current * this.Sigma + this.Mean);
            }

            return ret;
        }

        [Category("white noise")]
        [DisplayName("mean")]
        public double Mean { get; }

        [DisplayName("variance")]
        public double Variance { get; }
    }
}