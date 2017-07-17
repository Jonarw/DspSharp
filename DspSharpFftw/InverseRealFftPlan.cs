// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InverseRealFftPlan.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Numerics;
using DspSharp;
using DspSharp.Algorithms;

namespace DspSharpFftw
{
    /// <summary>
    ///     Plan for a real-valued IFFT.
    /// </summary>
    public unsafe class InverseRealFftPlan : RealFftPlan
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InverseRealFftPlan" /> class.
        /// </summary>
        /// <param name="fftLength">The FFT length the plan is used for.</param>
        public InverseRealFftPlan(int fftLength) : base(fftLength, FftwInterop.PlanDftC2R1D)
        {
            this.NFactor = 1D / fftLength;
            this.SqrNFactor = Math.Sqrt(this.NFactor);
        }

        private double NFactor { get; }
        private double SqrNFactor { get; }

        private static Dictionary<int, InverseRealFftPlan> PlanCache { get; } =
            new Dictionary<int, InverseRealFftPlan>();

        public void Execute(Complex[] input, double[] output, NormalizationKind normalization)
        {
            if (input.Length != this.SpectrumLength)
                throw new ArgumentException();

            if (output.Length < this.FftLength)
                throw new ArgumentException();

            var pInput = (void*)0;
            var pOutput = (void*)0;
            try
            {
                pInput = FftwInterop.Malloc(this.SpectrumLength * 2 * sizeof(double));
                pOutput = FftwInterop.Malloc(this.FftLength * sizeof(double));

                fixed (Complex* pinputarray = input)
                {
                    Interop.memcpy(pInput, pinputarray, this.SpectrumLength * 2 * sizeof(double));
                }

                this.ExecuteUnsafe(pInput, pOutput, normalization);

                fixed (double* pRet = output)
                {
                    Interop.memcpy(pRet, pOutput, this.FftLength * sizeof(double));
                }
            }
            finally
            {
                FftwInterop.Free(pInput);
                FftwInterop.Free(pOutput);
            }
        }

        public double[] Execute(Complex[] input, NormalizationKind normalization)
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

        public static InverseRealFftPlan GetPlan(int length)
        {
            return new InverseRealFftPlan(length);
            if (!PlanCache.ContainsKey(length))
                PlanCache.Add(length, new InverseRealFftPlan(length));

        }
    }
}