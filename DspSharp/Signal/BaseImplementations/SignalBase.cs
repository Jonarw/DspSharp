// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignalBase.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp.Algorithms;
using UTilities.Observable;
using UTilities.Extensions;

namespace DspSharp.Signal
{
    /// <summary>
    ///     Base class for all signals.
    /// </summary>
    /// <seealso cref="ISignal" />
    public abstract class SignalBase : ISignal
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SignalBase" /> class.
        /// </summary>
        /// <param name="sampleRate">The sample rate.</param>
        protected SignalBase(double sampleRate)
        {
            this.SampleRate = sampleRate;
        }

        public string DisplayName { get; set; }

        /// <summary>
        ///     Gets a section of the signal in time domain.
        /// </summary>
        /// <param name="start">The start of the section.</param>
        /// <param name="length">The length of the section.</param>
        /// <returns>
        ///     The specified section.
        /// </returns>
        public abstract IEnumerable<double> GetWindowedSamples(int start, int length);

        public IFiniteSignal GetWindowedSignal(int start, int length)
        {
            return new FiniteSignal(this.GetWindowedSamples(start, length).ToReadOnlyList(), this.SampleRate);
        }

        public double SampleRate { get; }
    }
}