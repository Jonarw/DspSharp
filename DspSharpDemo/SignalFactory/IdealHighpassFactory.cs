using DspSharp.Signal;
using PropertyTools.DataAnnotations;

namespace DspSharpDemo.SignalFactory
{
    public class IdealHighpassFactory : SignalFactory
    {
        private double _Frequency = 1000;

        [DisplayName("cutoff frequency [Hz]")]
        [Category("ideal highpass settings")]    
        [SortIndex(2)]
        public double Frequency
        {
            get { return this._Frequency; }
            set { this.SetField(ref this._Frequency, value); }
        }

        public override ISignal CreateSignal()
        {
            return new IdealHighpass(this.SampleRate, this.Frequency);
        }
    }
}