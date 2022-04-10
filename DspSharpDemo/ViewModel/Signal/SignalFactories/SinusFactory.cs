// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SinusFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Signal;

namespace DspSharpDemo.ViewModel.Signal.SignalFactories
{
    public class SinusFactory : SignalFactory
    {
        private double _Frequency = 1000;
        private double _Phase;

        public SinusFactory(double sampleRate) : base(sampleRate)
        {
        }

        public double Frequency
        {
            get { return this._Frequency; }
            set { this.SetField(ref this._Frequency, value); }
        }

        public double Phase
        {
            get { return this._Phase; }
            set { this.SetField(ref this._Phase, value); }
        }

        public override ISignal CreateItem()
        {
            return new Sine(this.SampleRate, this.Frequency, this.Phase);
        }
    }
}