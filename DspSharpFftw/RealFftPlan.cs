// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealFftPlan.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DspSharpFftw
{
    /// <summary>
    ///     Handles the creating of an fftw plan and the associated memory blocks.
    /// </summary>
    public abstract unsafe class RealFftPlan : FftPlan
    {
        /// <summary>
        ///     Initializes a new instance of the base class <see cref="RealFftPlan" />.
        /// </summary>
        /// <param name="fftLength">The FFT lenght the plan is used for.</param>
        /// <param name="createPlanDelegate"></param>
        protected RealFftPlan(int fftLength, CreateRealPlanDelegate createPlanDelegate)
            : base(fftLength, CreatePlan(fftLength, createPlanDelegate))
        {
            this.SpectrumLength = (this.FftLength >> 1) + 1;
        }

        public int SpectrumLength { get; }

        private static void* CreatePlan(int fftLength, CreateRealPlanDelegate createPlanDelegate)
        {
            var spectrumLength = (fftLength >> 1) + 1;

            var pInput = (void*)0;
            var pOutput = (void*)0;
            try
            {
                // make both memory blocks the same size for simplicity (16 bytes are wasted)
                pInput = FftwInterop.Malloc(spectrumLength * 2 * sizeof(double));
                pOutput = FftwInterop.Malloc(spectrumLength * 2 * sizeof(double));

                return createPlanDelegate(fftLength, pInput, pOutput, FftwFlags.Measure | FftwFlags.DestroyInput);
            }
            finally
            {
                // free arrays used for planning - we won't ever call fftw_execute
                FftwInterop.Free(pInput);
                FftwInterop.Free(pOutput);
            }
        }

        protected delegate void* CreateRealPlanDelegate(int fftLength, void* pInput, void* pOutput, FftwFlags flags);
    }
}