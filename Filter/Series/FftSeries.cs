using Filter.Algorithms;
using Filter.Helpers;

namespace Filter.Series
{
    /// <summary>
    ///     Autogenerated series representing the positive part of the frequency axis for an FFT spectrum.
    /// </summary>
    public class FftSeries : SeriesBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FftSeries" /> class.
        /// </summary>
        /// <param name="samplerate">The Samplerate used for computing the frequency values.</param>
        /// <param name="n">The FFT length N (in time domain) used for computing the frequency values.</param>
        public FftSeries(double samplerate, int n) : base(Fft.GetFrequencies(samplerate, n), false)
        {
            this.SampleRate = samplerate;
            this.N = n;
        }

        public override int Length => (this.N >> 1) + 1;

        /// <summary>
        ///     The FFT length N (in time domain) used for computing the frequency values.
        /// </summary>
        public int N { get; }

        /// <summary>
        ///     The Samplerate used for computing the frequency values.
        /// </summary>
        public double SampleRate { get; }

        /// <summary>
        ///     Compares the <see cref="FftSeries" /> to an other <see cref="ISeries" /> for equality.
        /// </summary>
        /// <param name="other">The other <see cref="ISeries" />.</param>
        /// <returns>True if the other object is a <see cref="FftSeries" /> with the same parameters, false otherwise.</returns>
        public override bool Equals(ISeries other)
        {
            if (other == null)
                return false;
            return this.Equals(other as FftSeries);
        }

        /// <summary>
        ///     Compares the <see cref="FftSeries" /> to an other <see cref="FftSeries" /> for equality.
        /// </summary>
        /// <param name="other">The other <see cref="FftSeries" />.</param>
        /// <returns>True if the other <see cref="FftSeries" /> has the same parameters, false otherwise.</returns>
        public bool Equals(FftSeries other)
        {
            if (other?.N != this.N)
                return false;
            if (other.SampleRate != this.SampleRate)
                return false;
            return true;
        }

        /// <summary>
        ///     Computes the Hashcode for the <see cref="FftSeries" />.
        /// </summary>
        /// <returns>The Hashcode.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.N, this.SampleRate);
        }
    }
}