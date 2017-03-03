// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FftPlan.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DspSharpFftw
{
    public abstract unsafe class FftPlan
    {
        protected FftPlan(int fftLength, void* plan)
        {
            this.FftLength = fftLength;
            this.Plan = plan;
            FftwProvider.ExportWisdom();
        }

        public int FftLength { get; }
        public void* Plan { get; }

        public abstract void ExecuteUnsafe(void* input, void* output);

        ~FftPlan()
        {
            FftwInterop.DestroyPlan(this.Plan);
        }
    }
}