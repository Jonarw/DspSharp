// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsioFillBufferCallback.cs">
//   Copyright (c) 2017 NAudio contributors, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace DspSharpAsio
{
    /// <summary>
    ///     Callback used by the AsioDriverExt to get wave data
    /// </summary>
    public delegate void AsioFillBufferCallback(IntPtr[] inputChannels, IntPtr[] outputChannels);
}