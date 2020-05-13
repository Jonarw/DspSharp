using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DspSharp.Algorithms;
using DspSharp.Filter;
using DspSharp.Filter.LtiFilters.Iir;
using DspSharp.Filter.LtiFilters.Primitive;
using OxyPlot;
using OxyPlot.Axes;
using UmtUtilities;
using UmtUtilities.Collections;
using UmtUtilities.ViewModel;

namespace AutoBiquad
{
    public class ViewModel : ViewModelBase
    {
        private GraphViewModel _ComparisonGraphViewModel;
        private string _ComparisonPath;
        private char _CsvSeparator = ';';
        private char _DecimalPoint = ',';
        private ICommand _ExecuteCommand;
        private GraphViewModel _FilteredComparisonGraphViewModel;
        private GraphViewModel _FilteredGraphViewModel;
        private GraphViewModel _OriginalGraphViewModel;
        private GraphViewModel _TargetGraphViewModel;
        private string _TargetPath;

        public ViewModel()
        {
            this.OxyModel = new PlotModel
            {
                LegendBackground = OxyColor.FromArgb(150, 255, 255, 255),
                LegendBorder = OxyColor.FromArgb(128, 0, 0, 0),
                LegendOrientation = LegendOrientation.Horizontal,
                LegendPosition = LegendPosition.TopLeft,
                IsLegendVisible = true,
                PlotMargins = new OxyThickness(double.NaN, double.NaN, 10, double.NaN),
                DefaultFont = "CMU Sans Serif",
                DefaultFontSize = 11
            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                MajorGridlineThickness = 2,
                Title = "Magnitude [dB]"
            };

            var xAxis = new LogarithmicAxis
            {
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                MajorGridlineThickness = 2,
                Title = "Frequency [Hz]"
            };

            this.OxyModel.Axes.Add(xAxis);
            this.OxyModel.Axes.Add(yAxis);

            this.FiltersViewModel = new FiltersViewModel(this);
            this.OptimizationViewModel = new OptimizationViewModel(this);
        }

        public OptimizationViewModel OptimizationViewModel { get; }

        public AutoBiquadModel AutoBiquad { get; } = new AutoBiquadModel();

        public GraphViewModel ComparisonGraphViewModel
        {
            get { return this._ComparisonGraphViewModel; }
            private set { this.SetField(ref this._ComparisonGraphViewModel, value); }
        }

        public string ComparisonPath
        {
            get { return this._ComparisonPath; }
            set
            {
                this.SetField(ref this._ComparisonPath, value);
                this.UpdateComparisonGraphViewModel();
            }
        }

        public char CsvSeparator
        {
            get { return this._CsvSeparator; }
            set { this.SetField(ref this._CsvSeparator, value); }
        }

        public char DecimalPoint
        {
            get { return this._DecimalPoint; }
            set { this.SetField(ref this._DecimalPoint, value); }
        }

        public ICommand ExecuteCommand => this._ExecuteCommand ?? (this._ExecuteCommand = new RelayCommand(
                                              param => this.Execute(),
                                              o => this.OriginalGraphViewModel != null && this.TargetGraphViewModel != null &&
                                                   this.OriginalGraphViewModel.X.Count > 1 && this.TargetGraphViewModel.X.Count > 1));

        public GraphViewModel FilteredComparisonGraphViewModel
        {
            get { return this._FilteredComparisonGraphViewModel; }
            private set { this.SetField(ref this._FilteredComparisonGraphViewModel, value); }
        }

        public GraphViewModel FilteredGraphViewModel
        {
            get { return this._FilteredGraphViewModel; }
            private set { this.SetField(ref this._FilteredGraphViewModel, value); }
        }

        public IObservableList<IFilter> Filters { get; } = new ObservableList<IFilter>();

        public FiltersViewModel FiltersViewModel { get; }

        public GraphViewModel OriginalGraphViewModel
        {
            get { return this._OriginalGraphViewModel; }
            private set { this.SetField(ref this._OriginalGraphViewModel, value); }
        }

        public string OriginalPath
        {
            get { return this._ComparisonPath; }
            set
            {
                this.SetField(ref this._ComparisonPath, value);
                this.UpdateOriginalGraphViewModel();
            }
        }

        public PlotModel OxyModel { get; }

        public GraphViewModel TargetGraphViewModel
        {
            get { return this._TargetGraphViewModel; }
            private set { this.SetField(ref this._TargetGraphViewModel, value); }
        }

        public string TargetPath
        {
            get { return this._TargetPath; }
            set
            {
                this.SetField(ref this._TargetPath, value);
                this.UpdateTargetGraphViewModel();
            }
        }

