using Filter_Win.FFTWSharp;

namespace Filter_Win
{
    /// <summary>
    ///     Handles the creating of an fftw plan and the associated memory blocks.
    /// </summary>
    public abstract class RealToComplexFftPlan
    {
        /// <summary>
        ///     Initializes a new instance of the base class <see cref="RealToComplexFftPlan" />.
        /// </summary>
        /// <param name="fftLength">The FFT lenght the plan is used for.</param>
        protected RealToComplexFftPlan(int fftLength)
        {
            this.N = fftLength;
            this.FftwR = new FftwRealarray(this.N);
            this.FftwC = new FftwComplexarray((this.N >> 1) + 1);
        }

        /// <summary>
        ///     The FFT length the plan is used for.
        /// </summary>
        public int N { get; }

        /// <summary>
        ///     The FFTW plan.
        /// </summary>
        protected FftwPlan FftwP { get; set; }

        /// <summary>
        ///     The unmanaged data array for the real values.
        /// </summary>
        protected FftwRealarray FftwR { get; set; }

        /// <summary>
        ///     The unmanaged data array for the complex values.
        /// </summary>
        protected FftwComplexarray FftwC { get; set; }
    }
}