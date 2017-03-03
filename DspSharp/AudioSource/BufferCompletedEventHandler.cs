// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferCompletedEventHandler.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DspSharp.AudioSource
{
    public delegate void BufferCompletedEventHandler(IAudioSource sender, BufferCompletedEventArgs e);
}