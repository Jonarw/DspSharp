namespace Filter.Algorithms.FftwProvider
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
            FftwInterop.destroy_plan(this.Plan);
        }
    }
}