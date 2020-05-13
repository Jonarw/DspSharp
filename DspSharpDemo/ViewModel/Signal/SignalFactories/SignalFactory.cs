// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignalFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Signal;
using UTilities.Factory;

namespace DspSharpDemo.ViewModel.Signal.SignalFactories
{
    public abstract class SignalFactory : Factory<ISignal>
    {
        private int _TimeOffset;

        protected SignalFactory(double sampleRate)
        {
            this.SampleRate = sampleRate;
        }

        public double SampleRate { get; }

        public int TimeOffset
        {
            get { return this._TimeOffset; }
            set { this.SetField(ref this._TimeOffset, value); }
        }
    }
}