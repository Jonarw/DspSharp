// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SinusFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Signal;
using PropertyTools.DataAnnotations;

namespace DspSharpDemo.SignalFactory
{
    public class SinusFactory : SignalFactory
    {
        private double _Frequency = 1000;
        private double _Phase;

        public override ISignal CreateSignal()
        {
            return new Sinus(this.SampleRate, this.Frequency, this.Phase);
        }

        [DisplayName("frequency [Hz]")]
        [Category("sinus settings")]
        [SortIndex(2)]
        public double Frequency
        {
            get { return this._Frequency; }
            set { this.SetField(ref this._Frequency, value); }
        }

        [DisplayName("phase offset [rad]")]
        [Category("sinus settings")]
        [SortIndex(3)]
        public double Phase
        {
            get { return this._Phase; }
            set { this.SetField(ref this._Phase, value); }
        }
    }
}