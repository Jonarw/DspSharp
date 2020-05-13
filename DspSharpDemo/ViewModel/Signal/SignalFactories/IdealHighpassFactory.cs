// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdealHighpassFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Signal;
using UTilities.Observable.DataAnnotations;

namespace DspSharpDemo.ViewModel.Signal.SignalFactories
{
    public class IdealHighpassFactory : SignalFactory
    {
        private double _Frequency = 1000;

        public IdealHighpassFactory(double sampleRate) : base(sampleRate)
        {
        }

        [GreaterThan(0)]
        public double Frequency
        {
            get { return this._Frequency; }
            set { this.SetField(ref this._Frequency, value); }
        }

        public override ISignal CreateItem()
        {
            return new IdealHighpass(this.SampleRate, this.Frequency);
        }
    }
}