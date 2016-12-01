using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Filter.Extensions;
using Filter.Signal;
using PropertyTools.DataAnnotations;

namespace Filter.LtiFilters
{
    public class CustomConvolver : Convolver, ISignalBasedFilter
    {
        private ObservableCollection<ISignal> _availableSignals = new ObservableCollection<ISignal>();
        private IFiniteSignal _Source;

        public CustomConvolver(double samplerate) : base(samplerate)
        {
        }

        public SmartCollection<IFiniteSignal> AvailableFiniteSignals { get; } = new SmartCollection<IFiniteSignal>();

        public override IReadOnlyList<double> ImpulseResponse
        {
            get
            {
                if (this.Source != null)
                {
                    return this.Source.Signal;
                }

                return 1.0.ToEnumerable().ToReadOnlyList();
            }
        }

        protected override bool HasEffectOverride
        {
            get { return this.Source != null; }
        }

        public ObservableCollection<ISignal> AvailableSignals
        {
            get { return this._availableSignals; }
            set
            {
                if (this.AvailableSignals != null)
                {
                    this.AvailableSignals.CollectionChanged -= this.AvailableSignalsChanged;
                }

                this._availableSignals = value;

                if (this.AvailableSignals != null)
                {
                    this.AvailableSignals.CollectionChanged += this.AvailableSignalsChanged;
                }
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
            {
                this.AvailableFiniteSignals.Reset(this.AvailableSignals.OfType<FiniteSignal>());
            }
        }

        [DisplayName("source impulse response")]
        [ItemsSourceProperty(nameof(AvailableFiniteSignals))]
        [DisplayMemberPath(nameof(IFiniteSignal.DisplayName))]
        public IFiniteSignal Source
        {
            get { return this._Source; }
            set { this.SetField(ref this._Source, value); }
        }
    }
}