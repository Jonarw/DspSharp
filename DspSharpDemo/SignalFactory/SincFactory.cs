using DspSharp.Signal;
using PropertyTools.DataAnnotations;

namespace DspSharpDemo.SignalFactory
{
    public class SincFactory : SignalFactory
    {
        private double _Frequency = 1000;

        [DisplayName("frequency [Hz]")]
        [Category("sinc settings")]
        [SortIndex(2)]
        public double Frequency
        {
            get { return this._Frequency; }
            set { this.SetField(ref this._Frequency, value); }
        }

        public override ISignal CreateSignal()
        {
            return new Sinc(this.SampleRate, this.Frequency);
        }
    }
}