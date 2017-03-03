// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstantSeries.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Algorithms;

namespace DspSharp.Series
{
    /// <summary>
    ///     Special kind of series that only has one point. Used for constant frequency
    ///     responses.
    /// </summary>
    public class ConstantSeries : SeriesBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstantSeries" /> class.
        /// </summary>
        /// <param name="frequency">The frequency at which the constant value should be evaluated.</param>
        public ConstantSeries(double frequency = 1000) : base(frequency.ToEnumerable(), false)
        {
            this.Frequency = frequency;
        }

        /// <summary>
        ///     Gets the frequency at which the constant value should be evaluated.
        /// </summary>
        public double Frequency { get; }

        public override int Length => 1;

        /// <summary>
        ///     Compares the <see cref="ConstantSeries" /> to an other <see cref="ISeries" /> for equality.
        /// </summary>
        /// <param name="other">The other <see cref="ISeries" />.</param>
        /// <returns>True if the other object is a not null and a <see cref="ConstantSeries" />, false otherwise.</returns>
        public override bool Equals(ISeries other)
        {
            if (other == null)
                return false;
            return this.Equals(other as ConstantSeries);
        }

        /// <summary>
        ///     Compares the <see cref="ConstantSeries" /> to an other <see cref="ConstantSeries" />.
        /// </summary>
        /// <param name="other">The other <see cref="ConstantSeries" />.</param>
        /// <returns>True if the other <see cref="ConstantSeries" /> is not null, false otherwise.</returns>
        public bool Equals(ConstantSeries other)
        {
            if (other == null)
                return false;

            if (other.Frequency != this.Frequency)
                return false;

            return true;
        }

        /// <summary>
        ///     Computes the Hashcode for the <see cref="ConstantSeries" />.
        /// </summary>
        /// <returns>The Hashcode.</returns>
        public override int GetHashCode()
        {
            return this.Frequency.GetHashCode();
        }
    }
}