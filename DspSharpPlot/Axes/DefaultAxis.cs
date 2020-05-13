// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAxis.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace DspSharpPlot.Axes
{
    public class LineSeriesEx : LineSeries
    {
        public override void Render(IRenderContext rc)
        {
            IEnumerable<DataPoint> points = this.ActualPoints;
            if (points == null)
                return;

            if (this.XAxis is LogarithmicAxis)
                points = points.Where(p => p.X > 0);
            if (this.YAxis is LogarithmicAxis)
                points = points.Where(p => p.Y > 0);

            var pointsList = points.ToList();
            if (pointsList.Count == 0)
                return;

            this.VerifyAxes();

            var clippingRect = this.GetClippingRect();
            rc.SetClip(clippingRect);

            this.RenderPoints(rc, clippingRect, pointsList);

            if (this.LabelFormatString != null)
            {
                // render point labels (not optimized for performance)
                this.RenderPointLabels(rc, clippingRect);
            }

            rc.ResetClip();

            if (this.LineLegendPosition != LineLegendPosition.None && !string.IsNullOrEmpty(this.Title))
            {
                // renders a legend on the line
                this.RenderLegendOnLine(rc);
            }
        }

        protected override void UpdateMaxMin()
        {
            this.MinX = this.MinY = this.MaxX = this.MaxY = double.NaN;
            IEnumerable<DataPoint> points;
            if (this.InterpolationAlgorithm != null)
            {
                base.UpdateMaxMin();
                this.ResetSmoothedPoints();
                if (this.SmoothedPoints.Count == 0)
                    return;

                points = this.SmoothedPoints;
            }
            else
                points = this.ActualPoints;

            if (this.XAxis is LogarithmicAxis)
                points = points.Where(p => p.X > 0);
            if (this.YAxis is LogarithmicAxis)
                points = points.Where(p => p.Y > 0);

            this.InternalUpdateMaxMin(points.ToList());
        }
    }

    public class LogarithmicAxisEx : LogarithmicAxis
    {
    }

    /// <summary>
    ///     Extends <see cref="OxyPlot.Axes.LinearAxis" /> for custom value initializations.
    /// </summary>
    /// <seealso cref="OxyPlot.Axes.LinearAxis" />
    public class DefaultAxis : LinearAxis
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultAxis" /> class.
        /// </summary>
        public DefaultAxis()
        {
            this.MajorGridlineStyle = LineStyle.Solid;
            this.MinorGridlineStyle = LineStyle.Solid;
            this.MajorGridlineThickness = 2;
        }
    }
}