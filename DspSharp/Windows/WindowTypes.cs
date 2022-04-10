// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowTypes.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DspSharp.Signal.Windows
{
    /// <summary>
    /// Enumeration of all supported window types.
    /// </summary>
    public enum WindowType
    {
        Rectangular,
        Hann,
        Hamming,
        Triangular,
        Welch,
        Blackman,
        BlackmanHarris,
        KaiserAlpha2,
        KaiserAlpha3
    }
}