namespace FilterWin.Fft
{
    /// <summary>
    ///     Defines direction of operation
    /// </summary>
    public enum FftwDirection
    {
        /// <summary>
        ///     Computes a regular DFT
        /// </summary>
        Forward = -1,

        /// <summary>
        ///     Computes the inverse DFT
        /// </summary>
        Backward = 1
    }
}