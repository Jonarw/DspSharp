using System;
using System.Linq;
using System.Windows.Input;
using DspSharpPlot.Axes;
using OxyPlot;
using OxyPlot.Axes;
using UmtUtilities;
using UmtUtilities.DialogProvider;
using UTilities;
using UTilities.Helpers;

namespace DspSharpPlot
{
    public class PlotViewModel : PlotViewModelBase
    {
        private ICommand _UpdatePlotCommand;
        private Axis _XAxis;
        private bool _XLogarithmic;
        private Axis _YAxis;
        private bool _YLogarithmic;

        public string TitleText
        {
            get => this._TitleText;
            private set => this.SetField(ref this._TitleText, value);
        }

        private string _TitleText;

        public PlotViewModel(IDialogProvider dialogProvider) : base(dialogProvider)
        {
            this.OxyModel.PlotMargins = new OxyThickness(double.NaN, double.NaN, 10, double.NaN);
            this.OxyModel.DefaultColors = DefaultColorPalette.Colors.Select(c => OxyColor.FromArgb(c.A, c.R, c.G, c.B)).ToList();

            this.UpdateAxes();
            this.DispatcherAction = dialogProvider.GetDispatcherAction();
            this.GraphsViewModel = new GraphsViewModel(this);
        }

        public Action<Action> DispatcherAction { get; }

        public GraphsViewModel GraphsViewModel { get; }

        public ICommand UpdatePlotCommand => this._UpdatePlotCommand ?? (this._UpdatePlotCommand = new RelayCommand(this.UpdatePlot, () => true));

        public Axis XAxis
        {
            get => this._XAxis;
            private set => this.SetField(ref this._XAxis, value);
        }

        public bool XLogarithmic
        {
            get => this._XLogarithmic;
            set => this.SetField(ref this._XLogarithmic, value);
        }

        public Axis YAxis
        {
            get => this._YAxis;
            private set => this.SetField(ref this._YAxis, value);
        }

        public bool YLogarithmic
        {
            get => this._YLogarithmic;
            set => this.SetField(ref this._YLogarithmic, value);
        }

        public void UpdatePlot()
        {
            this.UpdateAxes();
            this.UpdateTitle();
            this.GraphsViewModel.UpdateGraphs();
            this.DispatcherAction(() => this.OxyModel.InvalidatePlot(true));
        }

        private void UpdateTitle()
        {
            this.TitleText = $"{this.YAxis.Title} vs. {this.XAxis.Title} Plot";
        }

        private void UpdateAxes()
        {
            if (this.XAxis == null || this.XAxis is LogarithmicAxis != this.XLogarithmic)
            {
                var newAxis = this.XLogarithmic ? (Axis)new LogarithmicAxisEx() : new LinearAxis();

                if (this.XAxis != null)
                {
                    this.OxyModel.Axes.Remove(this.XAxis);
                    Misc.TransferPublicProperties(this.XAxis, newAxis);
                }
                else
                {
                    newAxis.Position = AxisPosition.Bottom;
                    newAxis.Title = "X";
                    newAxis.MajorGridlineStyle = LineStyle.Automatic;
                    newAxis.MinorGridlineStyle = LineStyle.Automatic;
                }

                this.XAxis = newAxis;
                this.OxyModel.Axes.Add(this.XAxis);
            }

            if (this.YAxis == null || this.YAxis is LogarithmicAxis != this.YLogarithmic)
            {
                var newAxis = this.YLogarithmic ? (Axis)new LogarithmicAxisEx() : new LinearAxis();

                if (this.YAxis != null)
                {
                    this.OxyModel.Axes.Remove(this.YAxis);
                    Misc.TransferPublicProperties(this.YAxis, newAxis);
                }
                else
                {
                    newAxis.Position = AxisPosition.Left;
                    newAxis.Title = "Y";
                    newAxis.MajorGridlineStyle = LineStyle.Automatic;
                    newAxis.MinorGridlineStyle = LineStyle.Automatic;
                }

                this.YAxis = newAxis;
                this.OxyModel.Axes.Add(this.YAxis);
            }
        }
    }
}