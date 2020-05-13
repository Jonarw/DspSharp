// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiracFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Signal;

namespace DspSharpDemo.ViewModel.Signal.SignalFactories
{
    public class DiracFactory : SignalFactory
    {
        private double _Gain = 1;

        public DiracFactory(double sampleRate) : base(sampleRate)
        {
        }

        public double Gain
        {
            get { return this._Gain; }
            set { this.SetField(ref this._Gain, value); }
        }

        public override ISignal CreateItem()
        {
            return new Dirac(this.SampleRate, this.Gain);
        }
    }
}