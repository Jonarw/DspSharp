using System;
using System.Numerics;

namespace DspSharp.Algorithms.FftwProvider
{
    /// <summary>
    ///     Handles the creating of an fftw plan and the associated memory blocks.
    /// </summary>
    public unsafe class ComplexToComplexFftPlan : FftPlan
    {
        /// <summary>
        ///     Initializes a new instance of the base class <see cref="ComplexToComplexFftPlan" />.
        /// </summary>
        /// <param name="fftLength">The FFT lenght the plan is used for.</param>
        /// <param name="direction">The FFT direction.</param>
        public ComplexToComplexFftPlan(int fftLength, FftwDirection direction)
            : base(fftLength, CreatePlan(fftLength, direction))
        {
            this.NormalizationFactor = 1D / this.FftLength;
            this.Direction = direction;
        }

        public FftwDirection Direction { get; }
        private double NormalizationFactor { get; }

        /// <summary>
        ///     Executes the plan for the specified input, writing to an already existing array.
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <param name="output">The output array.</param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void Execute(Complex[] input, Complex[] output)
        {
            if (input.Length > this.FftLength)
                throw new ArgumentException();

            if (output.Length < this.FftLength)
                throw new ArgumentException();

            var pInput = (void*)0;
            var pOutput = (void*)0;

            try
            {
                pInput = FftwInterop.malloc(this.FftLength * 2 * sizeof(double));
                pOutput = FftwInterop.malloc(this.FftLength * 2 * sizeof(double));

                fixed (Complex* pinputarray = input)
                {
                    Interop.memcpy(pInput, pinputarray, input.Length * 2 * sizeof(double));

                    if (input.Length < this.FftLength)
                    {
                        Interop.memset(
                            (Complex*)pInput + input.Length,
                            0,
                            (this.FftLength - input.Length) * 2 * sizeof(double));
                    }
                }

                FftwInterop.execute_dft(this.Plan, pInput, pOutput);

                fixed (Complex* pRet = output)
                {
                    if (this.Direction == FftwDirection.Forward)
                        Interop.memcpy(pRet, pOutput, this.FftLength * 2 * sizeof(double));
                    else
                    {
                        var dpOutput = (double*)pOutput;
                        var dpRet = (double*)pRet;

                        for (var i = 0; i < this.FftLength * 2; i++)
                        {
                            *(dpRet + i) = *(dpOutput + i) * this.NormalizationFactor;
                        }
                    }
                }
            }
            finally
            {
                FftwInterop.free(pInput);
                FftwInterop.free(pOutput);
            }
        }

        /// <summary>
        ///     Executes the plan for the provided data, creating a new result array.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <returns>The (I)FFT of the input data.</returns>
        public Complex[] Execute(Complex[] input)
        {
            var ret = new Complex[this.FftLength];
            this.Execute(input, ret);
            return ret;
        }

        public override void ExecuteUnsafe(void* pInput, void* pOutput)
        {
            FftwInterop.execute_dft(this.Plan, pInput, pOutput);
            if (this.Direction == FftwDirection.Backward)
            {
                var dpOutput = (double*)pOutput;

                for (var i = 0; i < this.FftLength * 2; i++)
                {
                    *(dpOutput + i) *= this.NormalizationFactor;
                }
            }
        }

        private static void* CreatePlan(int fftLength, FftwDirection direction)
        {
            var pInput = (void*)0;
            var pOutput = (void*)0;
            try
            {
                pInput = FftwInterop.malloc(fftLength * 2 * sizeof(double));
                pOutput = FftwInterop.malloc(fftLength * 2 * sizeof(double));

                lock (FftwInterop.FftwLock)
                {
                    return FftwInterop.dft_1d(
                        fftLength,
                        pInput,
                        pOutput,
                        direction,
                        FftwFlags.Measure | FftwFlags.DestroyInput);
                }
            }
            finally
            {
                // free arrays used for planning - we won't ever call fftw_execute
                FftwInterop.free(pInput);
                FftwInterop.free(pOutput);
            }
        }
    }
}