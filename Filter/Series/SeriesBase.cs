using System.Collections;
using System.Collections.Generic;
using Filter.Extensions;

namespace Filter.Series
{
    public abstract class SeriesBase : ISeries
    {
        protected SeriesBase(IEnumerable<double> source, bool logarithmic)
        {
            this.Values = source;
            this.IsLogarithmic = logarithmic;
        }

        public bool IsLogarithmic { get; }

        public IEnumerable<double> Values { get; }

        public abstract int Length { get; }
        public abstract bool Equals(ISeries other);
    }
}