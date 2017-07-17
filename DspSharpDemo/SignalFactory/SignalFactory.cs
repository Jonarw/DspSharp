// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignalFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp;
using DspSharp.Filter;
using DspSharp.Signal;
using DspSharp.Utilities;
using PropertyTools.DataAnnotations;

namespace DspSharpDemo.SignalFactory
{
    public abstract class SignalFactory : Observable
    {
        private double _SampleRate = 44100;

        private int _TimeOffset;

        [Browsable(false)]
        public IReadOnlyList<double> AvailableSampleRates { get; } = FilterBase.DefaultSampleRates;

        [ItemsSourceProperty(nameof(AvailableSampleRates))]
        //[DisplayName("sample rate")]
        [Category("basic settings")]
        [ReadOnly(true)]
        [SortIndex(0)]
        public double SampleRate
        {
            get { return this._SampleRate; }
            set { this.SetField(ref this._SampleRate, value); }
        }

        public abstract ISignal CreateSignal();

        [DisplayName("sample offset")]
        [Category("basic settings")]
        [SortIndex(1)]
        public int TimeOffset
        {
            get { return this._TimeOffset; }
            set { this.SetField(ref this._TimeOffset, value); }
        }
    }
}