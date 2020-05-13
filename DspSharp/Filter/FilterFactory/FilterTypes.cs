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
    public enum FilterType
    {
        Distortion,
        Biquad,
        Butterworth,
        CustomConvolver,
        Correcting,
        Delay,
        Dirac,
        Fir,
        Gain,
        Iir,
        Invert,
        Zero
    }
}