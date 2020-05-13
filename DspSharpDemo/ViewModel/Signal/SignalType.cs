// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvailableSignals.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;

namespace DspSharpDemo.ViewModel.Signal
{
    public enum SignalType
    {
        [Description("Sine")] Sine,
        [Description("Sinc")] Sinc,
        [Description("White Noise")] WhiteNoise,
        [Description("Ideal Highpass")] IdealHighpass,
        [Description("Ideal Lowpass")] IdealLowpass,
        [Description("Dirac")] Dirac,
        [Description("Exponential Sine Sweep")] LogSweep,
        [Description("Window Function")] Window,
        [Description("Impulse Response From File")] FileImpulseResponse,
        [Description("Frequency Response From File")] FileFrequencyResponse
    }
}