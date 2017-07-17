// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComplexFftPlan.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Numerics;
using DspSharp;
using DspSharp.Algorithms;

namespace DspSharpFftw
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
            this.Direction = direction;
            this.NFactor = 1D / fftLength;
            this.SqrNFactor = Math.Sqrt(this.NFactor);
        }

        public FftwDirection Direction { get; }

        private double NFactor { get; }
        private double SqrNFactor { get; }

        /// <summary>
        ///     Executes the plan for the specified input, writing to an already existing array.
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <param name="output">The output array.</param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void Execute(Complex[] input, Complex[] output, NormalizationKind normalization)
        {
            if (input.Length > this.FftLength)
                throw new ArgumentException();

            if (output.Length < this.FftLength)
                throw new ArgumentException();

            var pInput = (void*)0;
            var pOutput = (void*)0;

            try
            {
                pInput = FftwInterop.Malloc(this.FftLength * 2 * sizeof(double));
                pOutput = FftwInterop.Malloc(this.FftLength * 2 * sizeof(double));

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

                this.ExecuteUnsafe(pInput, pOutput, normalization);

                fixed (Complex* pRet = output)
                {
                    Interop.memcpy(pRet, pOutput, this.FftLength * 2 * sizeof(double));
                }
            }
            finally
            {
                FftwInterop.Free(pInput);
                FftwInterop.Free(pOutput);
            }
        }

        /// <summary>
        ///     Executes the plan for the provided data, creating a new result array.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <returns>The (I)FFT of the input data.</returns>
        public Complex[] Execute(Complex[] input, NormalizationKind normalization)
        {
            var ret = new Complex[this.FftLength];
            this.Execute(input, ret, normalization);
            return ret;
        }

        public override void ExecuteUnsafe(void* pInput, void* pOutput, NormalizationKind normalization)
        {
            FftwInterop.ExecuteDft(this.Plan, pInput, pOutput);

            if (normalization != NormalizationKind.None)
            {
                var dpOutput = (double*)pOutput;
                var factor = normalization == NormalizationKind.N ? this.NFactor : this.SqrNFactor;
                for (var i = 0; i < this.FftLength * 2; i++)
                {
                    dpOutput[i] *= factor;
                }
            }
        }

        private static void* CreatePlan(int fftLength, FftwDirection direction)
        {
            var pInput = (void*)0;
            var pOutput = (void*)0;
            try
            {
                pInput = FftwInterop.Malloc(fftLength * 2 * sizeof(double));
                pOutput = FftwInterop.Malloc(fftLength * 2 * sizeof(double));

                return FftwInterop.PlanDft1D(
                    fftLength,
                    pInput,
                    pOutput,
                    direction,
                    FftwFlags.Measure | FftwFlags.DestroyInput);
            }
            finally
            {
                // free arrays used for planning - we won't ever call fftw_execute
                FftwInterop.Free(pInput);
                FftwInterop.Free(pOutput);
            }
        }
    }
}