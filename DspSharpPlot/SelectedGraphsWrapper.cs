using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using UTilities.Collections;
using UTilities.Observable;
using LineSeries = OxyPlot.Series.LineSeries;

namespace DspSharpPlot
{
    public class LineSeriesWrapper : Observable
    {
        private Color? _Background;
        private Color? _BrokenLineColor;
        private double? _BrokenLineThickness;
        private bool? _CanTrackerInterpolatePoints;
        private Color? _Color;
        private bool? _IsVisible;
        private string _LabelFormatString;
        private double? _LabelMargin;
        private LineJoin _LineJoin;
        private LineLegendPosition _LineLegendPosition;
        private LineStyle _LineStyle;
        private Color? _MarkerFill;
        private int? _MarkerResolution;
        private double? _MarkerSize;
        private Color? _MarkerStroke;
        private double? _MarkerStrokeThickness;
        private MarkerType _MarkerType;
        private double? _MinimumSegmentLength;
        private bool? _RenderInLegend;
        private double? _StrokeThickness;
        private object _Tag;
        private string _Title;
        private string _TrackerFormatString;
        private string _TrackerKey;
        private string _XAxisKey;
        private string _YAxisKey;

        public LineSeriesWrapper(IReadOnlyObservableList<LineSeries> graphs)
        {
            this.TargetGraphs = graphs;
            this.TargetGraphs.CollectionChanged += this.TargetGraphsOnCollectionChanged;
        }

        public Color? Background
        {
            get => this._Background;
            set => this.SetField(ref this._Background, value, () => this.DoForAllTargetGraphs(s => s.Background = this.Background.Value.ToOxyColor()));
        }

        public Color? BrokenLineColor
        {
            get => this._BrokenLineColor;
            set => this.SetField(
                ref this._BrokenLineColor,
                value,
                () => this.DoForAllTargetGraphs(s => s.BrokenLineColor = this.BrokenLineColor.Value.ToOxyColor()));
        }

        public double? BrokenLineThickness
        {
            get => this._BrokenLineThickness;
            set => this.SetField(ref this._BrokenLineThickness, value, () => this.DoForAllTargetGraphs(s => s.BrokenLineThickness = this.BrokenLineThickness.Value));
        }

        public bool? CanTrackerInterpolatePoints
        {
            get => this._CanTrackerInterpolatePoints;
            set => this.SetField(
                ref this._CanTrackerInterpolatePoints,
                value,
                () => this.DoForAllTargetGraphs(s => s.CanTrackerInterpolatePoints = this.CanTrackerInterpolatePoints.Value));
        }

