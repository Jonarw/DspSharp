using System.Collections.Generic;
using DspSharp.Spectrum;
using DspSharpPlot.Axes;
using OxyPlot.Axes;

namespace DspSharpPlot
{
    public class PhasePlot : SpectrumPlot
    {
        public PhasePlot()
        {
            this.DisplayName = "phase";
        }

        protected override Axis YAxis { get; } = new PhaseAxis();

        protected override IEnumerable<double> GetYValues(ISpectrum spectrum)
        {
            return spectrum.Phase;
        }
    }
}