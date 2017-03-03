// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFftSpectrum.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp.Series;

namespace DspSharp.Spectrum
{
    public interface IFftSpectrum : ISpectrum
    {
        new FftSeries Frequencies { get; }
        IReadOnlyList<double> GetTimeDomainSignal();
    }
}