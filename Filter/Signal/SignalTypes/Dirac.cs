using Filter.Extensions;

namespace Filter.Signal
{
    /// <summary>
    ///     Represents a scaled dirac pulse.
    /// </summary>
    public class Dirac : FiniteSignal
    {
        public Dirac(double sampleRate, double gain = 1) : base(new[] {gain}.ToReadOnlyList(), sampleRate)
        {
            this.Gain = gain;
            this.Name = "dirac, gain = " + gain;
        }

        public double Gain { get; }
    }
}