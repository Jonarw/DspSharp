using System.Linq;
using Filter.Extensions;
using Filter.Signal;
using Filter.Signal.Windows;
using FilterPlot.Axes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using static Filter.Algorithms.Dsp;

namespace FilterPlot
{
    public class MagnitudePlot : SignalPlot
    {
        public Window CausalWindow { get; set; } = new Window(WindowTypes.Hann, 8192, 44100, WindowModes.Causal);
        public Window SymmetricWindow { get; set; } = new Window(WindowTypes.Hann, 8192, 44100, WindowModes.Symmetric);
        private Axis XAxis { get; set; }
        private Axis YAxis { get; set; }

        protected override Series CreateGraph(ISignal signal)
        {
            var ret = new LineSeries();
            var fsignal = signal as IFiniteSignal;
            if (fsignal != null)
            {
                ret.Points.AddRange(LinearToDb(fsignal.Spectrum.Magnitude, -1000).Zip(fsignal.Spectrum.Frequencies.Values, (m, f) => new DataPoint(f, m)));
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
                ret.Points.AddRange(LinearToDb(ssignal.Spectrum.Magnitude, -1000).Zip(ssignal.Spectrum.Frequencies.Values, (m, f) => new DataPoint(f, m)));
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
                this.YAxis = new AmplitudeAxis();
            }

            return this.YAxis;
        }
    }
}