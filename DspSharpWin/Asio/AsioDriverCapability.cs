// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsioDriverCapability.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DspSharpWin.Asio
{
    /// <summary>
    ///     ASIODriverCapability holds all the information from the AsioDriver.
    ///     Use ASIODriverExt to get the Capabilities
    /// </summary>
    public class AsioDriverCapability
    {
        /// <summary>
        ///     Buffer Granularity
        /// </summary>
        public int BufferGranularity;

        /// <summary>
        ///     Buffer Maximum Size
        /// </summary>
        public int BufferMaxSize;

        /// <summary>
        ///     Buffer Minimum Size
        /// </summary>
        public int BufferMinSize;

        /// <summary>
        ///     Buffer Preferred Size
        /// </summary>
        public int BufferPreferredSize;

        /// <summary>
        ///     Drive Name
        /// </summary>
        public string DriverName;

        /// <summary>
        ///     Input Channel Info
        /// </summary>
        public AsioChannelInfo[] InputChannelInfos;

        /// <summary>
        ///     Input Latency
        /// </summary>
        public int InputLatency;

        /// <summary>
        ///     Number of Input Channels
        /// </summary>
        public int NbInputChannels;

        /// <summary>
        ///     Number of Output Channels
        /// </summary>
        public int NbOutputChannels;

        /// <summary>
        ///     Output Channel Info
        /// </summary>
        public AsioChannelInfo[] OutputChannelInfos;

        /// <summary>
        ///     Output Latency
        /// </summary>
        public int OutputLatency;

        /// <summary>
        ///     Sample Rate
        /// </summary>
        public double SampleRate;
    }
}