// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleAxis.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DspSharpPlot.Axes
{
    /// <summary>
    ///     Represents a sample axis.
    /// </summary>
    /// <seealso cref="DefaultAxis" />
    public sealed class SampleAxis : DefaultAxis
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SampleAxis" /> class.
        /// </summary>
        public SampleAxis()
        {
            this.Title = "Time [samples]";
            this.MinimumMajorStep = 1;
            this.MinimumMinorStep = 1;
        }
    }
}