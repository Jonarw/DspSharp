using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using OxyPlot.Wpf;
using UmtUtilities;
using UmtUtilities.Extensions;
using UmtUtilities.ViewModel;
using UTilities;
using UTilities.Collections;
using UTilities.Helpers;
using UTilities.Observable;
using LineSeries = OxyPlot.Series.LineSeries;

namespace DspSharpPlot
{
    public class ColorItem : Observable
    {
        private Color _SelectedColor;

        public ColorItem(string displayName)
        {
            this.DisplayName = displayName;
        }

        public string DisplayName { get; }

        public Color SelectedColor
        {
            get => this._SelectedColor;
            set => this.SetField(ref this._SelectedColor, value);
        }
    }

    public class GraphsViewModel : ViewModelBase
    {
        private static readonly IList<string> ExcludedGraphProperties = new List<string> {nameof(LineSeries.ItemsSource), nameof(LineSeries.Title)};

        private readonly ObservableList<LineSeries> _Graphs = new ObservableList<LineSeries>();
        private LineSeries _SelectedGraph;
        private readonly ObservableList<LineSeries> _SelectedGraphs = new ObservableList<LineSeries>();
        private bool graphsChanged;

        public GraphsViewModel(PlotViewModel parent) : base(parent.DialogProvider)
        {
            this.Parent = parent;
            this.DeleteGraphCommand = new RelayCommand(this.DeleteGraph, () => this.SelectedGraph != null);
            this.SetGraphsToGradientCommand = new RelayCommand(this.SetGraphsToGradient, () => this.SelectedGraphs.Count > 1);
            this.MoveBottomCommand = new RelayCommand(this.MoveBottom, () => this.SelectedGraph != null && this.Graphs.IndexOf(this.SelectedGraph) < this.Graphs.Count - 1);
            this.MoveTopCommand = new RelayCommand(this.MoveTop, () => this.SelectedGraph != null && this.Graphs.IndexOf(this.SelectedGraph) > 0);
            this.MoveDownCommand = new RelayCommand(this.MoveDown, () => this.SelectedGraph != null && this.Graphs.IndexOf(this.SelectedGraph) < this.Graphs.Count - 1);
            this.MoveUpCommand = new RelayCommand(this.MoveUp, () => this.SelectedGraph != null && this.Graphs.IndexOf(this.SelectedGraph) > 0);
            this.AddGradientColorCommand = new RelayCommand(this.AddGradientColor);
            this.SelectAllCommand = new RelayCommand(this.SelectAll);
            this.SelectNoneCommand = new RelayCommand(this.SelectNone);
            this.InvertSelectionCommand = new RelayCommand(this.InvertSelection);
            this.RemoveGradientColorCommand = new RelayCommand(this.RemoveGradientColor, () => this.GradientColors.Count > 2);
            this.SelectedSeriesWrapper = new LineSeriesWrapper(this._SelectedGraphs);
            this.Graphs.DispatcherAction = parent.DispatcherAction;
            this.Graphs.CollectionChanged += (sender, args) => this.graphsChanged = true;
            this._Graphs.UseSpecificActions = false;
            this.SelectedSeriesWrapper.TitleOrVisibleChanged += (sender, args) => this._Graphs.ForceUpdate();
            this.AddGradientColor();
            this.AddGradientColor();
            this.GradientColors[0].SelectedColor = Colors.Red;
            this.GradientColors[1].SelectedColor = Colors.Gray;
        }

        public ICommand AddGradientColorCommand { get; }
        public ICommand DeleteGraphCommand { get; }
        public IObservableList<ColorItem> GradientColors { get; } = new ObservableList<ColorItem>();
        public IObservableList<LineSeries> Graphs => this._Graphs;
        public ICommand InvertSelectionCommand { get; }
        public ICommand MoveBottomCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand MoveTopCommand { get; }
        public ICommand MoveUpCommand { get; }
        public PlotViewModel Parent { get; }
        public ICommand RemoveGradientColorCommand { get; }
        public ICommand SelectAllCommand { get; }

        public LineSeries SelectedGraph
        {
            get => this._SelectedGraph;
            set => this.SetField(ref this._SelectedGraph, value);
        }

        public IObservableList<LineSeries> SelectedGraphs => this._SelectedGraphs;

        public LineSeriesWrapper SelectedSeriesWrapper { get; }
        public ICommand SelectNoneCommand { get; }

        public ICommand SetGraphsToGradientCommand { get; }

        public void UpdateGraphs()
        {
            if (!this.graphsChanged)
                return;

            this.Parent.OxyModel.Series.Clear();
            foreach (var graph in this.Graphs)
                this.Parent.OxyModel.Series.Add(graph);

            this.graphsChanged = false;
        }

        private void AddGradientColor()
        {
            this.GradientColors.Add(new ColorItem($"Color {this.GradientColors.Count + 1}"));
        }

        private void ApplyToSelectedGraphs()
        {
            foreach (var graph in this.SelectedGraphs)
                Misc.TransferPublicProperties(this.SelectedGraph, graph, ExcludedGraphProperties);
        }

        private void DeleteGraph()
        {
            this.Parent.OxyModel.Series.Remove(this.SelectedGraph);
            this.Graphs.Remove(this.SelectedGraph);
        }

        private void InvertSelection()
        {
            foreach (var graph in this.Graphs)
                graph.IsVisible = !graph.IsVisible;

            this._Graphs.ForceUpdate();
        }

        private void MoveBottom()
        {
            this.Graphs.MoveItem(this.SelectedGraph, this.Graphs.Count - 1);
        }

        private void MoveDown()
        {
            this.Graphs.MoveItem(this.SelectedGraph, this.Graphs.IndexOf(this.SelectedGraph) + 1);
        }

        private void MoveTop()
        {
            this.Graphs.MoveItem(this.SelectedGraph, 0);
        }

        private void MoveUp()
        {
            this.Graphs.MoveItem(this.SelectedGraph, this.Graphs.IndexOf(this.SelectedGraph) - 1);
        }

        private void RemoveGradientColor()
        {
            this.GradientColors.RemoveAt(this.GradientColors.Count - 1);
        }

        private void SelectAll()
        {
            foreach (var graph in this.Graphs)
                graph.IsVisible = true;

            this._Graphs.ForceUpdate();
        }

        private void SelectNone()
        {
            foreach (var graph in this.Graphs)
                graph.IsVisible = false;

            this._Graphs.ForceUpdate();
        }

        private void SetGraphsToGradient()
        {
            var num = this.SelectedGraphs.Count;

            var delta = 1d / (num - 1);
            var currentx = 0d;

            foreach (var graph in this.SelectedGraphs)
            {
                graph.Color = UColor.Interpolate(currentx, this.GradientColors.Select(c => c.SelectedColor.ToUColor()).ToArray()).ToColor().ToOxyColor();
                currentx += delta;
            }
        }
    }
}