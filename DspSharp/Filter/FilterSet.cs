// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterSet.cs">
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
    /// Represents a set of filters which are applied in succession.
    /// </summary>
    /// <seealso cref="FilterBase" />
    public class FilterSet : FilterBase
    {
        private readonly ObservableCollection<IFilter> _Filters = new ObservableCollection<IFilter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterSet" /> class.
        /// </summary>
        /// <param name="samplerate">The samplerate.</param>
        public FilterSet(double samplerate) : base(samplerate)
        {
            this._Filters.CollectionChanged += this.FilterCollectionChanged;
        }

        /// <summary>
        /// Gets the contained filters.
        /// </summary>
        public IList<IFilter> Filters => this._Filters;

        /// <inheritdoc/>
        public override bool HasInfiniteImpulseResponse => this.Filters.Any(f => f.HasInfiniteImpulseResponse);

        /// <inheritdoc/>
        protected override bool HasEffectOverride => this.Filters.Any(f => f.HasEffect);

        /// <inheritdoc/>
        protected override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            foreach (var filter in this.Filters)
            {
                signal = filter.Process(signal);
            }

            return signal;
        }

        private void FilterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaiseChangedEvent();
        }
    }
}