using System.Collections.Generic;

namespace Filter.Series
{
    public abstract class SeriesBase : ISeries
    {
        protected SeriesBase(IEnumerable<double> source, bool logarithmic)
        {
            this.Values = source;
            this.IsLogarithmic = logarithmic;
        }

        public abstract bool Equals(ISeries other);

        public bool IsLogarithmic { get; }

        public abstract int Length { get; }

        public IEnumerable<double> Values { get; }
    }
}