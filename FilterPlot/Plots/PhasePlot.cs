using System.Collections.Generic;
using Filter.Spectrum;
using FilterPlot.Axes;
using OxyPlot.Axes;

namespace FilterPlot
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