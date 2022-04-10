// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FftwProvider.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DspSharp.Algorithms;

namespace DspSharpFftw
{
    public class FftwProvider : IFftProvider
    {
        private readonly object planningLock = new object();
        private readonly Dictionary<int, ComplexToComplexFftPlan> ComplexForwardPlans = new Dictionary<int, ComplexToComplexFftPlan>();
        private readonly Dictionary<int, ForwardRealFftPlan> RealForwardPlans = new Dictionary<int, ForwardRealFftPlan>();
        private readonly Dictionary<int, InverseRealFftPlan> RealInversePlans = new Dictionary<int, InverseRealFftPlan>();
        private readonly Dictionary<int, ComplexToComplexFftPlan> ComplexInversePlans = new Dictionary<int, ComplexToComplexFftPlan>();

        private Dictionary<int, int> OptimalFftLengths { get; } = new Dictionary<int, int>();

        /// <inheritdoc/>
        public Complex[] ComplexFft(IReadOnlyList<Complex> input)
        {
            ComplexToComplexFftPlan plan;
            lock (this.planningLock)
            {
                if (!this.ComplexForwardPlans.TryGetValue(input.Count, out plan))
                {
                    plan = new ComplexToComplexFftPlan(input.Count, FftwDirection.Forward);
                    this.ComplexForwardPlans.Add(input.Count, plan);
                }
            }

            return plan.Execute(input, NormalizationKind.None);
        }

        public Complex[] ComplexIfft(IReadOnlyList<Complex> input)
        {
            ComplexToComplexFftPlan plan;
            lock (this.planningLock)
            {
                if (!this.ComplexInversePlans.TryGetValue(input.Count, out plan))
                {
                    plan = new ComplexToComplexFftPlan(input.Count, FftwDirection.Backward);
                    this.ComplexInversePlans.Add(input.Count, plan);
                }
            }

            return plan.Execute(input, NormalizationKind.N);
        }

        public int GetOptimalFftLength(int minLength)
        {
            // fftw does mixed-radix ffts with prime factors 2, 3, 5 and 7
            if (!this.OptimalFftLengths.ContainsKey(minLength))
            {
                var ret = minLength - 1;
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

                this.OptimalFftLengths.Add(minLength, ret);
            }

            return this.OptimalFftLengths[minLength];
        }

        /// <inheritdoc/>
        public Complex[] RealFft(IReadOnlyList<double> input)
        {
            ForwardRealFftPlan plan;
            lock (this.planningLock)
            {
                if (!this.RealForwardPlans.TryGetValue(input.Count, out plan))
                {
                    plan = new ForwardRealFftPlan(input.Count);
                    this.RealForwardPlans.Add(input.Count, plan);
                }
            }

            return plan.Execute(input, NormalizationKind.None);
        }

        /// <inheritdoc/>
        public double[] RealIfft(IReadOnlyList<Complex> input, bool isEven)
        {
            InverseRealFftPlan plan;
            var n = isEven ? (input.Count << 1) - 1 : (input.Count - 1) << 1;

            lock (this.planningLock)
            {
                if (!this.RealInversePlans.TryGetValue(n, out plan))
                {
                    plan = new InverseRealFftPlan(n);
                    this.RealInversePlans.Add(n, plan);
                }
            }

            return plan.Execute(input, NormalizationKind.N);
        }
    }
}