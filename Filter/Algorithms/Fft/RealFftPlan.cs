using System;

namespace Filter.Algorithms
{
    /// <summary>
    ///     Handles the creating of an fftw plan and the associated memory blocks.
    /// </summary>
    public abstract class RealFftPlan : FftPlan
    {
        protected delegate IntPtr CreateRealPlanDelegate(int fftLength, IntPtr pInput, IntPtr pOutput, FftwFlags flags);

        /// <summary>
        ///     Initializes a new instance of the base class <see cref="RealFftPlan" />.
        /// </summary>
        /// <param name="fftLength">The FFT lenght the plan is used for.</param>
        /// <param name="createPlanDelegate"></param>
        protected RealFftPlan(int fftLength, CreateRealPlanDelegate createPlanDelegate) : base(fftLength, CreatePlan(fftLength, createPlanDelegate))
        {
            this.SpectrumLength = (this.FftLength >> 1) + 1;
        }

        private static IntPtr CreatePlan(int fftLength, CreateRealPlanDelegate createPlanDelegate)
        {
            var spectrumLength = (fftLength >> 1) + 1;

            IntPtr pInput = IntPtr.Zero;
            IntPtr pOutput = IntPtr.Zero;
            try
            {
                // make both memory blocks the same size for simplicity (16 bytes are wasted)
                pInput = FftwInterop.malloc(spectrumLength * 2 * sizeof(double));
                pOutput = FftwInterop.malloc(spectrumLength * 2 * sizeof(double));

                lock (FftwInterop.FftwLock)
                {
                    return createPlanDelegate(fftLength, pInput, pOutput, FftwFlags.Measure | FftwFlags.DestroyInput);
                }
            }
            finally
            {
                // free arrays used for planning - we won't ever call fftw_execute
                FftwInterop.free(pInput);
                FftwInterop.free(pOutput);
            }
        }

        public int SpectrumLength { get; }
    }
}