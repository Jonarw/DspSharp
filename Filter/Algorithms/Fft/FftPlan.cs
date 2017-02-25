using System;

namespace Filter.Algorithms
{
    public abstract class FftPlan
    {
        protected FftPlan(int fftLength, IntPtr plan)
        {
            this.FftLength = fftLength;
            this.Plan = plan;
            FftwProvider.ExportWisdom();
        }

        public int FftLength { get; }
        public IntPtr Plan { get; }

        public abstract void ExecuteUnsafe(IntPtr input, IntPtr output);

        ~FftPlan()
        {
            FftwInterop.destroy_plan(this.Plan);
        }
    }
}