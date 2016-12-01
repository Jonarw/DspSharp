namespace Filter.Signal.Windows
{
    /// <summary>
    ///     Enumerates the available window modes.
    /// </summary>
    public enum WindowModes
    {
        /// <summary>
        ///     Symmetric window (starting with 0, then rising towards 1, then declining towards 0)
        /// </summary>
        Symmetric,

        /// <summary>
        ///     Causal window (starting with 1, then declining).
        /// </summary>
        Causal,

        /// <summary>
        ///     Anti-causal window (starting with 0, then rising towards 1)
        /// </summary>
        AntiCausal
    }
}