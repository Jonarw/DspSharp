using System;
using System.Collections.Generic;
using System.Linq;
using Filter.Algorithms;
using Filter.Extensions;
using Filter.Series;
using Filter.Signal;
using Filter.Signal.Windows;
using Filter.Spectrum;
using FilterPlot.Axes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PropertyTools.DataAnnotations;

namespace FilterPlot
{
    public abstract class SpectrumPlot : SignalPlot
    {
        public enum EnumerableModes
        {
            [Description("fixed FFT length")] Fixed,
            [Description("same as view")] View
        }

        public enum FiniteModes
        {
            [Description("regular (FFT length = signal length)")] Regular,
            [Description("oversampled")] Oversampled,
            [Description("fixed FFT length")] Fixed,
            [Description("same as view")] View
        }

        public enum InfiniteModes
        {
            [Description("fixed FFT length")] Fixed,
            [Description("same as view")] View,
            [Description("fixed, symmetric")] FixedSymmetric
        }

        public enum SyntheticModes
        {
            [Description("ideal synthetic")] Ideal,
            [Description("FFT-based, fixed length")] Fixed,
            [Description("same as view")] View,
            [Description("fixed, symmetric")] FixedSymmetric
        }

        private bool _CustomResulutionEnabled = false;
        private WindowTypes _EnumerableFiniteWindowType = WindowTypes.Hann;
        private int _EnumerableFixedLength = 1024;
        private int _EnumerableFixedStart;
        private EnumerableModes _EnumerableMode;
        private WindowModes _EnumerableWindowMode = WindowModes.Causal;
        private int _FiniteFixedLength = 1024;
        private int _FiniteFixedStart;
        private FiniteModes _FiniteMode;
        private int _FiniteOversampling = 4;
        private WindowModes _FiniteWindowMode = WindowModes.Causal;
        private WindowTypes _FiniteWindowType = WindowTypes.Rectangular;
        private WindowTypes _InfiniteFiniteWindowType = WindowTypes.Hann;
        private int _InfiniteFixedLength = 1024;
        private int _InfiniteFixedStart = -512;
        private int _InfiniteFixedSymmetricLength = 1024;
        private InfiniteModes _InfiniteMode;
        private WindowModes _InfiniteWindowMode = WindowModes.Symmetric;
        private bool _Logarithmic = true;
        private int _NumberOfPoints = 500;
        private int _Smoothing = 0;
        private double _StartFrequency = 20;
        private double _StopFrequency = 20000;
        private WindowTypes _SyntheticFiniteWindowType = WindowTypes.Hann;
        private int _SyntheticFixedLength = 1024;
        private int _SyntheticFixedStart = -512;
        private int _SyntheticFixedSymmetricLength = 1024;
        private SyntheticModes _SyntheticMode;
        private WindowModes _SyntheticWindowMode = WindowModes.Symmetric;
        private int _ViewLength;
        private int _ViewStart;
        public Window CausalWindow { get; set; } = new Window(WindowTypes.Hann, 8192, 44100, WindowModes.Causal);
        public Window SymmetricWindow { get; set; } = new Window(WindowTypes.Hann, 8192, 44100, WindowModes.Symmetric);

        public int ViewLength
        {
            get { return this._ViewLength; }
            set { this.SetField(ref this._ViewLength, value); }
        }

        public int ViewStart
        {
            get { return this._ViewStart; }
            set { this.SetField(ref this._ViewStart, value); }
        }

        private UniformSeries CustomFrequencies { get; set; }

        private Axis XAxis { get; set; }
        private Axis YAxis { get; set; }

