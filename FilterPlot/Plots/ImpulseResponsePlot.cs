using System;
using System.Linq;
using Filter.Signal;
using FilterPlot.Axes;
using FilterPlot.Graphs;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PropertyTools.DataAnnotations;

namespace FilterPlot
{
    public class ImpulseResponsePlot : SignalPlot
    {
        private int _XMax = 1024;
        private int _XMin;
        private LinearAxis XAxis { get; set; }
        private LinearAxis YAxis { get; set; }

        public ImpulseResponsePlot()
        {
            this.DisplayName = "Impulse Response";
        }

        protected override Series CreateGraph(ISignal signal)
        {
            var ret = new ImpulseResponseGraph();

            ret.Points.AddRange(
                signal.GetWindowedSignal(this.XMin, this.XMax - this.XMin + 1)
                    .Zip(Enumerable.Range(this.XMin, this.XMax - this.XMin + 1), (m, t) => new DataPoint(t, m)));

            return ret;
            //var fsignal = signal as IFiniteSignal;
            //if (fsignal != null)
            //{
            //    ret.Points.AddRange(fsignal.Signal.Zip(Enumerable.Range(fsignal.Start, fsignal.Length), (m, t) => new DataPoint(t, m)));
            //    return ret;
            //}

            //var esignal = signal as IEnumerableSignal;
            //if (esignal != null)
            //{
            //    var wsignal = esignal.Multiply(this.CausalWindow);
            //    return this.CreateGraph(wsignal);
            //}

            //var iwsignal = signal.Multiply(this.SymmetricWindow);
            //return this.CreateGraph(iwsignal);
        }

        protected override Axis CreateXAxis()
        {
            return new SampleAxis {Minimum = this.XMin, Maximum = this.XMax, IsZoomEnabled = false, IsPanEnabled = false};
        }

        protected override Axis CreateYAxis()
        {
            return this.YAxis ?? (this.YAxis = new ImpulseResponseAxis());
        }

        [Category("X axis")]
        [DisplayName("minimum")]
        public int XMin
        {
            get { return this._XMin; }
            set
            {
                this.SetField(ref this._XMin, value);
                if (this.XMax < this.XMin)
                {
                    this.XMax = this.XMin + 100;
                }
            }
        }

        [DisplayName("maximum")]
        public int XMax
        {
            get { return this._XMax; }
            set
            {
                this.SetField(ref this._XMax, value);
                if (this.XMax < this.XMin)
                {
                    this.XMin = this.XMax - 100;
                }
            }
        }
    }
}