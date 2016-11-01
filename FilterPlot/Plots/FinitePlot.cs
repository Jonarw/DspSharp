using Filter.Extensions;
using Filter.Signal;
using Filter.Signal.Windows;
using OxyPlot.Series;

namespace FilterPlot
{
    public abstract class FinitePlot : SignalPlot
    {
        protected sealed override Series CreateGraph(ISignal signal)
        {
            var fsignal = signal as IFiniteSignal;
            if (fsignal != null)
            {
                return this.CreateGraph(fsignal);
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

        protected abstract Series CreateGraph(IFiniteSignal signal);
    }
}