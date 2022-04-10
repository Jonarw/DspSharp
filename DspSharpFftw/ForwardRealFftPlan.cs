// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForwardRealFftPlan.cs">
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
    /// Plan for a real-valued forward FFT.
    /// </summary>
    public unsafe class ForwardRealFftPlan : RealFftPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardRealFftPlan" /> class.
        /// </summary>
        /// <param name="fftLength">The FFT length the plan is used for.</param>
        public ForwardRealFftPlan(int fftLength, FftwFlags flags = FftwFlags.Measure | FftwFlags.DestroyInput) : base(fftLength, FftwInterop.PlanDftR2C1D, flags)
        {
            this.NFactor = 1D / fftLength;
            this.SqrNFactor = Math.Sqrt(this.NFactor);
        }

        private double NFactor { get; }
        private double SqrNFactor { get; }

        public void Execute(IReadOnlyList<double> input, IList<Complex> output, NormalizationKind normalization)
        {
            if (input.Count > this.FftLength)
                throw new ArgumentException();

            if (output.Count < this.SpectrumLength)
                throw new ArgumentException();

            var pInput = (double*)0;
            var pOutput = (Complex*)0;
            try
            {
                pInput = (double*)FftwInterop.Malloc(this.FftLength * sizeof(double));
                pOutput = (Complex*)FftwInterop.Malloc(this.SpectrumLength * 2 * sizeof(double));

                Memory.Copy(input, pInput);

                if (input.Count < this.FftLength)
                {
                    Memory.Clear(pInput + input.Count, this.FftLength - input.Count);
                }

                this.ExecuteUnsafe(pInput, pOutput, normalization);

                Memory.Copy(pOutput, output);
            }
            finally
            {
                FftwInterop.Free(pInput);
                FftwInterop.Free(pOutput);
            }
        }

        public Complex[] Execute(IReadOnlyList<double> input, NormalizationKind normalization)
        {
            var ret = new Complex[this.SpectrumLength];
            this.Execute(input, ret, normalization);
            return ret;
        }

        public override void ExecuteUnsafe(void* pInput, void* pOutput, NormalizationKind normalization)
        {
            FftwInterop.ExecuteDftR2C(this.Plan, pInput, pOutput);

            if (normalization != NormalizationKind.None)
            {
                var dpOutput = (double*)pOutput;
                var factor = normalization == NormalizationKind.N ? this.NFactor : this.SqrNFactor;
                for (var i = 0; i < this.SpectrumLength * 2; i++)
                {
                    dpOutput[i] *= factor;
                }
            }
        }
    }
}