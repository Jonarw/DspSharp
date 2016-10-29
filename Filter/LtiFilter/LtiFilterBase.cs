using System.Collections.Generic;
using Filter.Extensions;
using Filter.LtiFilter.Types;
using Filter.Series;
using Filter.Signal;

namespace Filter.LtiFilter
{
    /// <summary>
    ///     Base class for all types of LTI (linear, time-invariant) filters.
    /// </summary>
    public abstract class LtiFilterBase : FilterBase
    {
        internal const int DefaultImpulseLength = 8192;
        private IEnumerable<double> ImpulseResponseCache { get; set; }

        /// <summary>
        ///     Gets the default length for the impulse response.
        /// </summary>
        /// <returns>The default impulse length.</returns>
        public virtual int GetDefaultImpulseLength()
        {
            return DefaultImpulseLength;
        }

        /// <summary>
        ///     Gets the impulse response.
        /// </summary>
        /// <returns>The result.</returns>
        public IEnumerable<double> GetImpulseResponse()
        {
            if (!this.HasEffectOverride)
            {
                return 1.0.ToEnumerable();
            }

            return this.ImpulseResponseCache ?? (this.ImpulseResponseCache = this.GetImpulseResponseOverride());
        }

        /// <summary>
        ///     Computes the impulse response.
        /// </summary>
        protected virtual IEnumerable<double> GetImpulseResponseOverride()
        {
            return this.Process(1.0.ToEnumerable());
        }

        /// <summary>
        ///     Can be overridden by a child class to perform a certain action every time the <see cref="LtiFilterBase" /> is
        ///     changed.
        /// </summary>
        protected override void OnChangeOverride()
        {
            this.ImpulseResponseCache = null;
        }
    }
}