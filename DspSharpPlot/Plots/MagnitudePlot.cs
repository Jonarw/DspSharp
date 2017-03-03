// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MagnitudePlot.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp.Algorithms;
using DspSharp.Spectrum;
using DspSharpPlot.Axes;
using OxyPlot.Axes;

namespace DspSharpPlot
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