using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DspSharp.Algorithms;
using DspSharp.Filter;
using DspSharp.Filter.LtiFilters.Iir;
using DspSharp.Filter.LtiFilters.Primitive;
using UmtUtilities.Collections;
using UmtUtilities.CollectionViewModel;
using UmtUtilities.Factory;
using UmtUtilities.ViewModel;

namespace AutoBiquad
{
    public class FiltersViewModel : CollectionViewModelVm<IFilter, FilterViewModel>
    {
        public FiltersViewModel(ViewModel viewModel) : base(viewModel.Filters, new FilterFactory(viewModel), new FilterViewModelFactory(viewModel))
        {
            this.ViewModel = viewModel;
        }

        public ViewModel ViewModel { get; }

        protected override void ObservedCollectionOnItemPropertyChanged(INotifyItemPropertyChanged sender, ItemPropertyChangedEventArgs e)
        {
            if (sender == this.Items)
            {
                this.ViewModel.UpdateFilteredGraphViewModel();
                this.OnSelectedItemChanged();
            }

            base.ObservedCollectionOnItemPropertyChanged(sender, e);
        }

        protected override void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.ViewModel.UpdateFilteredGraphViewModel();
            base.OnCollectionChanged(sender, notifyCollectionChangedEventArgs);
        }

        protected override void OnSelectedItemChanged()
        {
            if (this.SelectedItemViewModel != null)
                this.ViewModel.OxyModel.Series.Remove(this.SelectedItemViewModel.GraphViewModel.Model);
            base.OnSelectedItemChanged();
            if (this.SelectedItemViewModel != null)
                this.ViewModel.OxyModel.Series.Add(this.SelectedItemViewModel.GraphViewModel.Model);

            this.ViewModel.UpdatePlot();
        }

    }

    public class FilterFactory : Factory<IFilter>
    {
        public ViewModel ViewModel { get; }

        public FilterFactory(ViewModel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public override IFilter CreateItem()
        {
            return new BiquadFilter(this.ViewModel.AutoBiquad.SampleRate, BiquadFilter.BiquadFilters.Peaking, 1000, 1, 0);
        }
    }

    public class FilterViewModel : ViewModelBase<IFilter>
    {
        public FilterViewModel(IFilter model, ViewModel viewModel) : base(model)
        {
            this.ViewModel = viewModel;
            var x = this.ViewModel.AutoBiquad.GetFrequencies();
            IReadOnlyList<double> y;
            if (this.Model is GainFilter gf)
                y = Enumerable.Repeat(FrequencyDomain.LinearToDb(gf.Gain), x.Count).ToReadOnlyList();
            else
                y = FrequencyDomain.LinearToDb(((BiquadFilter)this.Model).GetFrequencyResponse(x).Magitude()).ToReadOnlyList();

            this.GraphViewModel = new GraphViewModel(x, y, "Selected Filter");
        }

        public GraphViewModel GraphViewModel { get; }
        public ViewModel ViewModel { get; }
    }

    public class FilterViewModelFactory : Factory<FilterViewModel, IFilter>
    {
        public FilterViewModelFactory(ViewModel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public ViewModel ViewModel { get; }

        protected override FilterViewModel CreateItemOverride(IFilter parameter)
        {
            return new FilterViewModel(parameter, this.ViewModel);
        }
    }
}