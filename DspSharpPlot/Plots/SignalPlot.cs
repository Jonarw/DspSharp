using System.Collections.Generic;
using System.ComponentModel;
using DspSharp;
using DspSharp.Signal;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace DspSharpPlot
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

            this.XAxis.Position = AxisPosition.Bottom;
            this.YAxis.Position = AxisPosition.Left;

            this.Model.Axes.Add(this.XAxis);
            this.Model.Axes.Add(this.YAxis);

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

            foreach (var signal in this.Signals)
            {
                this.Model.Series.Add(this.CreateGraph(signal));
            }

            this.Model.InvalidatePlot(true);
        }

        protected abstract Series CreateGraph(ISignal signal);
        protected abstract Axis XAxis { get; }
        protected abstract Axis YAxis { get; }

    }
}