using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Filter.Algorithms;
using Filter.Algorithms.FFTWSharp;
using Filter.Extensions;
using Filter.Signal;

namespace Filter_Win
{
    public class FftwProvider : IFftProvider
    {
        /// <summary>
        ///     Performs necessary initializations.
        /// </summary>
        public FftwProvider()
        {
            var fi = new FileInfo(this.WisdomPath);
            fi.Directory?.Create();
            try
            {
                fftw.import_wisdom_from_filename(this.WisdomPath);
            }
            catch (Exception)
            {
                // no wisdom exists (yet)
            }

            AppDomain.CurrentDomain.ProcessExit += this.ExportWisdom;
        }

        private string WisdomPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                             "\\fftw\\fftw_real.wsd";

        private Dictionary<int, ForwardRealFftPlan> RealForwardPlans { get; } = new Dictionary<int, ForwardRealFftPlan>();
        private Dictionary<int, InverseRealFftPlan> RealInversePlans { get; } = new Dictionary<int, InverseRealFftPlan>();
        private Dictionary<int, ComplexFftPlan> ComplexForwardPlans { get; } = new Dictionary<int, ComplexFftPlan>();
        private Dictionary<int, ComplexFftPlan> ComplexInversePlans { get; } = new Dictionary<int, ComplexFftPlan>();

        public IReadOnlyList<Complex> ComplexFft(IReadOnlyList<Complex> input, int n = -1)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (n < 0)
            {
                n = input.Count;
            }

            if (n == 0)
            {
                return Enumerable.Empty<Complex>().ToReadOnlyList();
            }

            if (!this.ComplexForwardPlans.ContainsKey(n))
            {
                this.ComplexForwardPlans.Add(n, new ComplexFftPlan(n, fftw_direction.Forward));
            }

            var plan = this.ComplexForwardPlans[n];
            return plan.Execute(input.ZeroPad(n));
        }

        private Dictionary<int, int> OptimalFftLengths { get; } = new Dictionary<int, int>(); 

        public int GetOptimalFftLength(int originalLength)
        {
            if (!this.OptimalFftLengths.ContainsKey(originalLength))
            {
                int ret = originalLength - 1;
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

        public IReadOnlyList<Complex> ComplexIfft(IReadOnlyList<Complex> input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.Count == 0)
            {
                return Enumerable.Empty<Complex>().ToReadOnlyList();
            }

            var n = input.Count;

            if (!this.ComplexInversePlans.ContainsKey(n))
            {
                this.ComplexInversePlans.Add(n, new ComplexFftPlan(n, fftw_direction.Backward));
            }

            var plan = this.ComplexInversePlans[n];
            return plan.Execute(input).Multiply(1.0 / n).ToReadOnlyList();
        }

        /// <summary>
        ///     Computes the FFT over real-valued input data. Only the positive half of the hermitian symmetric fourier spectrum is
        ///     returned.
        /// </summary>
        /// <param name="input">The real-valued input data.</param>
        /// <param name="n">The desired fft length. If set, the <paramref name="input" /> is zero-padded to <paramref name="n" />.</param>
        /// <returns>The positive half of the hermitian-symmetric spectrum, including DC and Nyquist/2.</returns>
        public IReadOnlyList<Complex> RealFft(IReadOnlyList<double> input, int n = -1)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (n < 0)
            {
                n = input.Count;
            }

            if (n == 0)
            {
                return Enumerable.Empty<Complex>().ToReadOnlyList();
            }

            if (!this.RealForwardPlans.ContainsKey(n))
            {
                this.RealForwardPlans.Add(n, new ForwardRealFftPlan(n));
            }

            var plan = this.RealForwardPlans[n];
            return plan.Execute(input.ZeroPad(n));
        }

        /// <summary>
        ///     Computes the iFFT over the positive half of a hermitian-symmetric spectrum.
        /// </summary>
        /// <param name="input">The positive half of a hermitian-symmetric spectrum.</param>
        /// <returns>The computed time-domain values. Always has an even length.</returns>
        public IReadOnlyList<double> RealIfft(IReadOnlyList<Complex> input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.Count == 0)
            {
                return Enumerable.Empty<double>().ToReadOnlyList();
            }

            int n;
            if (Math.Abs(input[input.Count - 1].Imaginary) > 1e-13)
            {
                n = (input.Count << 1) - 1;
            }
            else
            {
                n = (input.Count - 1) << 1;
            }

            if (!this.RealInversePlans.ContainsKey(n))
            {
                this.RealInversePlans.Add(n, new InverseRealFftPlan(n));
            }

            var plan = this.RealInversePlans[n];
            return plan.Execute(input).Multiply(1.0 / n).ToReadOnlyList();
        }

        /// <summary>
        ///     Exports the accumulated wisdom to a file for later use.
        /// </summary>
        /// <param name="sender">Sender variable for event handler.</param>
        /// <param name="e">Event handler arguments.</param>
        private void ExportWisdom(object sender, EventArgs e)
        {
            fftw.export_wisdom_to_filename(this.WisdomPath);
        }
    }
}