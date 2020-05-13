// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomConvolver.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DspSharp.Algorithms;
using DspSharp.Signal;
using UTilities.Collections;
using UTilities.Extensions;

namespace DspSharp.Filter.LtiFilters.Fir
{
    public class CustomConvolver : Convolver, ISignalBasedFilter
    {
        private IReadOnlyObservableList<ISignal> _availableSignals;
        private IFiniteSignal _Source;

        public CustomConvolver(double samplerate) : base(samplerate)
        {
        }

        public IObservableList<IFiniteSignal> AvailableFiniteSignals { get; } = new ObservableList<IFiniteSignal>();

        public override IReadOnlyList<double> ImpulseResponse
        {
            get
            {
                if (this.Source != null)
                    return this.Source.Signal;

                return 1.0.ToEnumerable().ToReadOnlyList();
            }
        }

        public IFiniteSignal Source
        {
            get { return this._Source; }
            set { this.SetField(ref this._Source, value); }
        }

        protected override bool HasEffectOverride
        {
            get { return this.Source != null; }
        }

        public IReadOnlyObservableList<ISignal> AvailableSignals
        {
            get { return this._availableSignals; }
            set
            {
                if (this.AvailableSignals != null)
                    this.AvailableSignals.CollectionChanged -= this.AvailableSignalsChanged;

                this._availableSignals = value;

                if (this.AvailableSignals != null)
                    this.AvailableSignals.CollectionChanged += this.AvailableSignalsChanged;
                this.UpdateFiniteSignals();
            }
        }

        private void AvailableSignalsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateFiniteSignals();
        }

        private void UpdateFiniteSignals()
        {
            if (this.AvailableSignals != null)
                this.AvailableFiniteSignals.Reset(this.AvailableSignals.OfType<FiniteSignal>());
        }
    }
}