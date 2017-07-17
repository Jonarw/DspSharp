using DspSharp;
using DspSharp.Utilities;

namespace DspSharpPlot
{
    public class PlotWindowViewModel : Observable
    {
        private DspSharpPlotter _Plotter;

        public DspSharpPlotter Plotter
        {
            get => this._Plotter;
            set => this.SetField(ref this._Plotter, value);
        }
    }
}