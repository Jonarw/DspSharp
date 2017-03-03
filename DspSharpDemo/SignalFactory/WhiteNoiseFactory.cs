// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhiteNoiseFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Signal;
using PropertyTools.DataAnnotations;

namespace DspSharpDemo.SignalFactory
{
    public class WhiteNoiseFactory : SignalFactory
    {
        private double _Mean;
        private double _Variance = 1;

        public override ISignal CreateSignal()
        {
            return new WhiteNoise(this.SampleRate, this.Mean, this.Variance);
        }

        [DisplayName("mean")]
        [Category("white noise settings")]
        [SortIndex(2)]
        public double Mean
        {
            get { return this._Mean; }
            set { this.SetField(ref this._Mean, value); }
        }

        [DisplayName("variance")]
        [Category("white noise settings")]
        [SortIndex(3)]
        public double Variance
        {
            get { return this._Variance; }
            set { this.SetField(ref this._Variance, value); }
        }
    }
}