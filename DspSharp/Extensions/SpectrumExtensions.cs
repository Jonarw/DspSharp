using System;
using DspSharp.Algorithms;
using DspSharp.Spectrum;

namespace DspSharp.Extensions
{
    public static class SpectrumExtensions
    {
        public static IFftSpectrum Multiply(this IFftSpectrum spectrum1, IFftSpectrum spectrum2)
        {
            if (spectrum1 == null)
                throw new ArgumentNullException(nameof(spectrum1));
            if (spectrum2 == null)
                throw new ArgumentNullException(nameof(spectrum2));
            if (!spectrum1.Frequencies.Equals(spectrum2.Frequencies))
                throw new ArgumentException();

            return new FftSpectrum(spectrum1.Frequencies, spectrum1.Values.Multiply(spectrum2.Values).ToReadOnlyList());
        }

        public static IFftSpectrum Divide(this IFftSpectrum spectrum1, IFftSpectrum spectrum2)
        {   
            if (spectrum1 == null)
                throw new ArgumentNullException(nameof(spectrum1));
            if (spectrum2 == null)
                throw new ArgumentNullException(nameof(spectrum2));
            if (!spectrum1.Frequencies.Equals(spectrum2.Frequencies))
                throw new ArgumentException();

            return new FftSpectrum(spectrum1.Frequencies, spectrum1.Values.Divide(spectrum2.Values).ToReadOnlyList());
        }
    }
}