
using System.ComponentModel;

namespace FilterTest.SignalFactory
{
    public enum AvailableSignals
    {
        [Description("sine wave")] Sine,
        [Description("sinc pulse")] Sinc,
        [Description("white noise")] WhiteNoise,
        [Description("ideal highpass")] IdealHighpass,
        [Description("ideal lowpass")] IdealLowpass,
        [Description("dirac pulse")] Dirac,
        [Description("logarithmic sine sweep")] LogSweep,
        [Description("window function")] Window,
        [Description("impulse response from file")] FileImpulseResponse,
        [Description("frequency response from file")] FileFrequencyResponse
    }
}