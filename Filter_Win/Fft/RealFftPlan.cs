using System;

namespace FilterWin.Fft
{
    /// <summary>
    ///     Handles the creating of an fftw plan and the associated memory blocks.
    /// </summary>
    public abstract class RealFftPlan
    {
        protected delegate IntPtr CreateRealPlanDelegate(int fftLength, IntPtr pInput, IntPtr pOutput, FftwFlags flags);

        /// <summary>
        ///     Initializes a new instance of the base class <see cref="RealFftPlan" />.
        /// </summary>
        /// <param name="fftLength">The FFT lenght the plan is used for.</param>
        /// <param name="createPlanDelegate"></param>
        protected RealFftPlan(int fftLength, CreateRealPlanDelegate createPlanDelegate)
        {
            this.FftLength = fftLength;
            this.SpectrumLength = (this.FftLength >> 1) + 1;

            IntPtr pInput = IntPtr.Zero;
            IntPtr pOutput = IntPtr.Zero;
            try
            {
                // make both memory blocks the same size for simplicity (16 bytes are wasted)
                pInput = FftwInterop.malloc(this.SpectrumLength * 2 * sizeof(double));
                pOutput = FftwInterop.malloc(this.SpectrumLength * 2 * sizeof(double));

                lock (FftwInterop.FftwLock)
                {
                    this.FftwP = createPlanDelegate(this.FftLength, pInput, pOutput, FftwFlags.Measure | FftwFlags.DestroyInput);
                }
            }
            finally
            {
                // free arrays used for planning - we won't ever call fftw_execute
                FftwInterop.free(pInput);
                FftwInterop.free(pOutput);
            }
        }

        /// <summary>
        ///     The FFT length the plan is used for.
        /// </summary>
        public int FftLength { get; }

        public int SpectrumLength { get; }

        /// <summary>
        ///     The FFTW plan.
        /// </summary>
        protected IntPtr FftwP { get; set; }

        ~RealFftPlan()
        {
            FftwInterop.destroy_plan(this.FftwP);
        }
    }
}