// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DspSharp.Algorithms;
using DspSharp.Extensions;
using DspSharp.Filter;
using DspSharp.Signal;
using DspSharpFftw;
using DspSharpPlot;
using UmtUtilities.ViewModel;
using UTilities.Observable.DataAnnotations;

namespace DspSharpDemo.ViewModel
{
    public class DspSharpDemoViewModel : ViewModelBase
    {
        private SignalPlot _SelectedPlot;

        public DspSharpDemoViewModel()
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

            this.FiltersViewModel = new FiltersViewModel(this);
            this.SignalsViewModel = new SignalsViewModel(this);
            this.AutoBiquadViewModel = new AutoBiquadViewModel(this);
            this.ForcePropertyUpdate(nameof(this.FiltersViewModel));
            this.ForcePropertyUpdate(nameof(this.SignalsViewModel));
            this.ForcePropertyUpdate(nameof(this.AutoBiquadViewModel));
        }

        public IEnumerable<double> AvailableSamplerates { get; } = FilterBase.DefaultSampleRates;

        [Observed]
        public FiltersViewModel FiltersViewModel { get; }
        [Observed]
        public AutoBiquadViewModel AutoBiquadViewModel { get; }
        public ImpulseResponsePlot ImpulseResponsePlot { get; set; }
        public MagnitudePlot MagnitudePlot { get; set; }
        public PhasePlot PhasePlot { get; set; }
        public IReadOnlyList<SignalPlot> Plots { get; }

        public SignalPlot SelectedPlot
        {
            get { return this._SelectedPlot; }
            set
            {
                if (value != null && this.SelectedPlot != null)
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

        [Observed]
        public SignalsViewModel SignalsViewModel { get; }

        private void ImpulseResponsePlotChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "XMin" || e.PropertyName == "XMax")
                this.UpdateRanges();
        }

        private void UpdateRanges()
        {
            this.MagnitudePlot.ViewStart = this.PhasePlot.ViewStart = (int)this.ImpulseResponsePlot.XMin;
            this.MagnitudePlot.ViewLength = this.PhasePlot.ViewLength = (int)(this.ImpulseResponsePlot.XMax - this.ImpulseResponsePlot.XMin);
        }

        protected override void OnDeepPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == this.FiltersViewModel.SelectedFilters || sender == this.SignalsViewModel.SelectedSignals || sender is IFilter)
                this.UpdateSelectedPlot();

            base.OnDeepPropertyChanged(sender, e);
        }

        private void UpdateSelectedPlot()
        {
            if (this.SelectedPlot == null)
                return;

            this.SelectedPlot.Signals.Clear();

            foreach (var signal in this.SignalsViewModel.SelectedSignals)
            {
                if (signal is IEnumerableSignal esignal)
                {
                    var filteredsignal = esignal;
                    foreach (var filter in this.FiltersViewModel.SelectedFilters.Where(f => f.Samplerate == signal.SampleRate))
                    {
                        filteredsignal = filteredsignal.Process(filter);
                    }

                    if (filteredsignal != signal)
                        filteredsignal.DisplayName = signal.DisplayName + " (filtered)";

                    this.SelectedPlot.Signals.Add(filteredsignal);
                }
                else
                    this.SelectedPlot.Signals.Add(signal);
            }

            this.SelectedPlot.Update(true);
        }
    }
}