using DspSharp.Signal;
using PropertyTools.DataAnnotations;

namespace DspSharpDemo.SignalFactory
{
    public class DiracFactory : SignalFactory
    {
        private double _Gain = 1;

        [DisplayName("gain factor")]
        [Category("dirac settings")]    
        [SortIndex(2)]
        public double Gain
        {
            get { return this._Gain; }
            set { this.SetField(ref this._Gain, value); }
        }

        public override ISignal CreateSignal()
        {
            return new Dirac(this.SampleRate, this.Gain);
        }
    }
}