        public Color? Color
        {
            get => this._Color;
            set
            {
                this.SetField(ref this._Color, value, () => this.DoForAllTargetGraphs(s => s.Color = this.Color.Value.ToOxyColor()));
                this.TitleOrVisibleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool? IsVisible
        {
            get => this._IsVisible;
            set => this.SetField(ref this._IsVisible, value, () =>
            {
                this.DoForAllTargetGraphs(s => s.IsVisible = this.IsVisible.Value);
                this.TitleOrVisibleChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        public string LabelFormatString
        {
            get => this._LabelFormatString;
            set => this.SetField(ref this._LabelFormatString, value, () => this.DoForAllTargetGraphs(s => s.LabelFormatString = this.LabelFormatString));
        }

        public double? LabelMargin
        {
            get => this._LabelMargin;
            set => this.SetField(ref this._LabelMargin, value, () => this.DoForAllTargetGraphs(s => s.LabelMargin = this.LabelMargin.Value));
        }

        public LineJoin LineJoin
        {
            get => this._LineJoin;
            set => this.SetField(ref this._LineJoin, value, () => this.DoForAllTargetGraphs(s => s.LineJoin = this.LineJoin));
        }

        public LineLegendPosition LineLegendPosition
        {
            get => this._LineLegendPosition;
            set => this.SetField(ref this._LineLegendPosition, value, () => this.DoForAllTargetGraphs(s => s.LineLegendPosition = this.LineLegendPosition));
        }

        public LineStyle LineStyle
        {
            get => this._LineStyle;
            set => this.SetField(ref this._LineStyle, value, () => this.DoForAllTargetGraphs(s => s.LineStyle = this.LineStyle));
        }

        public Color? MarkerFill
        {
            get => this._MarkerFill;
            set => this.SetField(ref this._MarkerFill, value, () => this.DoForAllTargetGraphs(s => s.MarkerFill = this.MarkerFill.Value.ToOxyColor()));
        }

        public int? MarkerResolution
        {
            get => this._MarkerResolution;
            set => this.SetField(ref this._MarkerResolution, value, () => this.DoForAllTargetGraphs(s => s.MarkerResolution = this.MarkerResolution.Value));
        }

        public double? MarkerSize
        {
            get => this._MarkerSize;
            set => this.SetField(ref this._MarkerSize, value, () => this.DoForAllTargetGraphs(s => s.MarkerSize = this.MarkerSize.Value));
        }

        public Color? MarkerStroke
        {
            get => this._MarkerStroke;
            set => this.SetField(ref this._MarkerStroke, value, () => this.DoForAllTargetGraphs(s => s.MarkerStroke = this.MarkerStroke.Value.ToOxyColor()));
        }

        public double? MarkerStrokeThickness
        {
            get => this._MarkerStrokeThickness;
            set => this.SetField(ref this._MarkerStrokeThickness, value, () => this.DoForAllTargetGraphs(s => s.MarkerStrokeThickness = this.MarkerStrokeThickness.Value));
        }

        public MarkerType MarkerType
        {
            get => this._MarkerType;
            set => this.SetField(ref this._MarkerType, value, () => this.DoForAllTargetGraphs(s => s.MarkerType = this.MarkerType));
        }

        public double? MinimumSegmentLength
        {
            get => this._MinimumSegmentLength;
            set => this.SetField(ref this._MinimumSegmentLength, value, () => this.DoForAllTargetGraphs(s => s.MinimumSegmentLength = this.MinimumSegmentLength.Value));
        }

        public bool? RenderInLegend
        {
            get => this._RenderInLegend;
            set => this.SetField(ref this._RenderInLegend, value, () => this.DoForAllTargetGraphs(s => s.RenderInLegend = this.RenderInLegend.Value));
        }

        public double? StrokeThickness
        {
            get => this._StrokeThickness;
            set => this.SetField(ref this._StrokeThickness, value, () => this.DoForAllTargetGraphs(s => s.StrokeThickness = this.StrokeThickness.Value));
        }

        public object Tag
        {
            get => this._Tag;
            set => this.SetField(ref this._Tag, value, () => this.DoForAllTargetGraphs(s => s.Tag = this.Tag));
        }

        public IReadOnlyObservableList<LineSeries> TargetGraphs { get; }

        public string Title
        {
            get => this._Title;
            set => this.SetField(ref this._Title, value, () =>
            {
                this.DoForAllTargetGraphs(s => s.Title = this.Title);
                this.TitleOrVisibleChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        public string TrackerFormatString
        {
            get => this._TrackerFormatString;
            set => this.SetField(ref this._TrackerFormatString, value, () => this.DoForAllTargetGraphs(s => s.TrackerFormatString = this.TrackerFormatString));
        }

        public string TrackerKey
        {
            get => this._TrackerKey;
            set => this.SetField(ref this._TrackerKey, value, () => this.DoForAllTargetGraphs(s => s.TrackerKey = this.TrackerKey));
        }

        public string XAxisKey
        {
            get => this._XAxisKey;
            set => this.SetField(ref this._XAxisKey, value, () => this.DoForAllTargetGraphs(s => s.XAxisKey = this.XAxisKey));
        }

        public string YAxisKey
        {
            get => this._YAxisKey;
            set => this.SetField(ref this._YAxisKey, value, () => this.DoForAllTargetGraphs(s => s.YAxisKey = this.YAxisKey));
        }

        public event EventHandler TitleOrVisibleChanged;

        private void DoForAllTargetGraphs(Action<LineSeries> action)
        {
            foreach (var selectedGraph in this.TargetGraphs)
                action(selectedGraph);
        }

        private void TargetGraphsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var backgrounds = this.TargetGraphs.Select(g => g.Background).Distinct().ToList();
            this.SetField(ref this._Background, backgrounds.Count == 1 ? backgrounds[0].ToColor() : (Color?)null, nameof(this.Background));

            var brokenLineColors = this.TargetGraphs.Select(g => g.BrokenLineColor).Distinct().ToList();
            this.SetField(ref this._BrokenLineColor, brokenLineColors.Count == 1 ? brokenLineColors[0].ToColor() : (Color?)null, nameof(this.BrokenLineColor));

            var brokenLineThicknesss = this.TargetGraphs.Select(g => g.BrokenLineThickness).Distinct().ToList();
            this.SetField(ref this._BrokenLineThickness, brokenLineThicknesss.Count == 1 ? brokenLineThicknesss[0] : (double?)null, nameof(this.BrokenLineThickness));

            var canTrackerInterpolatePointss = this.TargetGraphs.Select(g => g.CanTrackerInterpolatePoints).Distinct().ToList();
            this.SetField(
                ref this._CanTrackerInterpolatePoints,
                canTrackerInterpolatePointss.Count == 1 ? canTrackerInterpolatePointss[0] : (bool?)null,
                nameof(this.CanTrackerInterpolatePoints));

            var colors = this.TargetGraphs.Select(g => g.Color).Distinct().ToList();
            this.SetField(ref this._Color, colors.Count == 1 ? colors[0].ToColor() : (Color?)null, nameof(this.Color));

            var isVisibles = this.TargetGraphs.Select(g => g.IsVisible).Distinct().ToList();
            this.SetField(ref this._IsVisible, isVisibles.Count == 1 ? isVisibles[0] : (bool?)null, nameof(this.IsVisible));

            var labelFormatStrings = this.TargetGraphs.Select(g => g.LabelFormatString).Distinct().ToList();
            this.SetField(ref this._LabelFormatString, labelFormatStrings.Count == 1 ? labelFormatStrings[0] : "[multiple]", nameof(this.LabelFormatString));

            var labelMargins = this.TargetGraphs.Select(g => g.LabelMargin).Distinct().ToList();
            this.SetField(ref this._LabelMargin, labelMargins.Count == 1 ? labelMargins[0] : (double?)null, nameof(this.LabelMargin));

            var lineJoins = this.TargetGraphs.Select(g => g.LineJoin).Distinct().ToList();
            this.SetField(ref this._LineJoin, lineJoins.Count == 1 ? lineJoins[0] : (LineJoin)(-1), nameof(this.LineJoin));

            var lineLegendPositions = this.TargetGraphs.Select(g => g.LineLegendPosition).Distinct().ToList();
            this.SetField(ref this._LineLegendPosition, lineLegendPositions.Count == 1 ? lineLegendPositions[0] : (LineLegendPosition)(-1), nameof(this.LineLegendPosition));

            var lineStyles = this.TargetGraphs.Select(g => g.LineStyle).Distinct().ToList();
            this.SetField(ref this._LineStyle, lineStyles.Count == 1 ? lineStyles[0] : (LineStyle)(-1), nameof(this.LineStyle));

            var markerFills = this.TargetGraphs.Select(g => g.MarkerFill).Distinct().ToList();
            this.SetField(ref this._MarkerFill, markerFills.Count == 1 ? markerFills[0].ToColor() : (Color?)null, nameof(this.MarkerFill));

            var markerResolutions = this.TargetGraphs.Select(g => g.MarkerResolution).Distinct().ToList();
            this.SetField(ref this._MarkerResolution, markerResolutions.Count == 1 ? markerResolutions[0] : (int?)null, nameof(this.MarkerResolution));

            var markerSizes = this.TargetGraphs.Select(g => g.MarkerSize).Distinct().ToList();
            this.SetField(ref this._MarkerSize, markerSizes.Count == 1 ? markerSizes[0] : (double?)null, nameof(this.MarkerSize));

            var markerStrokes = this.TargetGraphs.Select(g => g.MarkerStroke).Distinct().ToList();
            this.SetField(ref this._MarkerStroke, markerStrokes.Count == 1 ? markerStrokes[0].ToColor() : (Color?)null, nameof(this.MarkerStroke));

            var markerStrokeThicknesss = this.TargetGraphs.Select(g => g.MarkerStrokeThickness).Distinct().ToList();
            this.SetField(ref this._MarkerStrokeThickness, markerStrokeThicknesss.Count == 1 ? markerStrokeThicknesss[0] : (double?)null, nameof(this.MarkerStrokeThickness));

            var markerTypes = this.TargetGraphs.Select(g => g.MarkerType).Distinct().ToList();
            this.SetField(ref this._MarkerType, markerTypes.Count == 1 ? markerTypes[0] : (MarkerType)(-1), nameof(this.MarkerType));

            var minimumSegmentLengths = this.TargetGraphs.Select(g => g.MinimumSegmentLength).Distinct().ToList();
            this.SetField(ref this._MinimumSegmentLength, minimumSegmentLengths.Count == 1 ? minimumSegmentLengths[0] : (double?)null, nameof(this.MinimumSegmentLength));

            var renderInLegends = this.TargetGraphs.Select(g => g.RenderInLegend).Distinct().ToList();
            this.SetField(ref this._RenderInLegend, renderInLegends.Count == 1 ? renderInLegends[0] : (bool?)null, nameof(this.RenderInLegend));

            var strokeThicknesss = this.TargetGraphs.Select(g => g.StrokeThickness).Distinct().ToList();
            this.SetField(ref this._StrokeThickness, strokeThicknesss.Count == 1 ? strokeThicknesss[0] : (double?)null, nameof(this.StrokeThickness));

            var tags = this.TargetGraphs.Select(g => g.Tag).Distinct().ToList();
            this.SetField(ref this._Tag, tags.Count == 1 ? tags[0] : "[multiple]", nameof(this.Tag));

            var titles = this.TargetGraphs.Select(g => g.Title).Distinct().ToList();
            this.SetField(ref this._Title, titles.Count == 1 ? titles[0] : "[multiple]", nameof(this.Title));

            var trackerFormatStrings = this.TargetGraphs.Select(g => g.TrackerFormatString).Distinct().ToList();
            this.SetField(ref this._TrackerFormatString, trackerFormatStrings.Count == 1 ? trackerFormatStrings[0] : "[multiple]", nameof(this.TrackerFormatString));

            var trackerKeys = this.TargetGraphs.Select(g => g.TrackerKey).Distinct().ToList();
            this.SetField(ref this._TrackerKey, trackerKeys.Count == 1 ? trackerKeys[0] : "[multiple]", nameof(this.TrackerKey));

            var xAxisKeys = this.TargetGraphs.Select(g => g.XAxisKey).Distinct().ToList();
            this.SetField(ref this._XAxisKey, xAxisKeys.Count == 1 ? xAxisKeys[0] : "[multiple]", nameof(this.XAxisKey));

            var yAxisKeys = this.TargetGraphs.Select(g => g.YAxisKey).Distinct().ToList();
            this.SetField(ref this._YAxisKey, yAxisKeys.Count == 1 ? yAxisKeys[0] : "[multiple]", nameof(this.YAxisKey));
        }
    }
}