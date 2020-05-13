// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileFrequencyResponseFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using DspSharp.Algorithms;
using DspSharp.Series;
using DspSharp.Signal;
using DspSharp.Spectrum;
using UmtUtilities.Observable.DataAnnotations;
using UTilities.Observable.DataAnnotations;

namespace DspSharpDemo.ViewModel.Signal.SignalFactories
{
    public class FileFrequencyResponseFactory : SignalFactory
    {
        private string _FileName;
        private int _SignalLength;

        public FileFrequencyResponseFactory(double sampleRate) : base(sampleRate)
        {
            this.SignalLength = (int)sampleRate;
        }

        [NotNull]
        public string FileName
        {
            get { return this._FileName; }
            set { this.SetField(ref this._FileName, value); }
        }

        [GreaterThan(0)]
        public int SignalLength
        {
            get { return this._SignalLength; }
            set { this.SetField(ref this._SignalLength, value); }
        }

        public char Separator
        {
            get { return this._Separator; }
            set { this.SetField(ref this._Separator, value); }
        }

        private char _Separator = ';';

        public override ISignal CreateItem()
        {
            var fi = new FileInfo(this.FileName);
            if (!fi.Exists)
                return null;

            var file = File.ReadLines(this.FileName);

            var frequencies = new List<double>();
            var values = new List<Complex>();

            double frequency;
            double mag;
            double phase;

            foreach (var line in file)
            {
                var fields = line.Split(this.Separator);

                if (fields.Length < 2)
                    continue;

                if (!double.TryParse(fields[0].Replace(',', '.').Replace(" ", string.Empty), NumberStyles.Any, CultureInfo.InvariantCulture, out frequency))
                    continue;

                if (!double.TryParse(fields[1].Replace(',', '.').Replace(" ", string.Empty), NumberStyles.Any, CultureInfo.InvariantCulture, out mag))
                    continue;

                if (fields.Length == 2)
                {
                    frequencies.Add(frequency);
                    values.Add(FrequencyDomain.DbToLinear(mag));
                }
                else
                {
                    if (fields.Length == 3)
                    {
                        if (!double.TryParse(fields[1], NumberStyles.Any, CultureInfo.InvariantCulture, out phase))
                            continue;

                        frequencies.Add(frequency);
                        values.Add(Complex.FromPolarCoordinates(FrequencyDomain.DbToLinear(mag), phase));
                    }
                }
            }

            var series = new FftSeries(this.SampleRate, this.SignalLength);
            var ret = Interpolation.AdaptiveInterpolation(frequencies, values, series.Values.ToReadOnlyList(), false, false).ToReadOnlyList();

            return new FiniteSignal(new FftSpectrum(series, ret), this.TimeOffset);
        }
    }
}