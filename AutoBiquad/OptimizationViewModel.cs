using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DspSharp.Algorithms;
using DspSharp.Filter.LtiFilters.Iir;
using UmtUtilities;
using UmtUtilities.ViewModel;

namespace AutoBiquad
{
    public class OptimizationViewModel : ViewModelBase
    {
        private ICommand _ExecuteCommand;
        private double _GradientFactor = .0001;
        private int _Iterations = 100;
        private bool _RollingUpdate = true;
        private double _StepSize = 0.1;

        public OptimizationViewModel(ViewModel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public ICommand ExecuteCommand => this._ExecuteCommand ??
                                          (this._ExecuteCommand = new RelayCommand(param => this.Execute(), o => this.ViewModel.Filters.Count > 0));

        public double GradientFactor
        {
            get { return this._GradientFactor; }
            set { this.SetField(ref this._GradientFactor, value); }
        }

        public int Iterations
        {
            get { return this._Iterations; }
            set { this.SetField(ref this._Iterations, value); }
        }

        public bool RollingUpdate
        {
            get { return this._RollingUpdate; }
            set { this.SetField(ref this._RollingUpdate, value); }
        }

        public double StepSize
        {
            get { return this._StepSize; }
            set { this.SetField(ref this._StepSize, value); }
        }

        public ViewModel ViewModel { get; }

        public void Execute()
        {
            var x = this.ViewModel.AutoBiquad.GetFrequencies();

            var yOriginal = Interpolation.AdaptiveInterpolation(
                this.ViewModel.OriginalGraphViewModel.X,
                this.ViewModel.OriginalGraphViewModel.Y,
                x,
                true,
                false).ToReadOnlyList();

            var yTarget = Interpolation.AdaptiveInterpolation(
                this.ViewModel.TargetGraphViewModel.X,
                this.ViewModel.TargetGraphViewModel.Y,
                x,
                true,
                false).ToReadOnlyList();

            var biquads = this.ViewModel.Filters.OfType<BiquadFilter>().ToReadOnlyList();
            var parameters = new List<double>(biquads.Count * 3);
            var localBiquads = new List<BiquadFilter>(biquads.Count);
            var gainfilter = this.ViewModel.Filters[0];

            foreach (var filter in biquads)
            {
                parameters.Add(Math.Log(filter.Fc));
                parameters.Add(10 * Math.Log(filter.Q));
                parameters.Add(filter.Gain);
                localBiquads.Add(filter);
            }

            double ErrorFunc(IReadOnlyList<double> list)
            {
                for (int i = 0; i < localBiquads.Count; i++)
                {
                    localBiquads[i].Fc = Math.Exp(list[3 * i]);
                    localBiquads[i].Q = Math.Exp(list[3 * i + 1] / 10);
                    localBiquads[i].Gain = list[3 * i + 2];
                }

                var yfiltered = ViewModel.ApplyFilters(
                    x,
                    yOriginal,
                    localBiquads.Prepend(gainfilter).ToReadOnlyList());

                var dif = yfiltered.Subtract(yTarget);
                return dif.Rms();
            }

            AutoBiquadModel.Nlms(parameters, ErrorFunc, this.Iterations, this.GradientFactor, this.StepSize, this.RollingUpdate);
            this.ViewModel.Filters.Reset(biquads.Prepend(this.ViewModel.Filters[0]));
        }
    }
}