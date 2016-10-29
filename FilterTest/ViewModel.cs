using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Filter;
using Filter.Algorithms;
using Filter.Signal;
using FilterPlot;
using FilterTest.SignalFactory;
using Filter_Win;

namespace FilterTest
{
    public class ViewModel : Observable
    {
        private ICommand _AddSignalCommand;
        private IFilter _SelectedFilter;
        private SignalPlot _SelectedPlot;
        private ISignal _SelectedSignal;

        public ViewModel()
        {
            this.Plots = new List<SignalPlot>
            {
                new ImpulseResponsePlot {Signals = this.SelectedSignals},
                new MagnitudePlot {Signals = this.SelectedSignals},
                new PhasePlot {Signals = this.SelectedSignals}
            };

            Fft.FftProvider = new FftwProvider();

            this.SelectedSignals.CollectionChanged += this.SelectedSignalsChanged;
        }

        /// <summary>
        ///     Gets the close command.
        /// </summary>
        public ICommand AddSignalCommand => this._AddSignalCommand ?? (this._AddSignalCommand = new RelayCommand(param => this.AddSignal()));

        public ObservableCollection<IFilter> Filters { get; } = new ObservableCollection<IFilter>();

        public IList<SignalPlot> Plots { get; }

        public IFilter SelectedFilter
        {
            get { return this._SelectedFilter; }
            set { this.SetField(ref this._SelectedFilter, value); }
        }

        public SignalPlot SelectedPlot
        {
            get { return this._SelectedPlot; }
            set
            {
                this.SetField(ref this._SelectedPlot, value);
                this.SelectedPlot?.Update(true);
            }
        }

        public ISignal SelectedSignal
        {
            get { return this._SelectedSignal; }
            set { this.SetField(ref this._SelectedSignal, value); }
        }

        public ObservableCollection<ISignal> SelectedSignals { get; } = new ObservableCollection<ISignal>();

        public ObservableCollection<ISignal> Signals { get; } = new ObservableCollection<ISignal>();

        private void AddSignal()
        {
            var dia = new SignalDialog();
            var result = dia.ShowDialog();
            if (result == true)
            {
                this.Signals.Add(dia.CreatedSignal);
            }
        }

        private void SelectedSignalsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.SelectedPlot?.Update(true);
        }
    }
}