using System.Collections.Generic;
using Filter.Signal;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Filter.Signal.Windows;

namespace FilterPlot
{
    public abstract class SignalPlot
    {
        protected SignalPlot()
        {
            this.Model.LegendBackground = OxyColor.FromArgb(150, 255, 255, 255);
            this.Model.LegendBorder = OxyColor.FromArgb(128, 0, 0, 0);
            this.Model.LegendOrientation = LegendOrientation.Horizontal;
            this.Model.LegendPosition = LegendPosition.TopLeft;
            this.Model.IsLegendVisible = true;
            var xaxis = this.GetXAxis();
            xaxis.Position = AxisPosition.Bottom;
            this.Model.Axes.Add(xaxis);

            var yaxis = this.GetYAxis();
            yaxis.Position = AxisPosition.Left;
            this.Model.Axes.Add(yaxis);
        }



        public Window CausalWindow { get; set; } = new Window(WindowTypes.Rectangular, 8192, 44100, WindowModes.Causal);
        public Window SymmetricWindow { get; set; } = new Window(WindowTypes.Rectangular, 8192, 44100, WindowModes.Symmetric);

        public PlotModel Model { get; } = new PlotModel();
        public IEnumerable<ISignal> Signals { get; set; } = new List<ISignal>();

        public void Update(bool updateData)
        {
            if (!updateData)
            {
                this.Model.InvalidatePlot(false);
                return;
            }

            this.Model.Series.Clear();

            var newGraphs = new Dictionary<ISignal, Series>();

            foreach (var signal in this.Signals)
            {
                Series newGraph;

                if (this.Graphs.ContainsKey(signal))
                {
                    newGraph = this.Graphs[signal];
                }
                else
                {
                    newGraph = this.CreateGraph(signal);
                }

                newGraphs.Add(signal, newGraph);
                this.Model.Series.Add(newGraph);
            }

            this.Graphs = newGraphs;
            this.Model.InvalidatePlot(true);
        }

        protected abstract Series CreateGraph(ISignal signal);
        protected abstract Axis GetXAxis();
        protected abstract Axis GetYAxis();

        private Dictionary<ISignal, Series> Graphs { get; set; } = new Dictionary<ISignal, Series>(); 
    }
}