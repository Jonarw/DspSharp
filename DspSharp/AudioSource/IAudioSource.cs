// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAudioSource.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DspSharp.AudioSource
{
    public interface IAudioSource
    {
        int BlockSize { get; }
        event BufferCompletedEventHandler BufferCompleted;
    }
}