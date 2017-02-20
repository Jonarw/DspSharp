using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Filter_Win.FFTWSharp;

namespace Filter_Win
{
    /// <summary>
    ///     Handles the creating of an fftw plan and the associated memory blocks.
    /// </summary>
    public class ComplexToComplexFftPlan
    {
        /// <summary>
        ///     Initializes a new instance of the base class <see cref="ComplexToComplexFftPlan" />.
        /// </summary>
        /// <param name="fftLength">The FFT lenght the plan is used for.</param>
        /// <param name="direction">The FFT direction.</param>
        public ComplexToComplexFftPlan(int fftLength, FftwDirection direction)
        {
            this.N = fftLength; 
            this.FftwIn = new FftwComplexarray(this.N);
            this.FftwOut = new FftwComplexarray(this.N);
            this.FftwP = FftwPlan.Dft1D(this.N, this.FftwIn, this.FftwOut, direction, FftwFlags.Measure | FftwFlags.DestroyInput);
        }

        /// <summary>
        ///     The FFT length the plan is used for.
        /// </summary>
        public int N { get; }

        /// <summary>
        ///     The FFTW plan.
        /// </summary>
        protected FftwPlan FftwP { get; set; }

        /// <summary>
        ///     The unmanaged data array for the real values.
        /// </summary>
        protected FftwComplexarray FftwIn { get; set; }

        /// <summary>
        ///     The unmanaged data array for the complex values.
        /// </summary>
        protected FftwComplexarray FftwOut { get; set; }

        /// <summary>
        ///     Executes the plan for the provided data.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <returns>The (I)FFT of the input data.</returns>
        public IReadOnlyList<Complex> Execute(IEnumerable<Complex> input)
        {
            this.FftwIn.SetData(input);
            this.FftwP.Execute();
            return this.FftwOut.GetData();
        }

    }
}