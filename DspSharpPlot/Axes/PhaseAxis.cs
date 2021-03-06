﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhaseAxis.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace DspSharpPlot.Axes
{
    /// <summary>
    ///     Represents a phase axis (degree).
    /// </summary>
    /// <seealso cref="DefaultAxis" />
    public class PhaseAxis : DefaultAxis
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PhaseAxis" /> class.
        /// </summary>
        public PhaseAxis()
        {
            this.Minimum = -Math.PI;
            this.Maximum = Math.PI;
            this.FormatAsFractions = true;
            this.FractionUnit = Math.PI;
            this.FractionUnitSymbol = "π";
            this.Title = "Phase [rad]";
            this.IsPanEnabled = false;
            this.IsZoomEnabled = false;
            this.MajorStep = Math.PI / 2;
            this.MinorStep = Math.PI / 4;
        }
    }
}