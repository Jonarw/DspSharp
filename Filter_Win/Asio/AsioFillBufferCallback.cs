using System;

namespace FilterWin.Asio
{
    /// <summary>
    ///     Callback used by the AsioDriverExt to get wave data
    /// </summary>
    public delegate void AsioFillBufferCallback(IntPtr[] inputChannels, IntPtr[] outputChannels);
}