// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForwardRealFftPlan.cs">
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
    ///     Plan for a real-valued forward FFT.
    /// </summary>
    public unsafe class ForwardRealFftPlan : RealFftPlan
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ForwardRealFftPlan" /> class.
        /// </summary>
        /// <param name="fftLength">The FFT length the plan is used for.</param>
        public ForwardRealFftPlan(int fftLength) : base(fftLength, FftwInterop.PlanDftR2C1D)
        {
            this.NFactor = 1D / fftLength;
            this.SqrNFactor = Math.Sqrt(this.NFactor);
        }

        private double NFactor { get; }

        private static Dictionary<int, ForwardRealFftPlan> PlanCache { get; } =
            new Dictionary<int, ForwardRealFftPlan>();

        private double SqrNFactor { get; }

        public void Execute(double[] input, Complex[] output, NormalizationKind normalization)
        {
            if (input.Length > this.FftLength)
                throw new ArgumentException();

            if (output.Length < this.SpectrumLength)
                throw new ArgumentException();

            var pInput = (void*)0;
            var pOutput = (void*)0;
            try
            {
                pInput = FftwInterop.Malloc(this.FftLength * sizeof(double));
                pOutput = FftwInterop.Malloc(this.SpectrumLength * 2 * sizeof(double));

                fixed (double* pinputarray = input)
                {
                    Interop.memcpy(pInput, pinputarray, input.Length * sizeof(double));

                    if (input.Length < this.FftLength)
                    {
                        Interop.memset(
                            (double*)pInput + input.Length,
                            0,
                            (this.FftLength - input.Length) * sizeof(double));
                    }
                }

                this.ExecuteUnsafe(pInput, pOutput, normalization);

                fixed (Complex* poutputarray = output)
                {
                    Interop.memcpy(poutputarray, pOutput, this.SpectrumLength * 2 * sizeof(double));
                }
            }
            finally
            {
                FftwInterop.Free(pInput);
                FftwInterop.Free(pOutput);
            }
        }

        public Complex[] Execute(double[] input, NormalizationKind normalization)
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

        public static ForwardRealFftPlan GetPlan(int length)
        {
            return new ForwardRealFftPlan(length);
            if (!PlanCache.ContainsKey(length))
            {
                var plan = new ForwardRealFftPlan(length);
                if (!PlanCache.ContainsKey(length))
                    PlanCache.Add(length, plan);
            }
        }
    }
}