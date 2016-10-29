using System;
using System.Collections.Generic;
using System.Numerics;
using Filter.Algorithms;
using Filter.Extensions;
using Filter.Series;

namespace Filter.Spectrum
{
    public class Spectrum : ISpectrum
    {
        private IReadOnlyList<double> _groupDelay;
        private IReadOnlyList<double> _magnitude;
        private IReadOnlyList<double> _phase;

        public Spectrum(ISeries frequencies, IReadOnlyList<Complex> values)
        {
            if (frequencies.Length != values.Count)
            {
                throw new Exception();
            }

            this.Frequencies = frequencies;
            this.Values = values;
        }

        public ISeries Frequencies { get; }

        public IReadOnlyList<double> GroupDelay
        {
            get { return this._groupDelay ?? (this._groupDelay = Dsp.CalculateGroupDelay(this.Phase, this.Frequencies.Values).ToReadOnlyList()); }
        }

        public IReadOnlyList<double> Magnitude
        {
            get { return this._magnitude ?? (this._magnitude = this.Values.Magitude().ToReadOnlyList()); }
        }

        public IReadOnlyList<double> Phase
        {
            get { return this._phase ?? (this._phase = this.Values.Phase().ToReadOnlyList()); }
        }

        public IReadOnlyList<Complex> Values { get; }
    }
}