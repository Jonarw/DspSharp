using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;

namespace DspSharpPlot.Graphs
{
    /// <summary>
    ///     Represents an impulse response graph to be used in an <see cref="ImpulseResponsePlot" />.
    /// </summary>
    public class ImpulseResponseGraph : LineSeries
    {
        private double _base = 0;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImpulseResponseGraph" /> class.
        /// </summary>
        public ImpulseResponseGraph()
        {
            //Me.Color = Me.PlotModel.GetDefaultColor
            this.MarkerStrokeThickness = this.StrokeThickness;
            this.MarkerSize = 4;
        }

        private ZoomLevels ZoomLevel
        {
            get
            {
                if (this.Points.Count <= 16)
                {
                    return ZoomLevels.Stems;
                }

                var ratio = (this.XAxis.ScreenMax.X - this.XAxis.ScreenMin.X) / (this.XAxis.ActualMaximum - this.XAxis.ActualMinimum);
                if (ratio < 10)
                {
                    return ZoomLevels.Line;
                }
                if ((ratio < 30) || (this.Points.Count > 128))
                {
                    return ZoomLevels.Markers;
                }

                return ZoomLevels.Stems;
            }
        }

        /// <summary>
        ///     Gets the point on the series that is nearest the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="interpolate">Interpolate the series if this flag is set to <c>true</c>.</param>
        /// <returns>
        ///     A TrackerHitResult for the current hit.
        /// </returns>
        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            if ((this.XAxis == null) || (this.YAxis == null))
            {
                return null;
            }

            if (interpolate)
            {
                return null;
            }

            var p1 = this.InverseTransform(point);
            var points = this.ActualPoints;
            int c;
            for (c = 0; c <= points.Count - 1; c++)
            {
                if ((p1.X - points[c].X < 0.5) && (p1.X - points[c].X > -0.5))
                {
                    break;
                }
            }

            if (c >= points.Count)
            {
                return null;
            }

            var p2 = points[c];

            var result = new TrackerHitResult
            {
                Series = this,
                DataPoint = p2,
                Position = this.Transform(p2.X, p2.Y),
                Item = this.GetItem(c),
                Index = c,
                Text =
                    StringHelper.Format(
                        this.ActualCulture,
                        this.TrackerFormatString,
                        p2,
                        this.Title,
                        this.XAxis.Title ?? DefaultXAxisTitle,
                        this.XAxis.GetValue(p2.X),
                        this.YAxis.Title ?? DefaultYAxisTitle,
                        this.YAxis.GetValue(p2.Y))
            };

            return result;
        }

        /// <summary>
        ///     Renders the series on the specified rendering context.
        /// </summary>
        /// <param name="rc">The rendering context.</param>
        public override void Render(IRenderContext rc)
        {
            if (this.ZoomLevel != ZoomLevels.Stems)
            {
                if (this.ZoomLevel == ZoomLevels.Markers)
                {
                    this.MarkerType = MarkerType.Circle;
                }
                else
                {
                    this.MarkerType = MarkerType.None;
                }
                base.Render(rc);
                return;
            }

            this.MarkerType = MarkerType.Circle;

            if (this.ActualPoints.Count == 0)
            {
                return;
            }

            this.VerifyAxes();

            var minDistSquared = this.MinimumSegmentLength * this.MinimumSegmentLength;

            var clippingRect = this.GetClippingRect();

            // Transform all points to screen coordinates
            // Render the line when invalid points occur
            var dashArray = this.ActualDashArray;
            var actualColor = this.GetSelectableColor(this.ActualColor);
            var points = new ScreenPoint[2];
            var markerPoints = this.MarkerType != MarkerType.None ? new List<ScreenPoint>(this.ActualPoints.Count) : null;
            foreach (var point in this.ActualPoints)
            {
                if (!this.IsValidPoint(point))
                {
                    continue;
                }

                points[0] = this.Transform(point.X, this._base);
                points[1] = this.Transform(point.X, point.Y);

                if ((this.StrokeThickness > 0) && (this.ActualLineStyle != LineStyle.None))
                {
                    rc.DrawClippedLine(
                        clippingRect,
                        points,
                        minDistSquared,
                        actualColor,
                        this.StrokeThickness,
                        dashArray,
                        this.LineJoin,
                        false);
                }

                markerPoints?.Add(points[1]);
            }

            if (this.MarkerType != MarkerType.None)
            {
                rc.DrawMarkers(
                    clippingRect,
                    markerPoints,
                    this.MarkerType,
                    this.MarkerOutline,
                    new[] {this.MarkerSize},
                    this.MarkerFill,
                    this.MarkerStroke,
                    this.MarkerStrokeThickness);
            }
        }

        /// <summary>
        ///     Gets or sets the source impulse response.
        /// </summary>
        /// <summary>
        ///     Sets default values of the plot model.
        /// </summary>
        protected override void SetDefaultValues()
        {
            base.SetDefaultValues();
            this.MarkerFill = this.ActualColor;
        }

        private enum ZoomLevels
        {
            Stems,
            Markers,
            Line
        }
    }
}