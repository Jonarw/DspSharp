using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using Filter.Algorithms;
using Filter.Extensions;
using Filter.Series;
using Filter.Signal;
using Filter.Spectrum;
using PropertyTools.DataAnnotations;

namespace FilterTest.SignalFactory
{
    public class FileFrequencyResponseFactory : SignalFactory
    {
        private string _FileName;

        private int _SignalLength;

        public override ISignal CreateSignal()
        {
            var fi = new FileInfo(this.FileName);
            if (!fi.Exists)
            {
                return null;
            }

            var file = File.ReadLines(this.FileName);

            var frequencies = new List<double>();
            var values = new List<Complex>();

            double frequency;
            double mag;
            double phase;

            foreach (var line in file)
            {
                var fields = line.Split(',');

                if (fields.Length < 2)
                {
                    continue;
                }

                if (!double.TryParse(fields[0], NumberStyles.Any, CultureInfo.InvariantCulture, out frequency))
                {
                    continue;
                }

                if (!double.TryParse(fields[1], NumberStyles.Any, CultureInfo.InvariantCulture, out mag))
                {
                    continue;
                }

                if (fields.Length == 2)
                {
                    frequencies.Add(frequency);
                    values.Add(mag);
                }
                else if (fields.Length == 3)
                {
                    if (!double.TryParse(fields[1], NumberStyles.Any, CultureInfo.InvariantCulture, out phase))
                    {
                        continue;
                    }

                    frequencies.Add(frequency);
                    values.Add(Complex.FromPolarCoordinates(mag, phase));
                }
            }

            var series = new FftSeries(this.SampleRate, this.SignalLength);
            var ret = Dsp.AdaptiveInterpolation(frequencies, values, series.Values.ToReadOnlyList(), false).ToReadOnlyList();

            return new FiniteSignal(new FftSpectrum(series, ret), this.TimeOffset);
        }

        [DisplayName("frequency response path")]
        [InputFilePath("txt", "*.txt-files|*.txt|*.csv-files|*.csv")]
        public string FileName
        {
            get { return this._FileName; }
            set { this.SetField(ref this._FileName, value); }
        }

        [DisplayName("signal time domain length")]
        public int SignalLength
        {
            get { return this._SignalLength; }
            set { this.SetField(ref this._SignalLength, value); }
        }
    }
}