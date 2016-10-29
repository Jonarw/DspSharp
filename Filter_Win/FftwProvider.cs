using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Filter.Algorithms;
using Filter.Algorithms.FFTWSharp;
using Filter.Extensions;

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

        private Dictionary<int, ForwardFftPlan> ForwardPlans { get; } = new Dictionary<int, ForwardFftPlan>();
        private Dictionary<int, InverseFftPlan> InversePlans { get; } = new Dictionary<int, InverseFftPlan>();

        /// <summary>
        ///     Computes the FFT over real-valued input data. Only the positive half of the hermitian symmetric fourier spectrum is
        ///     returned.
        /// </summary>
        /// <param name="input">The real-valued input data.</param>
        /// <param name="n">The desired fft length. If set, the <paramref name="input" /> is zero-padded to <paramref name="n" />.</param>
        /// <returns>The positive half of the hermitian-symmetric spectrum, including DC and Nyquist/2.</returns>
        public IReadOnlyList<Complex> RealFft(IReadOnlyList<double> input, int n = -1)
        {
            if (n < 0)
            {
                n = input.Count;
            }

            if (!this.ForwardPlans.ContainsKey(n))
            {
                this.ForwardPlans.Add(n, new ForwardFftPlan(n));
            }

            var plan = this.ForwardPlans[n];
            plan.Input = input.ZeroPad(n);
            return plan.Output.ToReadOnlyList();
        }

        /// <summary>
        ///     Computes the iFFT over the positive half of a hermitian-symmetric spectrum.
        /// </summary>
        /// <param name="input">The positive half of a hermitian-symmetric spectrum.</param>
        /// <returns>The computed time-domain values. Always has an even length.</returns>
        public IReadOnlyList<double> RealIfft(IReadOnlyList<Complex> input)
        {
            var n = (input.Count - 1) << 1;

            if (!this.InversePlans.ContainsKey(n))
            {
                this.InversePlans.Add(n, new InverseFftPlan(n));
            }

            var plan = this.InversePlans[n];
            plan.Input = input;
            return plan.Output.ToList();
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

    /// <summary>
    ///     Handles the creating of an fftw plan and the associated memory blocks.
    /// </summary>
    public abstract class FftPlan
    {
        /// <summary>
        ///     Initializes a new instance of the base class <see cref="FftPlan" />.
        /// </summary>
        /// <param name="fftLength">The FFT lenght the plan is used for.</param>
        protected FftPlan(int fftLength)
        {
            this.N = fftLength;
            this.FftwR = new fftw_realarray(this.N);
            this.FftwC = new fftw_complexarray((this.N >> 1) + 1);
        }

        /// <summary>
        ///     The FFT length the plan is used for.
        /// </summary>
        public int N { get; }

        /// <summary>
        ///     The FFTW plan.
        /// </summary>
        protected fftw_plan FftwP { get; set; }

        /// <summary>
        ///     The unmanaged data array for the real values.
        /// </summary>
        protected fftw_realarray FftwR { get; set; }

        /// <summary>
        ///     The unmanaged data array for the complex values.
        /// </summary>
        protected fftw_complexarray FftwC { get; set; }
    }

    /// <summary>
    ///     Plan for a real-valued forward FFT.
    /// </summary>
    public class ForwardFftPlan : FftPlan
    {
        private IEnumerable<double> _Input;
        private IReadOnlyList<Complex> _Output;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ForwardFftPlan" /> class.
        /// </summary>
        /// <param name="fftLength">The FFT length the plan is used for.</param>
        public ForwardFftPlan(int fftLength) : base(fftLength)
        {
            this.FftwP = fftw_plan.dft_r2c_1d(this.N, this.FftwR, this.FftwC, fftw_flags.Measure);
        }

        /// <summary>
        ///     The real-valued input data, i.e. the time samples.
        /// </summary>
        public IEnumerable<double> Input
        {
            get { return this._Input; }
            set
            {
                this._Input = value;
                this._Output = null;
            }
        }

        /// <summary>
        ///     The complex-valued result, i.e. the positive half of the hermitian-symmatric fourier spectrum of the previously
        ///     provided input data.
        /// </summary>
        public IReadOnlyList<Complex> Output
        {
            get
            {
                if (this._Output == null)
                {
                    this.FftwR.SetData(this.Input.ToArray());
                    this.FftwP.Execute();
                    this._Output = this.FftwC.GetData().ToReadOnlyList();
                }
                return this._Output;
            }
        }
    }

    /// <summary>
    ///     Plan for a real-valued iFFT.
    /// </summary>
    public class InverseFftPlan : FftPlan
    {
        private IEnumerable<Complex> _Input;
        private IReadOnlyList<double> _Output;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InverseFftPlan" /> class.
        /// </summary>
        /// <param name="fftLength">The FFT length the plan is used for.</param>
        public InverseFftPlan(int fftLength) : base(fftLength)
        {
            this.FftwP = fftw_plan.dft_c2r_1d(this.N, this.FftwC, this.FftwR, fftw_flags.Measure);
        }

        /// <summary>
        ///     The complex valued input data, i.e. the positive half of a hermitian symmatric fourier spectrum.
        /// </summary>
        public IEnumerable<Complex> Input
        {
            get { return this._Input; }
            set
            {
                this._Input = value;
                this._Output = null;
            }
        }

        /// <summary>
        ///     The real-valued result, i.e. the time samples.
        /// </summary>
        public IReadOnlyList<double> Output
        {
            get
            {
                if (this._Output == null)
                {
                    this.FftwC.SetData(this.Input);
                    this.FftwP.Execute();
                    this._Output = this.FftwR.GetData().ToReadOnlyList();
                }
                return this._Output;
            }
            protected set { this._Output = value; }
        }
    }
}