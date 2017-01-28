using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Filter.Algorithms.FFTWSharp;

namespace Filter_Win
{
    /// <summary>
    ///     Plan for a real-valued IFFT.
    /// </summary>
    public class InverseRealFftPlan : RealFftPlan
    {
        private IEnumerable<Complex> _Input;
        private IReadOnlyList<double> _Output;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InverseRealFftPlan" /> class.
        /// </summary>
        /// <param name="fftLength">The FFT length the plan is used for.</param>
        public InverseRealFftPlan(int fftLength) : base(fftLength)
        {
            this.FftwP = fftw_plan.dft_c2r_1d(this.N, this.FftwC, this.FftwR, fftw_flags.Measure);
        }

        /// <summary>
        ///     Executes the plan for the provided data.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <returns>The IFFT of the input data.</returns>
        public IReadOnlyList<double> Execute(IEnumerable<Complex> input)
        {
            this.FftwC.SetData(input.ToArray());
            this.FftwP.Execute();
            return this.FftwR.GetData();
        }
    }
}