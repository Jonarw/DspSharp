// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BiquadFilterType.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DspSharp.Filter.LtiFilters.Iir
{
    /// <summary>
    /// Enumerates the available kinds of biquad filters.
    /// </summary>
    public enum BiquadFilterType
    {
        Lowpass,
        Highpass,
        Peaking,
        Bandpass,
        Notch,
        Lowshelf,
        Highshelf,
        Allpass
    }
}