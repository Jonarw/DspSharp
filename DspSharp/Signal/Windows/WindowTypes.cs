// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowTypes.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;

namespace DspSharp.Signal.Windows
{
    /// <summary>
    ///     Enumeration of all supported window types.
    /// </summary>
    public enum WindowTypes
    {
        /// <summary>
        ///     The rectangular window.
        /// </summary>
        [Description("rectangular")] Rectangular,

        /// <summary>
        ///     The Hann window.
        /// </summary>
        [Description("Hann")] Hann,

        /// <summary>
        ///     The Hamming window.
        /// </summary>
        [Description("Hamming")] Hamming,

        /// <summary>
        ///     The triangular window.
        /// </summary>
        [Description("triangular")] Triangular,

        /// <summary>
        ///     The Welch window.
        /// </summary>
        [Description("Welch")] Welch,

        /// <summary>
        ///     The Blackman window.
        /// </summary>
        [Description("Blackman")] Blackman,

        /// <summary>
        ///     The Blackman-Harris window.
        /// </summary>
        [Description("Blackman-Harris")] BlackmanHarris,

        /// <summary>
        ///     The Kaiser window with alpha=2.
        /// </summary>
        [Description("Kaiser, alpha = 2")] KaiserAlpha2,

        /// <summary>
        ///     The Kaiser window with alpha=3.
        /// </summary>
        [Description("Kaiser, alpha = 3")] KaiserAlpha3
    }
}