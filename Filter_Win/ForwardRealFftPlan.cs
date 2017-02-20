using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Filter_Win.FFTWSharp;

namespace Filter_Win
{
    /// <summary>
    ///     Plan for a real-valued forward FFT.
    /// </summary>
    public class ForwardRealFftPlan : RealToComplexFftPlan
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ForwardRealFftPlan" /> class.
        /// </summary>
        /// <param name="fftLength">The FFT length the plan is used for.</param>
        public ForwardRealFftPlan(int fftLength) : base(fftLength)
        {
            this.FftwP = FftwPlan.DftRealToComplex1D(this.N, this.FftwR, this.FftwC, FftwFlags.Measure | FftwFlags.DestroyInput);
        }

        /// <summary>
        ///     Executes the plan for the provided data.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <returns>The FFT of the input data.</returns>
        public IReadOnlyList<Complex> Execute(IEnumerable<double> input)
        {
            this.FftwR.SetData(input.ToArray());
            this.FftwP.Execute();
            return this.FftwC.GetData();
        }
    }
}