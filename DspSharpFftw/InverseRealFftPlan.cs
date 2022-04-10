// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InverseRealFftPlan.cs">
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
    /// Plan for a real-valued IFFT.
    /// </summary>
    public unsafe class InverseRealFftPlan : RealFftPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InverseRealFftPlan" /> class.
        /// </summary>
        /// <param name="fftLength">The FFT length the plan is used for.</param>
        public InverseRealFftPlan(int fftLength, FftwFlags flags = FftwFlags.Measure | FftwFlags.DestroyInput) : base(fftLength, FftwInterop.PlanDftC2R1D, flags)
        {
            this.NFactor = 1D / fftLength;
            this.SqrNFactor = Math.Sqrt(this.NFactor);
        }

        private double NFactor { get; }
        private double SqrNFactor { get; }

        public void Execute(IReadOnlyList<Complex> input, IList<double> output, NormalizationKind normalization)
        {
            if (input.Count != this.SpectrumLength)
                throw new ArgumentException();

            if (output.Count < this.FftLength)
                throw new ArgumentException();

            var pInput = (Complex*)0;
            var pOutput = (double*)0;
            try
            {
                pInput = (Complex*)FftwInterop.Malloc(this.SpectrumLength * 2 * sizeof(double));
                pOutput = (double*)FftwInterop.Malloc(this.FftLength * sizeof(double));

                Memory.Copy(input, pInput);
                this.ExecuteUnsafe(pInput, pOutput, normalization);
                Memory.Copy(pOutput, output);
            }
            finally
            {
                FftwInterop.Free(pInput);
                FftwInterop.Free(pOutput);
            }
        }

        public double[] Execute(IReadOnlyList<Complex> input, NormalizationKind normalization)
        {
            var ret = new double[this.FftLength];
            this.Execute(input, ret, normalization);
            return ret;
        }

        public override void ExecuteUnsafe(void* pInput, void* pOutput, NormalizationKind normalization)
        {
            FftwInterop.ExecuteDftC2R(this.Plan, pInput, pOutput);

            if (normalization != NormalizationKind.None)
            {
                var dpOutput = (double*)pOutput;
                var factor = normalization == NormalizationKind.N ? this.NFactor : this.SqrNFactor;
                for (var i = 0; i < this.FftLength; i++)
                {
                    dpOutput[i] *= factor;
                }
            }
        }
    }
}