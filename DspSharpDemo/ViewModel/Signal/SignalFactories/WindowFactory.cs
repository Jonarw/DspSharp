// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Signal;
using DspSharp.Signal.Windows;

namespace DspSharpDemo.ViewModel.Signal.SignalFactories
{
    public class WindowFactory : SignalFactory
    {
        private bool _CustomStart;
        private int _Length = 1024;
        private WindowMode _Mode = WindowMode.Symmetric;
        private int _Start;
        private WindowTypes _type = WindowTypes.Hann;

        public WindowFactory(double sampleRate) : base(sampleRate)
        {
        }

        public bool CustomStart
        {
            get { return this._CustomStart; }
            set { this.SetField(ref this._CustomStart, value); }
        }

        public int Length
        {
            get { return this._Length; }
            set { this.SetField(ref this._Length, value); }
        }

        public WindowMode Mode
        {
            get { return this._Mode; }
            set { this.SetField(ref this._Mode, value); }
        }

        public int Start
        {
            get { return this._Start; }
            set { this.SetField(ref this._Start, value); }
        }

        public WindowTypes Type
        {
            get { return this._type; }
            set { this.SetField(ref this._type, value); }
        }

        public override ISignal CreateItem()
        {
            if (this.CustomStart)
                return new Window(this.Type, this.Start, this.Length, this.SampleRate, this.Mode);

            return new Window(this.Type, this.Length, this.SampleRate, this.Mode);
        }
    }
}