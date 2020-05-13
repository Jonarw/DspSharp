using System;
using DspSharp.Filter;
using DspSharp.Filter.LtiFilters.Fir;
using DspSharp.Filter.LtiFilters.Iir;
using DspSharp.Filter.LtiFilters.Primitive;
using DspSharp.Filter.NonlinearFilters;
using UmtUtilities.CollectionViewModel;
using UmtUtilities.DialogProvider;
using UmtUtilities.Extensions;
using UmtUtilities.ViewModel;
using UTilities.Collections;
using UTilities.Factory;
using UTilities.Observable.DataAnnotations;

namespace DspSharpDemo.ViewModel
{
    public class FiltersViewModel : CollectionViewModelVm<IFilter>
    {
        public FiltersViewModel(DspSharpDemoViewModel viewModel, IDialogProvider dialogProvider = null) : base(
            new ObservableList<IFilter>(),
            new FilterFactory(), 
            new FuncFactory<IViewModel<IFilter>, IFilter>(filter => new FilterViewModel(filter)),
            dialogProvider)
        {
            this.ViewModel = viewModel;
        }

        [ObservedCollection]
        public IObservableList<IFilter> SelectedFilters { get; } = new ObservableList<IFilter>();

        public FilterFactory FilterFactory => (FilterFactory)this.ItemFactory;

        public DspSharpDemoViewModel ViewModel { get; }
    }

    public class FilterFactory : Factory<IFilter, FilterType>
    {
        private double _SampleRate = 48000;

        public double SampleRate
        {
            get { return this._SampleRate; }
            set { this.SetField(ref this._SampleRate, value); }
        }

        protected override IFilter CreateItemOverride(FilterType parameter)
        {
            switch (parameter)
            {
            case FilterType.Distortion:
                return new DistortionFilter(this.SampleRate);
            case FilterType.Biquad:
                return new BiquadFilter(this.SampleRate);
            case FilterType.Correcting:
                return new CorrectingFilter(this.SampleRate);
            case FilterType.Delay:
                return new DelayFilter(this.SampleRate);
            case FilterType.Dirac:
                return new DiracFilter(this.SampleRate);
            case FilterType.Fir:
                return new FirFilter(this.SampleRate);
            case FilterType.Gain:
                return new GainFilter(this.SampleRate);
            case FilterType.Iir:
                return new IirFilter(this.SampleRate);
            case FilterType.Invert:
                return new InvertFilter(this.SampleRate);
            case FilterType.Zero:
                return new ZeroFilter(this.SampleRate);
            case FilterType.Butterworth:
                return new ButterworthFilter(this.SampleRate);
            case FilterType.CustomConvolver:
            default:
                throw new ArgumentOutOfRangeException(nameof(parameter), parameter, null);
            }
        }

        protected override void Initialize(IFilter item, FilterType parameter)
        {
            item.DisplayName = parameter.GetDescription();
        }
    }

    public class FilterViewModel : ViewModelBase<IFilter>
    {
        public FilterViewModel(IFilter model) : base(model)
        {
        }
    }
}