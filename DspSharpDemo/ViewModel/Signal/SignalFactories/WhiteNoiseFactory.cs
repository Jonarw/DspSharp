// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhiteNoiseFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Signal;

namespace DspSharpDemo.ViewModel.Signal.SignalFactories
{
    public class WhiteNoiseFactory : SignalFactory
    {
        private double _Mean;
        private double _Variance = 1;

        public WhiteNoiseFactory(double sampleRate) : base(sampleRate)
        {
        }

        public double Mean
        {
            get { return this._Mean; }
            set { this.SetField(ref this._Mean, value); }
        }

        public double Variance
        {
            get { return this._Variance; }
            set { this.SetField(ref this._Variance, value); }
        }

        public override ISignal CreateItem()
        {
            return new WhiteNoise(this.SampleRate, this.Mean, this.Variance);
        }
    }
}