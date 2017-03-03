// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SeriesBase.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp.Algorithms;

namespace DspSharp.Series
{
    public abstract class SeriesBase : ISeries
    {
        protected SeriesBase(IEnumerable<double> source, bool logarithmic)
        {
            this.Values = source.ToReadOnlyList();
            this.IsLogarithmic = logarithmic;
        }

        public abstract bool Equals(ISeries other);

        public bool IsLogarithmic { get; }

        public abstract int Length { get; }

        public IReadOnlyList<double> Values { get; }
    }
}