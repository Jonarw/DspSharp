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
        public int XMax
        {
            get { return (int)(this.XAxis?.Maximum ?? 0); }
            set
            {
                this.GetXAxis().Maximum = value;
                if (this.XMax < this.XMin)
                {
                    this.XMin = this.XMax - 1;
                }
            }
        }

        public int XMin
        {
            get { return (int)(this.XAxis?.Minimum ?? 0); }
            set
            {
                this.GetXAxis().Minimum = value;
                if (this.XMax < this.XMin)
                {
                    this.XMax = this.XMin + 1;
                }
            }
        }

        private LinearAxis YAxis { get; set; }
        private LinearAxis XAxis { get; set; }

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
                var wsignal = esignal.Multiply(this.CausalWindow);
                return this.CreateGraph(wsignal);
            }

            var iwsignal = signal.Multiply(this.SymmetricWindow);
            return this.CreateGraph(iwsignal);
        }

        protected override Axis GetXAxis()
        {
            return this.XAxis ?? (this.XAxis = new SampleAxis());
        }

        protected override Axis GetYAxis()
        {
            if (this.YAxis == null)
            {
                this.YAxis = new ImpulseResponseAxis();
            }

            return this.YAxis;
        }
    }
}