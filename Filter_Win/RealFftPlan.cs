using Filter.Algorithms.FFTWSharp;

namespace Filter_Win
{
    /// <summary>
    ///     Handles the creating of an fftw plan and the associated memory blocks.
    /// </summary>
    public abstract class RealFftPlan
    {
        /// <summary>
        ///     Initializes a new instance of the base class <see cref="RealFftPlan" />.
        /// </summary>
        /// <param name="fftLength">The FFT lenght the plan is used for.</param>
        protected RealFftPlan(int fftLength)
        {
            this.N = fftLength;
            this.FftwR = new fftw_realarray(this.N);
            this.FftwC = new fftw_complexarray((this.N >> 1) + 1);
        }

        /// <summary>
        ///     The FFT length the plan is used for.
        /// </summary>
        public int N { get; }

        /// <summary>
        ///     The FFTW plan.
        /// </summary>
        protected fftw_plan FftwP { get; set; }

        /// <summary>
        ///     The unmanaged data array for the real values.
        /// </summary>
        protected fftw_realarray FftwR { get; set; }

        /// <summary>
        ///     The unmanaged data array for the complex values.
        /// </summary>
        protected fftw_complexarray FftwC { get; set; }
    }
}