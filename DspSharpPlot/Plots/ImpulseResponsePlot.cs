// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImpulseResponsePlot.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using DspSharp.Signal;
using DspSharpPlot.Axes;
using DspSharpPlot.Graphs;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace DspSharpPlot
{
    public class ImpulseResponsePlot : SignalPlot
    {
        public ImpulseResponsePlot()
        {
            this.DisplayName = "time domain";
            this.XAxis.AxisChanged += this.XAxisChanged;
        }

        public sealed override Axis XAxis { get; } = new SampleAxis();
        public sealed override Axis YAxis { get; } = new ImpulseResponseAxis();
        private int DataMax { get; set; }
        private int DataMin { get; set; }

        protected override Series CreateGraph(ISignal signal)
        {
            var ret = new ImpulseResponseGraph();

            switch (signal)
            {
                case IFiniteSignal fsignal:
                    ret.Points.AddRange(fsignal.Signal.Zip(Enumerable.Range(fsignal.Start, fsignal.Length), (m, t) => new DataPoint(t, m)));
                    return ret;
                case IEnumerableSignal esignal:
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

            this.OnPropertyChanged(nameof(this.XMin));
            this.OnPropertyChanged(nameof(this.XMax));
        }
    }
}