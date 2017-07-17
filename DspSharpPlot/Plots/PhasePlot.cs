// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhasePlot.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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

        public override Axis YAxis { get; } = new PhaseAxis();

        protected override IEnumerable<double> GetYValues(ISpectrum spectrum)
        {
            return spectrum.Phase;
        }
    }
}