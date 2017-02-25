using System;
using System.Collections.Generic;
using System.Numerics;

namespace Filter.Algorithms
{
    /// <summary>
    ///     Plan for a real-valued forward FFT.
    /// </summary>
    public class ForwardRealFftPlan : RealFftPlan
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ForwardRealFftPlan" /> class.
        /// </summary>
        /// <param name="fftLength">The FFT length the plan is used for.</param>
        protected ForwardRealFftPlan(int fftLength) : base(fftLength, FftwInterop.dft_r2c_1d)
        {
        }

        private static Dictionary<int, ForwardRealFftPlan> PlanCache { get; } = new Dictionary<int, ForwardRealFftPlan>();

        public void Execute(double[] input, Complex[] output)
        {
            if (input.Length > this.FftLength)
                throw new ArgumentException();

            if (output.Length < this.SpectrumLength)
                throw new ArgumentException();

            IntPtr pInput = IntPtr.Zero;
            IntPtr pOutput = IntPtr.Zero;

            try
            {
                pInput = FftwInterop.malloc(this.FftLength * sizeof(double));
                pOutput = FftwInterop.malloc(this.SpectrumLength * 2 * sizeof(double));

                unsafe
                {
                    fixed (double* pinputarray = input)
                    {
                        Interop.memcpy((void*)pInput, pinputarray, input.Length * sizeof(double));

                        if (input.Length < this.FftLength)
                            Interop.memset((double*)pInput + input.Length, 0, (this.FftLength - input.Length) * sizeof(double));
                    }

                    FftwInterop.execute_dft_r2c(this.Plan, pInput, pOutput);

                    fixed (Complex* poutputarray = output)
                    {
                        Interop.memcpy(poutputarray, (void*)pOutput, this.SpectrumLength * 2 * sizeof(double));
                    }
                }
            }
            finally
            {
                FftwInterop.free(pInput);
                FftwInterop.free(pOutput);
            }
        }

        public Complex[] Execute(double[] input)
        {
            var ret = new Complex[this.SpectrumLength];
            this.Execute(input, ret);
            return ret;
        }

        public override void ExecuteUnsafe(IntPtr pInput, IntPtr pOutput)
        {
            FftwInterop.execute_dft_r2c(this.Plan, pInput, pOutput);
        }

        public static ForwardRealFftPlan GetPlan(int length)
        {
            if (!PlanCache.ContainsKey(length))
                PlanCache.Add(length, new ForwardRealFftPlan(length));

            return PlanCache[length];
        }
    }
}