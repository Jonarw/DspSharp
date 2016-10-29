namespace Filter.Signal.Windows
{
    /// <summary>
    ///     Enumeration of all supported window types.
    /// </summary>
    public enum WindowTypes
    {
        /// <summary>
        ///     The rectangular window.
        /// </summary>
        Rectangular,

        /// <summary>
        ///     The Hann window.
        /// </summary>
        Hann,

        /// <summary>
        ///     The Hamming window.
        /// </summary>
        Hamming,

        /// <summary>
        ///     The triangular window.
        /// </summary>
        Triangular,

        /// <summary>
        ///     The Welch window.
        /// </summary>
        Welch,

        /// <summary>
        ///     The Blackman window.
        /// </summary>
        Blackman,

        /// <summary>
        ///     The Blackman-Harris window.
        /// </summary>
        BlackmanHarris,

        /// <summary>
        ///     The Kaiser window with alpha=2.
        /// </summary>
        KaiserAlpha2,

        /// <summary>
        ///     The Kaiser window with alpha=3.
        /// </summary>
        KaiserAlpha3
    }
}