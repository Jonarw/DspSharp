namespace FilterPlot.Axes
{
    /// <summary>
    /// Represents a group delay axis (ms).
    /// </summary>
    /// <seealso cref="DefaultAxis" />
    public class GroupDelayAxis : DefaultAxis
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDelayAxis"/> class.
        /// </summary>
        public GroupDelayAxis()
        {
            this.Title = "Group delay in ms";
            //Me.Zoom(0, 50)
        }
    }
}