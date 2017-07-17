// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FftwProvider.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DspSharp.Algorithms;

namespace DspSharpFftw
{
    public class FftwProvider : IFftProvider
    {
        private Dictionary<int, ComplexToComplexFftPlan> ComplexForwardPlans { get; } =
            new Dictionary<int, ComplexToComplexFftPlan>();

        private Dictionary<int, ComplexToComplexFftPlan> ComplexInversePlans { get; } =
            new Dictionary<int, ComplexToComplexFftPlan>();

        private Dictionary<int, int> OptimalFftLengths { get; } = new Dictionary<int, int>();

        public IReadOnlyList<Complex> ComplexFft(IReadOnlyList<Complex> input, int n = -1, NormalizationKind normalization = NormalizationKind.None)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (n < 0)
                n = input.Count;

            if (n == 0)
                return Enumerable.Empty<Complex>().ToReadOnlyList();

            if (!this.ComplexForwardPlans.ContainsKey(n))
                this.ComplexForwardPlans.Add(n, new ComplexToComplexFftPlan(n, FftwDirection.Forward));

            var plan = this.ComplexForwardPlans[n];
            return plan.Execute(input.ToArrayOptimized(), normalization);
        }

        public IReadOnlyList<Complex> ComplexIfft(IReadOnlyList<Complex> input, NormalizationKind normalization = NormalizationKind.N)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Count == 0)
                return Enumerable.Empty<Complex>().ToReadOnlyList();

            var n = input.Count;

            if (!this.ComplexInversePlans.ContainsKey(n))
                this.ComplexInversePlans.Add(n, new ComplexToComplexFftPlan(n, FftwDirection.Backward));

            var plan = this.ComplexInversePlans[n];
            return plan.Execute(input.ToArrayOptimized(), normalization);
        }

        public int GetOptimalFftLength(int originalLength)
        {
            if (!this.OptimalFftLengths.ContainsKey(originalLength))
            {
                var ret = originalLength - 1;
                int i;

                do
                {
                    ret++;
                    i = ret;

                    while (i % 2 == 0)
                    {
                        i /= 2;
                    }

                    while (i % 3 == 0)
                    {
                        i /= 3;
                    }

                    while (i % 5 == 0)
                    {
                        i /= 5;
                    }

                    while (i % 7 == 0)
                    {
                        i /= 7;
                    }
                }
                while (i > 7);

                this.OptimalFftLengths.Add(originalLength, ret);
            }

            return this.OptimalFftLengths[originalLength];
        }

        /// <summary>
        ///     Computes the FFT over real-valued input data. Only the positive half of the hermitian symmetric fourier spectrum is
        ///     returned.
        /// </summary>
        /// <param name="input">The real-valued input data.</param>
        /// <param name="n">The desired fft length. If set, the <paramref name="input" /> is zero-padded to <paramref name="n" />.</param>
        /// <returns>The positive half of the hermitian-symmetric spectrum, including DC and Nyquist/2.</returns>
        public IReadOnlyList<Complex> RealFft(IReadOnlyList<double> input, int n = -1, NormalizationKind normalization = NormalizationKind.None)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (n < 0)
                n = input.Count;

            if (n == 0)
                return Enumerable.Empty<Complex>().ToReadOnlyList();

            var plan = ForwardRealFftPlan.GetPlan(n);
            return plan.Execute(input.ToArrayOptimized(), normalization);
        }

        /// <summary>
        ///     Computes the iFFT over the positive half of a hermitian-symmetric spectrum.
        /// </summary>
        /// <param name="input">The positive half of a hermitian-symmetric spectrum.</param>
        /// <returns>The computed time-domain values. Always has an even length.</returns>
        public IReadOnlyList<double> RealIfft(IReadOnlyList<Complex> input, int n = -1, NormalizationKind normalization = NormalizationKind.N)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Count == 0)
                return Enumerable.Empty<double>().ToReadOnlyList();

            if (n > 0 && input.Count != n / 2 + 1)
                throw new ArgumentOutOfRangeException(nameof(n));

            if (n < 0)
            {
                if (Math.Abs(input[input.Count - 1].Imaginary) > 1e-13)
                    n = (input.Count << 1) - 1;
                else
                    n = (input.Count - 1) << 1;
            }

            var plan = InverseRealFftPlan.GetPlan(n);
            return plan.Execute(input.ToArrayOptimized(), normalization);
        }
    }
}