using System;
using System.Numerics;

namespace FilterWin.Fft
{
    /// <summary>
    ///     Handles the creating of an fftw plan and the associated memory blocks.
    /// </summary>
    public class ComplexToComplexFftPlan
    {
        private double NormalizationFactor { get; }

        /// <summary>
        ///     Initializes a new instance of the base class <see cref="ComplexToComplexFftPlan" />.
        /// </summary>
        /// <param name="fftLength">The FFT lenght the plan is used for.</param>
        /// <param name="direction">The FFT direction.</param>
        public ComplexToComplexFftPlan(int fftLength, FftwDirection direction)
        {
            this.FftLength = fftLength;
            this.NormalizationFactor = 1D / this.FftLength;
            this.Direction = direction;

            IntPtr pInput = IntPtr.Zero;
            IntPtr pOutput = IntPtr.Zero;
            try
            {
                pInput = FftwInterop.malloc(this.FftLength * 2 * sizeof (double));
                pOutput = FftwInterop.malloc(this.FftLength * 2 * sizeof (double));

                lock (FftwInterop.FftwLock)
                {
                    this.Plan = FftwInterop.dft_1d(this.FftLength, pInput, pOutput, direction, FftwFlags.Measure | FftwFlags.DestroyInput);
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

        public FftwDirection Direction { get; }

        /// <summary>
        ///     The FFTW plan.
        /// </summary>
        protected IntPtr Plan { get; }

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
            {
                throw new ArgumentException();
            }

            if (output.Length < this.FftLength)
            {
                throw new ArgumentException();
            }

            IntPtr pInput = IntPtr.Zero;
            IntPtr pOutput = IntPtr.Zero;

            try
            {
                pInput = FftwInterop.malloc(this.FftLength * 2 * sizeof (double));
                pOutput = FftwInterop.malloc(this.FftLength * 2 * sizeof (double));

                unsafe
                {
                    fixed (Complex* pinputarray = input)
                    {
                        FftwInterop.memcpy(pInput, (IntPtr)pinputarray, input.Length * 2 * sizeof (double));

                        if (input.Length < this.FftLength)
                        {
                            FftwInterop.memset(pInput + input.Length * 2 * sizeof (double), 0, (this.FftLength - input.Length) * 2 * sizeof (double));
                        }
                    }

                    FftwInterop.execute_dft(this.Plan, pInput, pOutput);

                    fixed (Complex* pRet = output)
                    {
                        if (this.Direction == FftwDirection.Forward)
                        {
                            FftwInterop.memcpy((IntPtr)pRet, pOutput, this.FftLength * 2 * sizeof(double));
                        }
                        else
                        {
                            var dpOutput = (double*)pOutput;
                            var dpRet = (double*)pRet;

                            for (int i = 0; i < this.FftLength * 2; i++)
                            {
                                *(dpRet + i) = *(dpOutput + i) * this.NormalizationFactor;
                            }
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

        ~ComplexToComplexFftPlan()
        {
            FftwInterop.destroy_plan(this.Plan);
        }
    }
}