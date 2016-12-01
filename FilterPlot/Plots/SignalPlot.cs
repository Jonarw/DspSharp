using System.Collections.Generic;
using System.ComponentModel;
using Filter;
using Filter.Signal;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Filter.Signal.Windows;

namespace FilterPlot
{
    public abstract class SignalPlot : Observable
    {
        protected SignalPlot()
        {
            this.Model.LegendBackground = OxyColor.FromArgb(150, 255, 255, 255);
            this.Model.LegendBorder = OxyColor.FromArgb(128, 0, 0, 0);
            this.Model.LegendOrientation = LegendOrientation.Horizontal;
            this.Model.LegendPosition = LegendPosition.TopLeft;
            this.Model.IsLegendVisible = true;

            this.PropertyChanged += this.ConfigChanged;
        }

        private void ConfigChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Update(true);
        }

        public PlotModel Model { get; } = new PlotModel();
        public IList<ISignal> Signals { get; } = new List<ISignal>();
        public string DisplayName { get; set; }

        public void Update(bool updateData)
        {
            if (!updateData)
            {
                this.Model.InvalidatePlot(false);
                return;
            }

            this.Model.Series.Clear();
            this.Model.Axes.Clear();

            var xaxis = this.CreateXAxis();
            xaxis.Position = AxisPosition.Bottom;
            this.Model.Axes.Add(xaxis);

            var yaxis = this.CreateYAxis();
            yaxis.Position = AxisPosition.Left;
            this.Model.Axes.Add(yaxis);

            foreach (var signal in this.Signals)
            {
                this.Model.Series.Add(this.CreateGraph(signal));
            }

            this.Model.InvalidatePlot(true);
        }

        protected abstract Series CreateGraph(ISignal signal);
        protected abstract Axis CreateXAxis();
        protected abstract Axis CreateYAxis();

    }
}