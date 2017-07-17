// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterSet.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DspSharp.Utilities.Collections;

namespace DspSharp.Filter
{
    /// <summary>
    ///     Represents a set of filters which are applied in succession.
    /// </summary>
    /// <seealso cref="FilterBase" />
    public class FilterSet : FilterBase
    {
        private readonly IObservableList<IFilter> _Filters = new ObservableList<IFilter>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="FilterSet" /> class.
        /// </summary>
        /// <param name="samplerate">The samplerate.</param>
        public FilterSet(double samplerate) : base(samplerate)
        {
            this._Filters.CollectionChanged += this.FilterCollectionChanged;
        }

        /// <summary>
        ///     Gets the contained filters.
        /// </summary>
        public IList<IFilter> Filters => this._Filters;

        /// <summary>
        ///     Gets a value indicating whether this instance has infinite impulse response.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has an infinite impulse response; otherwise, <c>false</c>.
        /// </value>
        public override bool HasInfiniteImpulseResponse => this.Filters.Any(f => f.HasInfiniteImpulseResponse);

        protected override bool HasEffectOverride => this.Filters.Count > 0;

        /// <summary>
        ///     Processes the specified signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>
        ///     The processed signal.
        /// </returns>
        public override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
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