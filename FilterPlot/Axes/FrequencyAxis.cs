
namespace FilterPlot.Axes
{
    using OxyPlot;
/// <summary>
    /// Extends <see cref="OxyPlot.Axes.LogarithmicAxis"/> for custom value initializations.
    /// </summary>
    /// <seealso cref="OxyPlot.Axes.LogarithmicAxis" />
    public class FrequencyAxis : OxyPlot.Axes.LogarithmicAxis
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrequencyAxis"/> class.
        /// </summary>
        public FrequencyAxis()
        {
            this.MajorGridlineStyle = LineStyle.Solid;
            this.Title = "frequency [Hz]";
            this.MinorGridlineStyle = LineStyle.Solid;
            this.Base = 10;
            //Me.IsZoomEnabled = False
            //Me.IsPanEnabled = False
        }
    }
}