using System.Collections.Generic;
using Filter.Series;
using Filter.Signal;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    /// A filter that's transfer function is determined by a static pre-defined impulse response.
    /// </summary>
    public class ImpulseFilter : Convolver
    {
        /// <summary>
        /// The impulse response that determines the <see cref="SpectrumFilter"/>'s transfer function.
        /// </summary>
        public IReadOnlyList<double> SourceImpulseResponse { get; set; }

        /// <summary>
        /// True if the <see cref="SourceImpulseResponse"/> is not null and not dirac. False otherwise.
        /// </summary>
        protected override bool HasEffectOverride
        {
            get
            {
                if (this.SourceImpulseResponse == null)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the default length for the impulse response.
        /// </summary>
        /// <returns>
        /// The default impulse length.
        /// </returns>
        public override int GetDefaultImpulseLength()
        {
            if (this.SourceImpulseResponse == null)
            {
                return 0;
            }

            return this.SourceImpulseResponse.Count;
        }

        /// <summary>
        /// Computes the impulse response.
        /// </summary>
        /// <param name="length">The length of the impulse response.</param>
        /// <returns>
        /// The impulse response.
        /// </returns>
        protected override IEnumerable<double> GetImpulseResponseOverride()
        {
            return this.SourceImpulseResponse;
        }
    }
}