using System;
using System.Collections.Generic;
using System.Numerics;
using Filter.Algorithms;
using Filter.Extensions;
using Filter.Series;
using Filter.Spectrum;

namespace Filter.Signal
{
    public class FiniteSignal : IFiniteSignal
    {
        private IFftSpectrum _spectrum;
        private IReadOnlyList<double> _signal;

        public FiniteSignal(IReadOnlyList<double> signal, double sampleRate, int start = 0)
        {
            this.Signal = signal;
            this.SampleRate = sampleRate;
            this.Start = start;
            this.Length = signal.Count;
            this.Stop = start + signal.Count;
            this.Frequencies = new FftSeries(sampleRate, this.Length);
        }

        public FiniteSignal(IFftSpectrum spectrum, int start = 0)
        {
            this.SampleRate = spectrum.Frequencies.SampleRate;
            this.Spectrum = spectrum;
            this.Start = start;
            this.Length = spectrum.Frequencies.N;
            this.Stop = this.Start + this.Length;
        }

        public int MinFftLength { get; set; } = 128;

        public FftSeries Frequencies { get; }

        public IFftSpectrum Spectrum
        {
            get
            {
                if (this._spectrum == null)
                {
                    this._spectrum = new FftSpectrum(this.Signal, Math.Max(this.Length, this.MinFftLength), this.SampleRate, this.Start);
                }

                return this._spectrum;
            }

            private set { this._spectrum = value; }
        }

        public int Start { get; }

        public IFftSpectrum GetSpectrum(int fftLength)
        {
            return new FftSpectrum(this.Signal, fftLength, this.SampleRate, this.Start);
        }

        IEnumerable<double> IEnumerableSignal.Signal => this.Signal;

        public double GetSample(int time)
        {
            if ((time < this.Start) || (time >= this.Stop))
            {
                return 0.0;
            }

            return this.Signal[time - this.Start];
        }

        public int Length { get; }

        public IReadOnlyList<double> Signal
        {
            get
            {
                if (this._signal == null)
                {
                    this._signal = this.Spectrum.GetTimeDomainSignal();
                }

                return this._signal;
            }

            private set { this._signal = value; }
        }

        public int Stop { get; }

        public IEnumerable<double> GetWindowedSignal(int start, int length)
        {
            return this.Signal.GetPaddedRange(start - this.Start, length);
        }

        public double SampleRate { get; }
        public string Name { get; set; } = "finite signal";
    }
}