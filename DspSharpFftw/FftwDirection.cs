// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FftwDirection.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DspSharpFftw
{
    /// <summary>
    ///     Defines direction of operation
    /// </summary>
    public enum FftwDirection
    {
        /// <summary>
        ///     Computes a regular DFT
        /// </summary>
        Forward = -1,

        /// <summary>
        ///     Computes the inverse DFT
        /// </summary>
        Backward = 1
    }
}