        protected override Series CreateGraph(ISignal signal)
        {
            var ret = new LineSeries();
            var fsignal = signal as IFiniteSignal;
            var esignal = signal as IEnumerableSignal;
            var ssignal = signal as ISyntheticSignal;

            int winstart;
            int winlength;
            WindowModes winmode;
            WindowTypes wintype;

            if (fsignal != null)
            {
                winmode = this.FiniteWindowMode;
                wintype = this.FiniteWindowType;

                if (this.FiniteMode == FiniteModes.Fixed)
                {
                    winstart = this.FiniteFixedStart;
                    winlength = this.FiniteFixedLength;
                }
                else if (this.FiniteMode == FiniteModes.Regular)
                {
                    winstart = fsignal.Start;
                    winlength = fsignal.Length;
                }
                else if (this.FiniteMode == FiniteModes.Oversampled)
                {
                    winstart = fsignal.Start;
                    winlength = fsignal.Length * this.FiniteOversampling;
                }
                else if (this.FiniteMode == FiniteModes.View)
                {
                    winstart = this.ViewStart;
                    winlength = this.ViewLength;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            else if (esignal != null)
            {
                winmode = this.EnumerableWindowMode;
                wintype = this.EnumerableWindowType;

                if (this.EnumerableMode == EnumerableModes.Fixed)
                {
                    winstart = this.EnumerableFixedStart;
                    winlength = this.EnumerableFixedLength;
                }
                else if (this.EnumerableMode == EnumerableModes.View)
                {
                    winstart = this.ViewStart;
                    winlength = this.ViewLength;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            else if (ssignal != null)
            {
                winmode = this.SyntheticWindowMode;
                wintype = this.SyntheticWindowType;

                if (this.SyntheticMode == SyntheticModes.Ideal)
                {
                    ret.Points.AddRange(this.GetYValues(ssignal.Spectrum).Zip(ssignal.Spectrum.Frequencies.Values, (m, f) => new DataPoint(f, m)));
                    return ret;
                }
                if (this.SyntheticMode == SyntheticModes.Fixed)
                {
                    winstart = this.SyntheticFixedStart;
                    winlength = this.SyntheticFixedLength;
                }
                else if (this.SyntheticMode == SyntheticModes.View)
                {
                    winstart = this.ViewStart;
                    winlength = this.ViewLength;
                }
                else if (this.SyntheticMode == SyntheticModes.FixedSymmetric)
                {
                    winstart = -(this.SyntheticFixedSymmetricLength >> 1);
                    winlength = this.SyntheticFixedSymmetricLength;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                winmode = this.InfiniteWindowMode;
                wintype = this.InfiniteWindowType;

                if (this.InfiniteMode == InfiniteModes.Fixed)
                {
                    winstart = this.InfiniteFixedStart;
                    winlength = this.InfiniteFixedLength;
                }
                else if (this.InfiniteMode == InfiniteModes.View)
                {
                    winstart = this.ViewStart;
                    winlength = this.ViewLength;
                }
                else if (this.InfiniteMode == InfiniteModes.FixedSymmetric)
                {
                    winstart = -(this.InfiniteFixedSymmetricLength >> 1);
                    winlength = this.InfiniteFixedSymmetricLength;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            var win = new Window(wintype, winstart, winlength, signal.SampleRate, winmode);

            var winsignal = signal.Multiply(win);

            var frequencies = winsignal.Spectrum.Frequencies.Values.ToReadOnlyList();
            var values = this.GetYValues(winsignal.Spectrum).ToReadOnlyList();

            if (this.CustomResulutionEnabled)
            {
                if (this.CustomFrequencies == null)
                {
                    this.CustomFrequencies = new UniformSeries(this.StartFrequency, this.StopFrequency, this.NumberOfPoints, this.Logarithmic);
                }

                var fnew = this.CustomFrequencies.Values.ToReadOnlyList();

                values = Dsp.AdaptiveInterpolation(frequencies, values, fnew, this.Logarithmic).ToReadOnlyList();
                frequencies = fnew;
            }

            if (this.Smoothing > 0)
            {
                values = Dsp.Smooth(frequencies, values, this.Smoothing, this.Logarithmic).ToReadOnlyList();
            }

            ret.Points.AddRange(values.Zip(frequencies, (m, f) => new DataPoint(f, m)));
            return ret;
        }

        protected override Axis CreateXAxis()
        {
            if (this.XAxis == null)
            {
                this.XAxis = new FrequencyAxis();
            }

            return this.XAxis;
        }

        protected abstract IEnumerable<double> GetYValues(ISpectrum spectrum);

        [Category("display")]
        [DisplayName("smoothing")]
        public int Smoothing
        {
            get { return this._Smoothing; }
            set { this.SetField(ref this._Smoothing, value); }
        }

        [DisplayName("custom resolution")]
        public bool CustomResulutionEnabled
        {
            get { return this._CustomResulutionEnabled; }
            set { this.SetField(ref this._CustomResulutionEnabled, value); }
        }

        [DisplayName("number of points")]
        [VisibleBy(nameof(CustomResulutionEnabled))]
        public int NumberOfPoints
        {
            get { return this._NumberOfPoints; }
            set
            {
                this.SetField(ref this._NumberOfPoints, value);
                this.CustomFrequencies = null;
            }
        }

        [DisplayName("logarithmic")]
        [VisibleBy(nameof(CustomResulutionEnabled))]
        public bool Logarithmic
        {
            get { return this._Logarithmic; }
            set
            {
                this.SetField(ref this._Logarithmic, value);
                this.CustomFrequencies = null;
            }
        }

        [DisplayName("start frequency")]
        [VisibleBy(nameof(CustomResulutionEnabled))]
        public double StartFrequency
        {
            get { return this._StartFrequency; }
            set
            {
                this.SetField(ref this._StartFrequency, value);
                this.CustomFrequencies = null;
            }
        }

        [DisplayName("stop frequency")]
        [VisibleBy(nameof(CustomResulutionEnabled))]
        public double StopFrequency
        {
            get { return this._StopFrequency; }
            set
            {
                this.SetField(ref this._StopFrequency, value);
                this.CustomFrequencies = null;
            }
        }

        [Category("finite signals")]
        [DisplayName("window type")]
        public WindowTypes FiniteWindowType
        {
            get { return this._FiniteWindowType; }
            set { this.SetField(ref this._FiniteWindowType, value); }
        }

        [DisplayName("window mode")]
        public WindowModes FiniteWindowMode
        {
            get { return this._FiniteWindowMode; }
            set { this.SetField(ref this._FiniteWindowMode, value); }
        }

        [DisplayName("window length mode")]
        public FiniteModes FiniteMode
        {
            get { return this._FiniteMode; }
            set { this.SetField(ref this._FiniteMode, value); }
        }

        [DisplayName("window start")]
        [VisibleBy(nameof(FiniteMode), FiniteModes.Fixed)]
        public int FiniteFixedStart
        {
            get { return this._FiniteFixedStart; }
            set { this.SetField(ref this._FiniteFixedStart, value); }
        }

        [DisplayName("window length")]
        [VisibleBy(nameof(FiniteMode), FiniteModes.Fixed)]
        public int FiniteFixedLength
        {
            get { return this._FiniteFixedLength; }
            set { this.SetField(ref this._FiniteFixedLength, value); }
        }

        [DisplayName("oversampling factor")]
        [VisibleBy(nameof(FiniteMode), FiniteModes.Oversampled)]
        public int FiniteOversampling
        {
            get { return this._FiniteOversampling; }
            set { this.SetField(ref this._FiniteOversampling, value); }
        }

        [Category("enumerable signals")]
        [DisplayName("window type")]
        public WindowTypes EnumerableWindowType
        {
            get { return this._EnumerableFiniteWindowType; }
            set { this.SetField(ref this._EnumerableFiniteWindowType, value); }
        }

        [DisplayName("window mode")]
        public WindowModes EnumerableWindowMode
        {
            get { return this._EnumerableWindowMode; }
            set { this.SetField(ref this._EnumerableWindowMode, value); }
        }

        [DisplayName("window length mode")]
        public EnumerableModes EnumerableMode
        {
            get { return this._EnumerableMode; }
            set { this.SetField(ref this._EnumerableMode, value); }
        }

        [DisplayName("window start")]
        [VisibleBy(nameof(EnumerableMode), EnumerableModes.Fixed)]
        public int EnumerableFixedStart
        {
            get { return this._EnumerableFixedStart; }
            set { this.SetField(ref this._EnumerableFixedStart, value); }
        }

        [DisplayName("window length")]
        [VisibleBy(nameof(EnumerableMode), EnumerableModes.Fixed)]
        public int EnumerableFixedLength
        {
            get { return this._EnumerableFixedLength; }
            set { this.SetField(ref this._EnumerableFixedLength, value); }
        }

        [Category("infinite signals")]
        [DisplayName("window type")]
        public WindowTypes InfiniteWindowType
        {
            get { return this._InfiniteFiniteWindowType; }
            set { this.SetField(ref this._InfiniteFiniteWindowType, value); }
        }

        [DisplayName("window mode")]
        public WindowModes InfiniteWindowMode
        {
            get { return this._InfiniteWindowMode; }
            set { this.SetField(ref this._InfiniteWindowMode, value); }
        }

        [DisplayName("window length mode")]
        public InfiniteModes InfiniteMode
        {
            get { return this._InfiniteMode; }
            set { this.SetField(ref this._InfiniteMode, value); }
        }

        [DisplayName("window start")]
        [VisibleBy(nameof(InfiniteMode), InfiniteModes.Fixed)]
        public int InfiniteFixedStart
        {
            get { return this._InfiniteFixedStart; }
            set { this.SetField(ref this._InfiniteFixedStart, value); }
        }

        [DisplayName("window length")]
        [VisibleBy(nameof(InfiniteMode), InfiniteModes.Fixed)]
        public int InfiniteFixedLength
        {
            get { return this._InfiniteFixedLength; }
            set { this.SetField(ref this._InfiniteFixedLength, value); }
        }

        [DisplayName("window length")]
        [VisibleBy(nameof(InfiniteMode), InfiniteModes.FixedSymmetric)]
        public int InfiniteFixedSymmetricLength
        {
            get { return this._InfiniteFixedSymmetricLength; }
            set { this.SetField(ref this._InfiniteFixedSymmetricLength, value); }
        }

        [Category("synthetic signals")]
        [DisplayName("window type")]
        public WindowTypes SyntheticWindowType
        {
            get { return this._SyntheticFiniteWindowType; }
            set { this.SetField(ref this._SyntheticFiniteWindowType, value); }
        }

        [DisplayName("window mode")]
        public WindowModes SyntheticWindowMode
        {
            get { return this._SyntheticWindowMode; }
            set { this.SetField(ref this._SyntheticWindowMode, value); }
        }

        [DisplayName("window length mode")]
        public SyntheticModes SyntheticMode
        {
            get { return this._SyntheticMode; }
            set { this.SetField(ref this._SyntheticMode, value); }
        }

        [DisplayName("window start")]
        [VisibleBy(nameof(SyntheticMode), SyntheticModes.Fixed)]
        public int SyntheticFixedStart
        {
            get { return this._SyntheticFixedStart; }
            set { this.SetField(ref this._SyntheticFixedStart, value); }
        }

        [DisplayName("window length")]
        [VisibleBy(nameof(SyntheticMode), SyntheticModes.Fixed)]
        public int SyntheticFixedLength
        {
            get { return this._SyntheticFixedLength; }
            set { this.SetField(ref this._SyntheticFixedLength, value); }
        }

        [DisplayName("window length")]
        [VisibleBy(nameof(SyntheticMode), SyntheticModes.FixedSymmetric)]
        public int SyntheticFixedSymmetricLength
        {
            get { return this._SyntheticFixedSymmetricLength; }
            set { this.SetField(ref this._SyntheticFixedSymmetricLength, value); }
        }
    }
}