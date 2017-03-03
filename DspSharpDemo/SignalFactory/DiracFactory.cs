// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiracFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Signal;
using PropertyTools.DataAnnotations;

namespace DspSharpDemo.SignalFactory
{
    public class DiracFactory : SignalFactory
    {
        private double _Gain = 1;

        public override ISignal CreateSignal()
        {
            return new Dirac(this.SampleRate, this.Gain);
        }

        [DisplayName("gain factor")]
        [Category("dirac settings")]
        [SortIndex(2)]
        public double Gain
        {
            get { return this._Gain; }
            set { this.SetField(ref this._Gain, value); }
        }
    }
}