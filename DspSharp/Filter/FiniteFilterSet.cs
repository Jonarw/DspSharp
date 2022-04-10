// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FiniteFilterSet.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace DspSharp.Filter
{
    /// <summary>
    /// Represents a set of finite filters which are applied in succession.
    /// </summary>
    public class FiniteFilterSet : FiniteFilter
    {
        private readonly ObservableCollection<IFiniteFilter> _Filters = new ObservableCollection<IFiniteFilter>();

        public FiniteFilterSet(double samplerate) : base(samplerate)
        {
            this._Filters.CollectionChanged += this.FilterCollectionChanged;
        }

        /// <summary>
        /// Gets the contained filters.
        /// </summary>
        public IList<IFiniteFilter> Filters => this._Filters;

        /// <inheritdoc/>
        protected override bool HasEffectOverride => this.Filters.Any(f => f.HasEffect);

        /// <inheritdoc/>
        protected override IEnumerable<double> ProcessOverride(IEnumerable<double> input)
        {
            foreach (var filter in this.Filters)
            {
                input = filter.Process(input);
            }

            return input;
        }

        private void FilterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaiseChangedEvent();
        }
    }
}