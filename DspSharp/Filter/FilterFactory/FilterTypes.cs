// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterTypes.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;

namespace DspSharp.Filter
{
    /// <summary>
    ///     Enumerates the available filter types.
    /// </summary>
    public enum FilterTypes
    {
        [Description("distortion")] Distortion,
        [Description("biquad")] Biquad,
        [Description("convolver (custom signal)")] CustomConvolver,
        [Description("correcting (experimental)")] Correcting,
        [Description("delay")] Delay,
        [Description("dirac")] Dirac,
        [Description("fir highpass/lowpass")] Fir,
        [Description("simple gain")] Gain,
        [Description("IIR filter with custom coefficients")] Iir,
        [Description("inverter")] Invert,
        [Description("muter")] Zero
    }
}