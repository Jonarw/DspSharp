// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FftPlan.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Algorithms;

namespace DspSharpFftw
{
    public abstract unsafe class FftPlan
    {
        protected FftPlan(int fftLength, void* plan, FftwFlags flags)
        {
            this.FftLength = fftLength;
            this.Plan = plan;
            FftwInterop.ExportWisdom();
            this.Flags = flags;
        }

        ~FftPlan()
        {
            FftwInterop.DestroyPlan(this.Plan);
        }

        public int FftLength { get; }
        public FftwFlags Flags { get; }
        public void* Plan { get; }

        public abstract void ExecuteUnsafe(void* input, void* output, NormalizationKind normalization);
    }
}