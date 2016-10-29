using System.Collections.Generic;
using Filter;
using Filter.Signal;
using PropertyTools.DataAnnotations;

namespace FilterTest.SignalFactory
{
    public abstract class SignalFactory : Observable
    {
        private double _SampleRate = 44100;

        [Browsable(false)]
        public IList<double> AvailableSampleRates { get; } = new List<double> {44100, 48000, 88200, 96000, 192000};

        [ItemsSourceProperty(nameof(AvailableSampleRates))]
        [DisplayName("sample rate")]
        [Category("basic settings")]
        [SortIndex(0)]
        public double SampleRate
        {
            get { return this._SampleRate; }
            set { this.SetField(ref this._SampleRate, value); }
        }

        [DisplayName("sample offset")]
        [Category("basic settings")]
        [SortIndex(1)]
        public int TimeOffset
        {
            get { return this._TimeOffset; }
            set { this.SetField(ref this._TimeOffset, value); }
        }

        private int _TimeOffset;

        public abstract ISignal CreateSignal();
    }
}