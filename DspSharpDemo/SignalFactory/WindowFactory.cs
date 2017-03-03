// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Signal;
using DspSharp.Signal.Windows;
using PropertyTools.DataAnnotations;

namespace DspSharpDemo.SignalFactory
{
    public class WindowFactory : SignalFactory
    {
        private bool _CustomStart;
        private int _Length = 1024;
        private WindowModes _Mode = WindowModes.Symmetric;
        private int _Start;
        private WindowTypes _type = WindowTypes.Hann;

        public override ISignal CreateSignal()
        {
            if (this.CustomStart)
                return new Window(this.Type, this.Start, this.Length, this.SampleRate, this.Mode);

            return new Window(this.Type, this.Length, this.SampleRate, this.Mode);
        }

        [DisplayName("custom window start time")]
        [Category("window settings")]
        [SortIndex(5)]
        public bool CustomStart
        {
            get { return this._CustomStart; }
            set { this.SetField(ref this._CustomStart, value); }
        }

        [DisplayName("window length")]
        [Category("window settings")]
        [SortIndex(3)]
        public int Length
        {
            get { return this._Length; }
            set { this.SetField(ref this._Length, value); }
        }

        [DisplayName("window mode")]
        [Category("window settings")]
        [SortIndex(4)]
        public WindowModes Mode
        {
            get { return this._Mode; }
            set { this.SetField(ref this._Mode, value); }
        }

        [DisplayName("default window start time")]
        [Category("window settings")]
        [SortIndex(6)]
        [EnableBy(nameof(CustomStart))]
        public int Start
        {
            get { return this._Start; }
            set { this.SetField(ref this._Start, value); }
        }

        [DisplayName("window type")]
        [Category("window settings")]
        [SortIndex(2)]
        public WindowTypes Type
        {
            get { return this._type; }
            set { this.SetField(ref this._type, value); }
        }
    }
}