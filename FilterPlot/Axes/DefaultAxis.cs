using OxyPlot;
using OxyPlot.Axes;

namespace FilterPlot.Axes
{
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