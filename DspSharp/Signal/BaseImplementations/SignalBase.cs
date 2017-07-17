// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignalBase.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp.Algorithms;
using DspSharp.Utilities;
using PropertyTools.DataAnnotations;

namespace DspSharp.Signal
{
    /// <summary>
    ///     Base class for all signals.
    /// </summary>
    /// <seealso cref="Observable" />
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

        [Category("general")]
        [DisplayName("display name")]
        public string DisplayName { get; set; }

        [DisplayName("sample rate")]
        public double SampleRate { get; }
    }
}