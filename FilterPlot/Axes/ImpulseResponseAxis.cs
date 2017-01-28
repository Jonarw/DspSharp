namespace FilterPlot.Axes
{
    /// <summary>
    /// Represents a linear scaled axis without unit for impulse responses.
    /// </summary>
    /// <seealso cref="DefaultAxis" />
    public sealed class ImpulseResponseAxis : DefaultAxis
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpulseResponseAxis"/> class.
        /// </summary>
        public ImpulseResponseAxis()
        {
            this.Title = "value";
        }
    }
}