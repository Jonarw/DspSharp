using DspSharp.Signal;
using PropertyTools.DataAnnotations;

namespace DspSharpDemo.SignalFactory
{
    public class LogSweepFactory : SignalFactory
    {
        private double _Length = 1;
        private double _StartFrequency = 20;
        private double _StopFrequency = 20000;

        [DisplayName("length [seconds]")]
        [Category("logsweep settings")]
        [SortIndex(4)]
        public double Length
        {
            get { return this._Length; }
            set { this.SetField(ref this._Length, value); }
        }

        [DisplayName("start frequency [Hz]")]
        [Category("logsweep settings")]
        [SortIndex(2)]
        public double StartFrequency
        {
            get { return this._StartFrequency; }
            set { this.SetField(ref this._StartFrequency, value); }
        }

        [DisplayName("stop frequency [Hz]")]
        [Category("logsweep settings")]
        [SortIndex(3)]
        public double StopFrequency
        {
            get { return this._StopFrequency; }
            set { this.SetField(ref this._StopFrequency, value); }
        }

        public override ISignal CreateSignal()
        {
            return new LogSweep(this.StartFrequency, this.StopFrequency, this.Length, this.SampleRate);
        }
    }
}