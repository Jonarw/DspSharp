// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Dirac.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Algorithms;
using PropertyTools.DataAnnotations;

namespace DspSharp.Signal
{
    /// <summary>
    ///     Represents a scaled dirac pulse.
    /// </summary>
    public class Dirac : FiniteSignal
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Dirac" /> class.
        /// </summary>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="gain">The gain.</param>
        public Dirac(double sampleRate, double gain = 1) : base(new[] {gain}.ToReadOnlyList(), sampleRate)
        {
            this.Gain = gain;
            this.DisplayName = "dirac, gain = " + gain;
        }

        [Category("dirac")]
        [DisplayName("gain")]
        public double Gain { get; }
    }
}