// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SincFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Signal;

namespace DspSharpDemo.ViewModel.Signal.SignalFactories
{
    public class SincFactory : SignalFactory
    {
        private double _Frequency = 1000;

        public SincFactory(double sampleRate) : base(sampleRate)
        {
        }

        public double Frequency
        {
            get { return this._Frequency; }
            set { this.SetField(ref this._Frequency, value); }
        }

        public override ISignal CreateItem()
        {
            return new Sinc(this.SampleRate, this.Frequency);
        }
    }
}