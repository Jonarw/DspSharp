// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FiniteFilterSet.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.Specialized;
using DspSharp.Utilities.Collections;

namespace DspSharp.Filter
{
    /// <summary>
    ///     Represents a set of finite filters which are applied in succession.
    /// </summary>
    /// <seealso cref="FiniteFilter" />
    public class FiniteFilterSet : FiniteFilter
    {
        private readonly IObservableList<IFiniteFilter> _Filters = new ObservableList<IFiniteFilter>();

        public FiniteFilterSet(double samplerate) : base(samplerate)
        {
            this._Filters.CollectionChanged += this.FilterCollectionChanged;
        }

        /// <summary>
        ///     Gets the contained filters.
        /// </summary>
        public IList<IFiniteFilter> Filters => this._Filters;

        /// <summary>
        ///     Specifies whether the filter object has an effect or not.
        /// </summary>
        protected override bool HasEffectOverride => this.Filters.Count > 0;

        /// <summary>
        ///     Processes the specified sequence.
        /// </summary>
        /// <param name="input">The sequence.</param>
        /// <returns></returns>
        public override IEnumerable<double> ProcessOverride(IEnumerable<double> input)
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