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

        private LinearAxis YAxis { get; set; }

        protected override Axis CreateYAxis()
        {
            if (this.YAxis == null)
            {
                this.YAxis = new PhaseAxis();
            }

            return this.YAxis;
        }

        protected override IEnumerable<double> GetYValues(ISpectrum spectrum)
        {
            return spectrum.Phase;
        }
    }
}