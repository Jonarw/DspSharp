// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfiniteSignal.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace DspSharp.Signal
{
    /// <summary>
    ///     Represents a digital signal that is infinitely long in time domain.
    /// </summary>
    /// <seealso cref="SignalBase" />
    public class InfiniteSignal : SignalBase
    {
        /// <summary>
        ///     Describes a function that returns a sample value for any given sample number.
        /// </summary>
        /// <param name="time">The sample number.</param>
        /// <returns></returns>
        public delegate double TimeDomainFunc(int time);

        /// <summary>
        ///     Describes a function that returns a range of samples values for any given sample range.
        /// </summary>
        /// <param name="start">The start of the sample range.</param>
        /// <param name="length">The length of the sample range.</param>
        /// <returns></returns>
        public delegate IEnumerable<double> TimeDomainRangeFunc(int start, int length);

        /// <summary>
        ///     Initializes a new instance of the <see cref="InfiniteSignal" /> class.
        /// </summary>
        /// <param name="sampleFunction">The sample function.</param>
        /// <param name="sampleRate">The sample rate.</param>
        public InfiniteSignal(TimeDomainFunc sampleFunction, double sampleRate) : base(sampleRate)
        {
            this.SampleFunction = sampleFunction;
            this.TimeDomainFunction = this.GetTimeDomainFunction;
            this.DisplayName = "infinite signal";
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InfiniteSignal" /> class.
        /// </summary>
        /// <param name="timeDomainFunction">The time domain range function.</param>
        /// <param name="sampleRate">The sample rate.</param>
        public InfiniteSignal(TimeDomainRangeFunc timeDomainFunction, double sampleRate) : base(sampleRate)
        {
            this.TimeDomainFunction = timeDomainFunction;
            this.DisplayName = "infinite signal";
        }

        private TimeDomainFunc SampleFunction { get; }

        private TimeDomainRangeFunc TimeDomainFunction { get; }

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
            return this.TimeDomainFunction.Invoke(start, length);
        }

        private IEnumerable<double> GetTimeDomainFunction(int start, int length)
        {
            for (var i = start; i < start + length; i++)
            {
                yield return this.SampleFunction.Invoke(i);
            }
        }
    }
}