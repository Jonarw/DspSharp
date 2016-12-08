using System.Collections.Generic;
using Filter.Algorithms;
using Filter.Spectrum;
using FilterPlot.Axes;
using OxyPlot.Axes;

namespace FilterPlot
{
    public class MagnitudePlot : SpectrumPlot
    {
        public MagnitudePlot()
        {
            this.DisplayName = "magnitude";
        }

        private Axis YAxis { get; set; }

        protected override Axis CreateYAxis()
        {
            if (this.YAxis == null)
            {
                this.YAxis = new AmplitudeAxis();
            }

            return this.YAxis;
        }

        protected override IEnumerable<double> GetYValues(ISpectrum spectrum)
        {
            return Dsp.LinearToDb(spectrum.Magnitude, -1000);
        }
    }
}