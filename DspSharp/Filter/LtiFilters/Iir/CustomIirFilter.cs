// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomIirFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace DspSharp.Filter.LtiFilters.Iir
{
    public class CustomIirFilter : IirFilter
    {
        public CustomIirFilter(double samplerate, IEnumerable<double> a, IEnumerable<double> b) : base(samplerate)
        {
            base.SetCoefficients(a, b);
        }

        public CustomIirFilter(double samplerate) : base(samplerate)
        {
        }

        public new void SetCoefficients(IEnumerable<double> a, IEnumerable<double> b)
        {
            base.SetCoefficients(a, b);
        }
    }
}