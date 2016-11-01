using System.Collections.Generic;

namespace Filter.LtiFilter.Types
{
    /// <summary>
    ///     A filter that's transfer function is determined by a static pre-defined impulse response.
    /// </summary>
    public class ImpulseFilter : Convolver
    {
        public ImpulseFilter(double samplerate) : base(samplerate)
        {
        }

        /// <summary>
        ///     The impulse response that determines the filter's transfer function.
        /// </summary>
        public IReadOnlyList<double> SourceImpulseResponse { get; set; }

        /// <summary>
        ///     True if the <see cref="SourceImpulseResponse" /> is not null and not dirac. False otherwise.
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
        ///     Computes the impulse response.
        /// </summary>
        /// <param name="length">The length of the impulse response.</param>
        /// <returns>
        ///     The impulse response.
        /// </returns>
        protected IEnumerable<double> GetImpulseResponse()
        {
            return this.SourceImpulseResponse;
        }
    }
}