        public static List<double> ApplyFilters(IReadOnlyList<double> x, IEnumerable<double> y, IEnumerable<BiquadFilter> filters)
        {
            return filters.Where(f => f.Enabled).Aggregate(
                y,
                (current, filter) => current.Add(FrequencyDomain.LinearToDb(filter.GetFrequencyResponse(x).Magitude()))).ToList();
        }

        public static List<double> ApplyFilters(IReadOnlyList<double> x, IEnumerable<double> y, IReadOnlyList<IFilter> filters)
        {
            var ygain = filters.OfType<GainFilter>().Where(f => f.Enabled).Aggregate(
                y,
                (current, filter) => current.Add(FrequencyDomain.LinearToDb(filter.Gain)));

            return filters.OfType<BiquadFilter>().Where(f => f.Enabled).Aggregate(
                ygain,
                (current, filter) => current.Add(FrequencyDomain.LinearToDb(filter.GetFrequencyResponse(x).Magitude()))).ToList();
        }

        public void Execute()
        {
            this.AutoBiquad.SetTarget(this.TargetGraphViewModel.X, this.TargetGraphViewModel.Y);
            this.Filters.Reset(this.AutoBiquad.MakeFilters(this.OriginalGraphViewModel.X, this.OriginalGraphViewModel.Y));
            this.UpdateFilteredGraphViewModel();
        }

        public GraphViewModel GetFilteredGraphViewModel(IReadOnlyList<double> x, IReadOnlyList<double> y, IReadOnlyList<double> xTarget, string title)
        {
            var yret = Interpolation.AdaptiveInterpolation(x, y, xTarget, true, false);
            var yretlist = ApplyFilters(xTarget, yret, this.Filters.ToReadOnlyList());
            return new GraphViewModel(xTarget, yretlist, title);
        }

        internal void UpdateFilteredGraphViewModel()
        {
            if (this.FilteredGraphViewModel != null)
                this.OxyModel.Series.Remove(this.FilteredGraphViewModel.Model);
            if (this.FilteredComparisonGraphViewModel != null)
                this.OxyModel.Series.Remove(this.FilteredComparisonGraphViewModel.Model);

            var x = this.AutoBiquad.GetFrequencies();
            this.FilteredGraphViewModel = this.GetFilteredGraphViewModel(this.OriginalGraphViewModel.X, this.OriginalGraphViewModel.Y, x, "Filtered");
            this.OxyModel.Series.Add(this.FilteredGraphViewModel.Model);

            if (this.ComparisonGraphViewModel != null && this.ComparisonGraphViewModel.X.Count > 0)
            {
                this.FilteredComparisonGraphViewModel = this.GetFilteredGraphViewModel(
                    this.ComparisonGraphViewModel.X,
                    this.ComparisonGraphViewModel.Y,
                    x,
                    "Filtered Comparison");
                this.OxyModel.Series.Add(this.FilteredComparisonGraphViewModel.Model);
            }

            this.UpdatePlot();
        }

        internal void UpdatePlot()
        {
            this.OxyModel.InvalidatePlot(true);
        }

        private void UpdateComparisonGraphViewModel()
        {
            if (this.ComparisonGraphViewModel != null)
                this.OxyModel.Series.Remove(this.ComparisonGraphViewModel.Model);

            var (x, y) = CsvImporter.ImportXyData(this.ComparisonPath, this.CsvSeparator, this.DecimalPoint);
            this.ComparisonGraphViewModel = new GraphViewModel(x, y, "Comparison");
            if (this.FilteredGraphViewModel != null)
                this.UpdateFilteredGraphViewModel();
            //this.OxyModel.Series.Add(this.ComparisonGraphViewModel.Model);
            this.UpdatePlot();
        }

        private void UpdateOriginalGraphViewModel()
        {
            if (this.OriginalGraphViewModel != null)
                this.OxyModel.Series.Remove(this.OriginalGraphViewModel.Model);

            var (x, y) = CsvImporter.ImportXyData(this.OriginalPath, this.CsvSeparator, this.DecimalPoint);
            this.OriginalGraphViewModel = new GraphViewModel(x, y, "Original");
            this.OxyModel.Series.Add(this.OriginalGraphViewModel.Model);
            if (this.FilteredGraphViewModel != null)
                this.UpdateFilteredGraphViewModel();

            this.UpdatePlot();
        }

        private void UpdateTargetGraphViewModel()
        {
            if (this.TargetGraphViewModel != null)
                this.OxyModel.Series.Remove(this.TargetGraphViewModel.Model);

            var (x, y) = CsvImporter.ImportXyData(this.TargetPath, this.CsvSeparator, this.DecimalPoint);
            this.TargetGraphViewModel = new GraphViewModel(x, y, "Target");
            this.OxyModel.Series.Add(this.TargetGraphViewModel.Model);
            this.UpdatePlot();
        }
    }
}