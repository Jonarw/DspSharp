using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Filter;
using Filter.Algorithms;
using Filter.Algorithms.FftwProvider;
using Filter.Collections;
using Filter.Extensions;
using Filter.LtiFilters;
using Filter.Signal;
using FilterPlot;
using FilterTest.SignalFactory;

namespace FilterTest
{
    public class ViewModel : Observable
    {
        private ICommand _AddFilterCommand;
        private ICommand _AddSignalCommand;
        private ICommand _RemoveFilterCommand;
        private ICommand _RemoveSignalCommand;

        private double _Samplerate;
        private IFilter _SelectedFilter;
        private FilterTypes _SelectedFilterType;
        private SignalPlot _SelectedPlot;
        private ISignal _SelectedSignal;

        public ViewModel()
        {
            this.ImpulseResponsePlot = new ImpulseResponsePlot();
            this.MagnitudePlot = new MagnitudePlot();
            this.PhasePlot = new PhasePlot();

            this.Plots = new List<SignalPlot>
            {
                this.ImpulseResponsePlot,
                this.MagnitudePlot,
                this.PhasePlot
            };

            this.UpdateRanges();
            this.Plots[0].PropertyChanged += this.ImpulseResponsePlotChanged;

            Fft.FftProvider = new FftwProvider();

            this.SelectedSignals.CollectionChanged += this.SelectedSignalsChanged;
            this.SelectedFilters.CollectionChanged += this.SelectedFiltersChanged;

            this.Samplerate = 44100;
        }

        public ICommand AddFilterCommand => this._AddFilterCommand ?? (this._AddFilterCommand = new RelayCommand(param => this.AddFilter()));
        public ICommand AddSignalCommand => this._AddSignalCommand ?? (this._AddSignalCommand = new RelayCommand(param => this.AddSignal()));
        public IEnumerable<double> AvailableSamplerates { get; } = FilterBase.DefaultSampleRates;
        public ObservableCollection<IFilter> Filters { get; } = new ObservableCollection<IFilter>();
        public ImpulseResponsePlot ImpulseResponsePlot { get; set; }
        public MagnitudePlot MagnitudePlot { get; set; }
        public PhasePlot PhasePlot { get; set; }
        public IList<SignalPlot> Plots { get; }

        public ICommand RemoveFilterCommand
            => this._RemoveFilterCommand ?? (this._RemoveFilterCommand = new RelayCommand(param => this.RemoveFilter()));

        public ICommand RemoveSignalCommand
            => this._RemoveSignalCommand ?? (this._RemoveSignalCommand = new RelayCommand(param => this.RemoveSignal()));

        public double Samplerate
        {
            get { return this._Samplerate; }
            set
            {
                this.SetField(ref this._Samplerate, value);
                this.Signals.Clear();
                this.Filters.Clear();
                this.Signals.Add(new Dirac(this.Samplerate));
                this.Filters.Add(new DiracFilter(this.Samplerate));
            }
        }

        public IFilter SelectedFilter
        {
            get { return this._SelectedFilter; }
            set
            {
                if (this.SelectedFilter != null)
                {
                    this.SelectedFilter.PropertyChanged -= this.SelectedFilterChanged;
                }

                this.SetField(ref this._SelectedFilter, value);

                if (this.SelectedFilter != null)
                {
                    this.SelectedFilter.PropertyChanged += this.SelectedFilterChanged;
                }
            }
        }

        public ObservableCollection<IFilter> SelectedFilters { get; } = new ObservableCollection<IFilter>();

        public FilterTypes SelectedFilterType
        {
            get { return this._SelectedFilterType; }
            set { this.SetField(ref this._SelectedFilterType, value); }
        }

        public SignalPlot SelectedPlot
        {
            get { return this._SelectedPlot; }
            set
            {
                if ((value != null) && (this.SelectedPlot != null))
                {
                    value.Signals.Clear();
                    foreach (var signal in this.SelectedPlot.Signals)
                    {
                        value.Signals.Add(signal);
                    }
                }

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

        private void AddFilter()
        {
            this.Filters.Add(FilterFactory.CreateFilter(this.SelectedFilterType, this.Samplerate, this.Signals.ToReadOnlyObservableList()));
        }

        private void AddSignal()
        {
            var dia = new SignalDialog(this.Samplerate);
            var result = dia.ShowDialog();
            if (result == true)
            {
                if (dia.CreatedSignal != null)
                {
                    this.Signals.Add(dia.CreatedSignal);
                }
            }
        }

        private void ImpulseResponsePlotChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "XMin") || (e.PropertyName == "XMax"))
            {
                this.UpdateRanges();
            }
        }

        private void RemoveFilter()
        {
            if (this.SelectedFilter != null)
            {
                this.Filters.Remove(this.SelectedFilter);
            }
        }

        private void RemoveSignal()
        {
            if (this.SelectedSignal != null)
            {
                this.Signals.Remove(this.SelectedSignal);
            }
        }

        private void SelectedFilterChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateSelectedPlot();
        }

        private void SelectedFiltersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateSelectedPlot();
        }

        private void SelectedSignalsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateSelectedPlot();
        }

        private void UpdateRanges()
        {
            this.MagnitudePlot.ViewStart = this.PhasePlot.ViewStart = this.ImpulseResponsePlot.XMin;
            this.MagnitudePlot.ViewLength = this.PhasePlot.ViewLength = this.ImpulseResponsePlot.XMax - this.ImpulseResponsePlot.XMin;
        }

        private void UpdateSelectedPlot()
        {
            if (this.SelectedPlot == null)
            {
                return;
            }

            this.SelectedPlot.Signals.Clear();

            foreach (var signal in this.SelectedSignals)
            {
                var esignal = signal as IEnumerableSignal;
                if (esignal != null)
                {
                    var filteredsignal = esignal;
                    foreach (var filter in this.SelectedFilters)
                    {
                        filteredsignal = filteredsignal.Process(filter);
                    }

                    if (filteredsignal != signal)
                    {
                        filteredsignal.DisplayName = signal.DisplayName + " (filtered)";
                    }

                    this.SelectedPlot.Signals.Add(filteredsignal);
                }
                else
                {
                    this.SelectedPlot.Signals.Add(signal);
                }
            }

            this.SelectedPlot.Update(true);
        }
    }
}