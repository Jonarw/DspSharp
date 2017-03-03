// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmplitudeAxis.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DspSharpPlot.Axes
{
    /// <summary>
    ///     Represents an amplitude axis (dB).
    /// </summary>
    /// <seealso cref="DefaultAxis" />
    public class AmplitudeAxis : DefaultAxis
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AmplitudeAxis" /> class.
        /// </summary>
        public AmplitudeAxis()
        {
            this.Title = "magnitude [dB]";
            //Me.Zoom(-50, 50)
        }
    }
}