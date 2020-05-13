// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogSweepFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Signal;

namespace DspSharpDemo.ViewModel.Signal.SignalFactories
{
    public class LogSweepFactory : SignalFactory
    {
        private double _Length = 1;
        private double _StartFrequency = 20;
        private double _StopFrequency = 20000;

        public LogSweepFactory(double sampleRate) : base(sampleRate)
        {
        }

        public double Length
        {
            get { return this._Length; }
            set { this.SetField(ref this._Length, value); }
        }

        public double StartFrequency
        {
            get { return this._StartFrequency; }
            set { this.SetField(ref this._StartFrequency, value); }
        }

        public double StopFrequency
        {
            get { return this._StopFrequency; }
            set { this.SetField(ref this._StopFrequency, value); }
        }

        public override ISignal CreateItem()
        {
            return new LogSweep(this.StartFrequency, this.StopFrequency, this.Length, this.SampleRate);
        }
    }
}