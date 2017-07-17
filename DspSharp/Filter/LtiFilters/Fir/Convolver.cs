// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Convolver.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using DspSharp.Algorithms;

namespace DspSharp.Filter.LtiFilters.Fir
{
    /// <summary>
    ///     Base class for all filters that can be described by their impulse response.
    /// </summary>
    /// <seealso cref="FilterBase" />
    public abstract class Convolver : FiniteFilter
    {
        protected Convolver(double samplerate) : base(samplerate)
        {
            this.Name = "Convolver";
        }

        public abstract IReadOnlyList<double> ImpulseResponse { get; }

        protected override bool HasEffectOverride
        {
            get
            {
                if (this.ImpulseResponse == null)
                    return false;

                return true;
            }
        }

        public override IEnumerable<double> ProcessOverride(IEnumerable<double> signal)
        {
            return TimeDomain.Convolve(signal, this.ImpulseResponse);
        }
    }
}