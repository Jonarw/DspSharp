using Filter.Signal;
using PropertyTools.DataAnnotations;

namespace FilterTest.SignalFactory
{
    public class IdealLowpassFactory : SignalFactory
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
            return new IdealLowpass(this.SampleRate, this.Frequency);
        }
    }
}