using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using DspSharp;
using DspSharp.Algorithms;
using DspSharp.Utilities;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace DspSharpPlot
{
    public static class DspSharpPlotExtensions
    {
        public static DspSharpPlotter Plot(this IReadOnlyList<double> yData, IReadOnlyList<double> xData)
        {
            var plot = new DspSharpPlotter();

            var t = new Thread(() => CreatePlotWindow(plot));
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;

            plot.Graphs.Add(new GraphData(xData, yData));
            t.Start();
            plot.UpdatePlot();

            Thread.Sleep(1000);
            return plot;
        }

        private static void CreatePlotWindow(DspSharpPlotter plot)
        {
            var win = new PlotWindow();
            win.ViewModel.Plotter = plot;
            win.Show();
            plot.UpdatePlot();
            Dispatcher.Run();
        }

        public static void Plot(this IReadOnlyList<double> yData, IReadOnlyList<double> xData, DspSharpPlotter plot)
        {
            plot.Graphs.Add(new GraphData(xData, yData));
            plot.UpdatePlot();
        }

        public static DspSharpPlotter Plot(this IReadOnlyList<double> xData)
        {
            return xData.Plot(Enumerable.Range(0, xData.Count).Select(i => (double)i).ToReadOnlyList());
        }

        public static void Plot(this IReadOnlyList<double> xData, DspSharpPlotter plot)
        {
            xData.Plot(Enumerable.Range(0, xData.Count).Select(i => (double)i).ToReadOnlyList(), plot);
        }
    }

    public class GraphData : Observable
    {
        public GraphData(IReadOnlyList<double> xdata, IReadOnlyList<double> ydata, string displayName = "")
        {
            this.DisplayName = displayName;
            this.Xdata = xdata ?? throw new ArgumentNullException(nameof(xdata));
            this.Ydata = ydata ?? throw new ArgumentNullException(nameof(ydata));

            if (xdata.Count != ydata.Count)
                throw new ArgumentException();
        }

        public int Length => this.Xdata.Count;
        public string DisplayName { get; }

        public bool Visible
        {
            get => this._Visible;
            set => this.SetField(ref this._Visible, value);
        }

        private bool _Visible = true;

        public IReadOnlyList<double> Xdata { get; }
        public IReadOnlyList<double> Ydata { get; }
    }

    public class DspSharpPlotter
    {
        public DspSharpPlotter()
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
        }

        public IList<GraphData> Graphs { get; } = new List<GraphData>();
        public PlotModel OxyModel { get; }

        public string Xlabel { get; set; } = "X";
        public bool Xlogarithmic { get; set; }
        public string Ylabel { get; set; } = "Y";
        public bool Ylogarithmic { get; set; }

        public void UpdatePlot()
        {
            this.UpdateAxes();
            this.UpdateGraphs();
            this.OxyModel.InvalidatePlot(true);
        } 

        private void UpdateAxes()
        {
            this.OxyModel.Axes.Clear();
            var xaxis = this.Xlogarithmic ? (Axis)new LogarithmicAxis() : new LinearAxis();
            xaxis.Position = AxisPosition.Bottom;
            xaxis.Title = this.Xlabel;
            xaxis.MajorGridlineStyle = LineStyle.Automatic;
            xaxis.MinorGridlineStyle = LineStyle.Automatic;
            this.OxyModel.Axes.Add(xaxis);

            var yaxis = this.Ylogarithmic ? (Axis)new LogarithmicAxis() : new LinearAxis();
            yaxis.MajorGridlineStyle = LineStyle.Automatic;
            yaxis.MinorGridlineStyle = LineStyle.Automatic;
            yaxis.Title = this.Ylabel;

            this.OxyModel.Axes.Add(yaxis);
        }

        private void UpdateGraphs()
        {
            this.OxyModel.Series.Clear();
            foreach (var graph in this.Graphs.Where(g => g.Visible))
            {
                var series = new LineSeries()
                {
                    Title = graph.DisplayName,
                };

                series.Points.AddRange(graph.Xdata.Zip(graph.Ydata, (x, y) => new DataPoint(x, y)));
                this.OxyModel.Series.Add(series);
            }
        }
    }
}