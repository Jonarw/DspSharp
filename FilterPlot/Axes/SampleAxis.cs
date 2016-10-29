namespace FilterPlot.Axes
{
    /// <summary>
    /// Represents a sample axis.
    /// </summary>
    /// <seealso cref="DefaultAxis" />
    public sealed class SampleAxis : DefaultAxis
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleAxis"/> class.
        /// </summary>
        public SampleAxis()
        {
            this.Title = "time [samples]";
            this.MinimumMajorStep = 1;
            this.MinimumMinorStep = 1;
            this.Minimum = 0;
            this.Maximum = 1024;
        }
    }
}