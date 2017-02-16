using System;
using System.Linq;
using Filter.Signal;
using FilterPlot.Axes;
using FilterPlot.Graphs;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace FilterPlot
{
    public class ImpulseResponsePlot : SignalPlot
    {
        public ImpulseResponsePlot()
        {
            this.DisplayName = "time domain";
            this.XAxis.AxisChanged += this.XAxisChanged;
        }

        public int XMax => (int)Math.Floor(this.XAxis.ActualMaximum);

        public int XMin => (int)Math.Ceiling(this.XAxis.ActualMinimum);

        protected sealed override Axis XAxis { get; } = new SampleAxis();
        protected sealed override Axis YAxis { get; } = new ImpulseResponseAxis();
        private int DataMax { get; set; }
        private int DataMin { get; set; }

        protected override Series CreateGraph(ISignal signal)
        {
            var ret = new ImpulseResponseGraph();

            var fsignal = signal as IFiniteSignal;
            if (fsignal != null)
            {
                ret.Points.AddRange(fsignal.Signal.Zip(Enumerable.Range(fsignal.Start, fsignal.Length), (m, t) => new DataPoint(t, m)));
                return ret;
            }

            var esignal = signal as IEnumerableSignal;
            if (esignal != null)
            {
                ret.Points.AddRange(
                    signal.GetWindowedSamples(esignal.Start, this.DataMax - esignal.Start)
                        .Zip(Enumerable.Range(esignal.Start, this.DataMax - esignal.Start), (m, t) => new DataPoint(t, m)));
                return ret;
            }

            ret.Points.AddRange(
                signal.GetWindowedSamples(this.DataMin, this.DataMax - this.DataMin + 1)
                    .Zip(Enumerable.Range(this.DataMin, this.DataMax - this.DataMin + 1), (m, t) => new DataPoint(t, m)));

            return ret;
        }

        private void XAxisChanged(object sender, AxisChangedEventArgs e)
        {
            var range = this.XAxis.ActualMaximum - this.XAxis.ActualMinimum;

            if ((this.XAxis.ActualMinimum < this.DataMin) || (this.XAxis.ActualMinimum > this.DataMin + 4 * range))
            {
                this.DataMin = (int)Math.Floor(this.XAxis.ActualMinimum - 0.5 * range);
                this.Update(true);
            }

            if ((this.XAxis.ActualMaximum > this.DataMax) || (this.XAxis.ActualMaximum < this.DataMax + 4 * range))
            {
                this.DataMax = (int)Math.Ceiling(this.XAxis.ActualMaximum + 0.5 * range);
                this.Update(true);
            }

            this.RaisePropertyChanged(nameof(this.XMin));
            this.RaisePropertyChanged(nameof(this.XMax));
        }
    }
}