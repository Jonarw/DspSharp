// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomSeries.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using DspSharp.Utilities;

namespace DspSharp.Series
{
    /// <summary>
    ///     Userdefined series based on a custom set of values.
    /// </summary>
    public class CustomSeries : SeriesBase
    {
        private int lengthcache = -1;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomSeries" /> class.
        /// </summary>
        /// <param name="values">The values defining the series.</param>
        /// <param name="logarithmic">
        ///     Can be set True for indicating that the <paramref name="values" /> are on a logarithmic scale. Does not have any
        ///     internal implications,
        ///     but may change the behaviour of other function operating with the series.
        /// </param>
        public CustomSeries(IEnumerable<double> values, bool logarithmic = false) : base(values, logarithmic)
        {
        }

        public override int Length
        {
            get
            {
                if (this.lengthcache < 0)
                    this.lengthcache = this.Values.Count();
                return this.lengthcache;
            }
        }

        /// <summary>
        ///     Compares the <see cref="CustomSeries" /> to an other <see cref="ISeries" /> for equality.
        /// </summary>
        /// <param name="other">The other <see cref="ISeries" />.</param>
        /// <returns>True if the other object is a <see cref="CustomSeries" /> with the same values, false otherwise.</returns>
        public override bool Equals(ISeries other)
        {
            if (other == null)
                return false;
            return this.Equals(other as CustomSeries);
        }

        /// <summary>
        ///     Compares the <see cref="CustomSeries" /> to an other <see cref="CustomSeries" /> for equality.
        /// </summary>
        /// <param name="other">The other <see cref="CustomSeries" />.</param>
        /// <returns>True if the other <see cref="CustomSeries" /> has the same values, false otherwise.</returns>
        public bool Equals(CustomSeries other)
        {
            if (other == null)
                return false;
            if (other == this)
                return true;

            return other.Values.SequenceEqual(this.Values);
        }

        /// <summary>
        ///     Computes the Hashcode for the <see cref="CustomSeries" />.
        /// </summary>
        /// <returns>The Hashcode.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.Values);
        }
    }
}