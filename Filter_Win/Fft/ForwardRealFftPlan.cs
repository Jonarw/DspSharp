using System;
using System.Numerics;
using FilterWin.Fft.FftwSharp;

namespace FilterWin.Fft
{
    /// <summary>
    ///     Plan for a real-valued forward FFT.
    /// </summary>
    public class ForwardRealFftPlan : RealToComplexFftPlan
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ForwardRealFftPlan" /> class.
        /// </summary>
        /// <param name="fftLength">The FFT length the plan is used for.</param>
        public ForwardRealFftPlan(int fftLength) : base(fftLength, FftwInterop.dft_r2c_1d)
        {
        }

        public void Execute(double[] input, Complex[] output)
        {
            if (input.Length > this.FftLength)
            {
                throw new ArgumentException();
            }

            if (output.Length < this.SpectrumLength)
            {
                throw new ArgumentException();
            }

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
                        FftwInterop.memcpy(pInput, (IntPtr)pinputarray, input.Length * sizeof(double));

                        if (input.Length < this.FftLength)
                        {
                            FftwInterop.memset(pInput + input.Length * sizeof(double), 0, (this.FftLength - input.Length) * sizeof(double));
                        }
                    }

                    FftwInterop.execute_dft_r2c(this.FftwP, pInput, pOutput);

                    fixed (Complex* poutputarray = output)
                    {
                        FftwInterop.memcpy((IntPtr)poutputarray, pOutput, this.SpectrumLength * 2 * sizeof(double));
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
    }
}