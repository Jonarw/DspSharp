// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImpulseResponseAxis.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DspSharpPlot.Axes
{
    /// <summary>
    ///     Represents a linear scaled axis without unit for impulse responses.
    /// </summary>
    /// <seealso cref="DefaultAxis" />
    public sealed class ImpulseResponseAxis : DefaultAxis
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ImpulseResponseAxis" /> class.
        /// </summary>
        public ImpulseResponseAxis()
        {
            this.Title = "value";
        }
    }
}