// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComplexFftPlan.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Algorithms;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DspSharpFftw
{
    /// <summary>
    /// Handles the creating of an fftw plan and the associated memory blocks.
    /// </summary>
    public unsafe class ComplexToComplexFftPlan : FftPlan
    {
        /// <summary>
        /// Initializes a new instance of the base class <see cref="ComplexToComplexFftPlan" />.
        /// </summary>
        /// <param name="fftLength">The FFT lenght the plan is used for.</param>
        /// <param name="direction">The FFT direction.</param>
        public ComplexToComplexFftPlan(int fftLength, FftwDirection direction, FftwFlags flags = FftwFlags.DestroyInput)
            : base(fftLength, CreatePlan(fftLength, direction), flags)
        {
            this.Direction = direction;
            this.NFactor = 1D / fftLength;
            this.SqrNFactor = Math.Sqrt(this.NFactor);
        }

        public FftwDirection Direction { get; }

        private double NFactor { get; }
        private double SqrNFactor { get; }

        /// <summary>
        /// Executes the plan for the specified input, writing to an already existing array.
        /// </summary>
        /// <param name="input">The input sequence.</param>
        /// <param name="output">The output sequence.</param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void Execute(IReadOnlyList<Complex> input, IList<Complex> output, NormalizationKind normalization)
        {
            if (input.Count > this.FftLength)
                throw new ArgumentException();

            if (output.Count < this.FftLength)
                throw new ArgumentException();

            var pInput = (Complex*)0;
            var pOutput = (Complex*)0;

            try
            {
                pInput = (Complex*)FftwInterop.Malloc(this.FftLength * 2 * sizeof(double));
                pOutput = (Complex*)FftwInterop.Malloc(this.FftLength * 2 * sizeof(double));

                Memory.Copy(input, pInput);

                if (input.Count < this.FftLength)
                    Memory.Clear(pInput + input.Count, this.FftLength - input.Count);

                this.ExecuteUnsafe(pInput, pOutput, normalization);

                Memory.Copy(pOutput, output);
            }
            finally
            {
                FftwInterop.Free(pInput);
                FftwInterop.Free(pOutput);
            }
        }

        /// <summary>
        /// Executes the plan for the provided data, creating a new result array.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The (I)FFT of the input data.</returns>
        public Complex[] Execute(IReadOnlyList<Complex> input, NormalizationKind normalization)
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