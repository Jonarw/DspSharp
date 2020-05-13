using System.Windows.Input;
using DspSharp.Algorithms;
using DspSharp.Signal;
using UmtUtilities;
using UmtUtilities.ViewModel;

namespace DspSharpDemo.ViewModel
{
    public class AutoBiquadViewModel : ViewModelBase
    {
        private ICommand _ExecuteCommand;

        public AutoBiquadViewModel(DspSharpDemoViewModel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public AutoBiquad AutoBiquad { get; } = new AutoBiquad();

        public ICommand ExecuteCommand => this._ExecuteCommand ?? (this._ExecuteCommand = new RelayCommand(param => this.Execute(), o => true));
        public DspSharpDemoViewModel ViewModel { get; }

        public void Execute()
        {
            var target = (IFiniteSignal)this.ViewModel.SignalsViewModel.Items[0];
            this.AutoBiquad.SampleRate = target.SampleRate;
            this.AutoBiquad.SetTarget(target.Spectrum.Frequencies.Values, FrequencyDomain.LinearToDb(target.Spectrum.Magnitude).ToReadOnlyList());
            var original = (IFiniteSignal)this.ViewModel.SignalsViewModel.Items[1];
            var filters = this.AutoBiquad.MakeFilters(original.Spectrum.Frequencies.Values, FrequencyDomain.LinearToDb(original.Spectrum.Magnitude).ToReadOnlyList());

            this.ViewModel.FiltersViewModel.Items.AddRange(filters);
        }
    }
}