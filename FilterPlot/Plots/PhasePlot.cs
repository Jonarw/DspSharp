using System;
using System.Linq;
using Filter.Signal;
using Filter.Signal.Windows;
using FilterPlot.Axes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace FilterPlot
{
    public class PhasePlot : SignalPlot
    {
        private bool _xLogarithmic;
        private Axis XAxis { get; set; }
        private LinearAxis YAxis { get; set; }

        public bool XLogarithmic
        {
            get { return this._xLogarithmic; }
            set
            {
                this._xLogarithmic = value;
                this.XAxis = null;
            }
        }

        public Window CausalWindow { get; set; } = new Window(WindowTypes.Hann, 8192, 44100, WindowModes.Causal);
        public Window SymmetricWindow { get; set; } = new Window(WindowTypes.Hann, 8192, 44100, WindowModes.Symmetric);


        protected override Series CreateGraph(ISignal signal)
        {
            var ret = new LineSeries();
            var fsignal = signal as IFiniteSignal;
            if (fsignal != null)
            {
                ret.Points.AddRange(fsignal.Spectrum.Phase.Zip(fsignal.Spectrum.Frequencies.Values, (p, f) => new DataPoint(f, p)));
                return ret;
            }

            var esignal = signal as IEnumerableSignal;
            if (esignal != null)
            {
                var wsignal = esignal.Multiply(this.CausalWindow);
                return this.CreateGraph(wsignal);
            }

            var ssignal = signal as ISyntheticSignal;
            if (ssignal != null)
            {
                ret.Points.AddRange(ssignal.Spectrum.Phase.Zip(ssignal.Spectrum.Frequencies.Values, (p, f) => new DataPoint(f, p)));
                return ret;
            }

            var iwsignal = signal.Multiply(this.SymmetricWindow);
            return this.CreateGraph(iwsignal);
        }

        protected override Axis GetXAxis()
        {
            if (this.XAxis == null)
            {
                this.XAxis = new FrequencyAxis();
            }

            return this.XAxis;
        }

        protected override Axis GetYAxis()
        {
            if (this.YAxis == null)
            {
                this.YAxis = new PhaseAxis();
            }

            return this.YAxis;
        }
    }
}