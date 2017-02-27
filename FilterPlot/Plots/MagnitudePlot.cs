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

        protected override Axis YAxis { get; } = new AmplitudeAxis();

        protected override IEnumerable<double> GetYValues(ISpectrum spectrum)
        {
            return FrequencyDomain.LinearToDb(spectrum.Magnitude, -1000);
        }
    }
}