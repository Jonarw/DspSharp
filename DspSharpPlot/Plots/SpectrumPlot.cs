// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpectrumPlot.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DspSharp.Algorithms;
using DspSharp.Extensions;
using DspSharp.Series;
using DspSharp.Signal;
using DspSharp.Signal.Windows;
using DspSharp.Spectrum;
using DspSharpPlot.Axes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using UTilities.Extensions;

namespace DspSharpPlot
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
            [Description("regular (FFT length = signal length)")]
            Regular,
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

            [Description("FFT-based, fixed length")]
            Fixed,
            [Description("same as view")] View,
            [Description("fixed, symmetric")] FixedSymmetric
        }

        private bool _CustomResulutionEnabled;
        private WindowType _EnumerableFiniteWindowType = WindowType.Hann;
        private int _EnumerableFixedLength = 1024;
        private int _EnumerableFixedStart;
        private EnumerableModes _EnumerableMode;
        private WindowModes _EnumerableWindowMode = WindowModes.Causal;
        private int _FiniteFixedLength = 1024;
        private int _FiniteFixedStart;
        private FiniteModes _FiniteMode;
        private int _FiniteOversampling = 4;
        private WindowModes _FiniteWindowMode = WindowModes.Causal;
        private WindowType _FiniteWindowType = WindowType.Rectangular;
        private WindowType _InfiniteFiniteWindowType = WindowType.Hann;
        private int _InfiniteFixedLength = 1024;
        private int _InfiniteFixedStart = -512;
        private int _InfiniteFixedSymmetricLength = 1024;
        private InfiniteModes _InfiniteMode;
        private WindowModes _InfiniteWindowMode = WindowModes.Symmetric;
        private bool _Logarithmic = true;
        private int _NumberOfPoints = 500;
        private int _Smoothing;
        private double _StartFrequency = 20;
        private double _StopFrequency = 20000;
        private WindowType _SyntheticFiniteWindowType = WindowType.Hann;
        private int _SyntheticFixedLength = 1024;
        private int _SyntheticFixedStart = -512;
        private int _SyntheticFixedSymmetricLength = 1024;
        private SyntheticModes _SyntheticMode;
        private WindowModes _SyntheticWindowMode = WindowModes.Symmetric;
        private int _ViewLength;
        private int _ViewStart;
        public Window CausalWindow { get; set; } = new Window(WindowType.Hann, 8192, 44100, WindowModes.Causal);

        public bool CustomResulutionEnabled
        {
            get => this._CustomResulutionEnabled;
            set => this.SetField(ref this._CustomResulutionEnabled, value);
        }

        public int EnumerableFixedLength
        {
            get => this._EnumerableFixedLength;
            set => this.SetField(ref this._EnumerableFixedLength, value);
        }

        public int EnumerableFixedStart
        {
            get => this._EnumerableFixedStart;
            set => this.SetField(ref this._EnumerableFixedStart, value);
        }

        public EnumerableModes EnumerableMode
        {
            get => this._EnumerableMode;
            set => this.SetField(ref this._EnumerableMode, value);
        }

        public WindowModes EnumerableWindowMode
        {
            get => this._EnumerableWindowMode;
            set => this.SetField(ref this._EnumerableWindowMode, value);
        }

        public WindowType EnumerableWindowType
        {
            get => this._EnumerableFiniteWindowType;
            set => this.SetField(ref this._EnumerableFiniteWindowType, value);
        }

        public int FiniteFixedLength
        {
            get => this._FiniteFixedLength;
            set => this.SetField(ref this._FiniteFixedLength, value);
        }

        public int FiniteFixedStart
        {
            get => this._FiniteFixedStart;
            set => this.SetField(ref this._FiniteFixedStart, value);
        }

        public FiniteModes FiniteMode
        {
            get => this._FiniteMode;
            set => this.SetField(ref this._FiniteMode, value);
        }

        public int FiniteOversampling
        {
            get => this._FiniteOversampling;
            set => this.SetField(ref this._FiniteOversampling, value);
        }

        public WindowModes FiniteWindowMode
        {
            get => this._FiniteWindowMode;
            set => this.SetField(ref this._FiniteWindowMode, value);
        }

        public WindowType FiniteWindowType
        {
            get => this._FiniteWindowType;
            set => this.SetField(ref this._FiniteWindowType, value);
        }

        public int InfiniteFixedLength
        {
            get => this._InfiniteFixedLength;
            set => this.SetField(ref this._InfiniteFixedLength, value);
        }

        public int InfiniteFixedStart
        {
            get => this._InfiniteFixedStart;
            set => this.SetField(ref this._InfiniteFixedStart, value);
        }

        public int InfiniteFixedSymmetricLength
        {
            get => this._InfiniteFixedSymmetricLength;
            set => this.SetField(ref this._InfiniteFixedSymmetricLength, value);
        }

        public InfiniteModes InfiniteMode
        {
            get => this._InfiniteMode;
            set => this.SetField(ref this._InfiniteMode, value);
        }

        public WindowModes InfiniteWindowMode
        {
            get => this._InfiniteWindowMode;
            set => this.SetField(ref this._InfiniteWindowMode, value);
        }

        public WindowType InfiniteWindowType
        {
            get => this._InfiniteFiniteWindowType;
            set => this.SetField(ref this._InfiniteFiniteWindowType, value);
        }

        public bool Logarithmic
        {
            get => this._Logarithmic;
            set
            {
                this.SetField(ref this._Logarithmic, value);
                this.CustomFrequencies = null;
            }
        }

        public int NumberOfPoints
        {
            get => this._NumberOfPoints;
            set
            {
                this.SetField(ref this._NumberOfPoints, value);
                this.CustomFrequencies = null;
            }
        }

        public int Smoothing
        {
            get => this._Smoothing;
            set => this.SetField(ref this._Smoothing, value);
        }

        public double StartFrequency
        {
            get => this._StartFrequency;
            set
            {
                this.SetField(ref this._StartFrequency, value);
                this.CustomFrequencies = null;
            }
        }

        public double StopFrequency
        {
            get => this._StopFrequency;
            set
            {
                this.SetField(ref this._StopFrequency, value);
                this.CustomFrequencies = null;
            }
        }

        public Window SymmetricWindow { get; set; } = new Window(WindowType.Hann, 8192, 44100, WindowModes.Symmetric);

        public int SyntheticFixedLength
        {
            get => this._SyntheticFixedLength;
            set => this.SetField(ref this._SyntheticFixedLength, value);
        }

        public int SyntheticFixedStart
        {
            get => this._SyntheticFixedStart;
            set => this.SetField(ref this._SyntheticFixedStart, value);
        }

        public int SyntheticFixedSymmetricLength
        {
            get => this._SyntheticFixedSymmetricLength;
            set => this.SetField(ref this._SyntheticFixedSymmetricLength, value);
        }

        public SyntheticModes SyntheticMode
        {
            get => this._SyntheticMode;
            set => this.SetField(ref this._SyntheticMode, value);
        }

        public WindowModes SyntheticWindowMode
        {
            get => this._SyntheticWindowMode;
            set => this.SetField(ref this._SyntheticWindowMode, value);
        }

        public WindowType SyntheticWindowType
        {
            get => this._SyntheticFiniteWindowType;
            set => this.SetField(ref this._SyntheticFiniteWindowType, value);
        }

        public int ViewLength
        {
            get => this._ViewLength;
            set => this.SetField(ref this._ViewLength, value);
        }

        public int ViewStart
        {
            get => this._ViewStart;
            set => this.SetField(ref this._ViewStart, value);
        }

        public override Axis XAxis { get; } = new FrequencyAxis();

        private UniformSeries CustomFrequencies { get; set; }

        protected override Series CreateGraph(ISignal signal)
        {
            var ret = new LineSeriesEx();
            var fsignal = signal as IFiniteSignal;
            var esignal = signal as IEnumerableSignal;
            var ssignal = signal as ISyntheticSignal;

            int winstart;
            int winlength;
            WindowModes winmode;
            WindowType wintype;

            if (fsignal != null)
            {
                winmode = this.FiniteWindowMode;
                wintype = this.FiniteWindowType;

                if (this.FiniteMode == FiniteModes.Fixed)
                {
                    winstart = this.FiniteFixedStart;
                    winlength = this.FiniteFixedLength;
                }
                else
                {
                    if (this.FiniteMode == FiniteModes.Regular)
                    {
                        winstart = fsignal.Start;
                        winlength = fsignal.Length;
                    }
                    else
                    {
                        if (this.FiniteMode == FiniteModes.Oversampled)
                        {
                            winstart = fsignal.Start;
                            winlength = fsignal.Length * this.FiniteOversampling;
                        }
                        else
                        {
                            if (this.FiniteMode == FiniteModes.View)
                            {
                                winstart = this.ViewStart;
                                winlength = this.ViewLength;
                            }
                            else
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            else
            {
                if (esignal != null)
                {
                    winmode = this.EnumerableWindowMode;
                    wintype = this.EnumerableWindowType;

                    if (this.EnumerableMode == EnumerableModes.Fixed)
                    {
                        winstart = this.EnumerableFixedStart;
                        winlength = this.EnumerableFixedLength;
                    }
                    else
                    {
                        if (this.EnumerableMode == EnumerableModes.View)
                        {
                            winstart = this.ViewStart;
                            winlength = this.ViewLength;
                        }
                        else
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    if (ssignal != null)
                    {
                        winmode = this.SyntheticWindowMode;
                        wintype = this.SyntheticWindowType;

                        if (this.SyntheticMode == SyntheticModes.Ideal)
                        {
                            ret.Points.AddRange(
                                this.GetYValues(ssignal.Spectrum).Zip(ssignal.Spectrum.Frequencies.Values, (m, f) => new DataPoint(f, m)));
                            return ret;
                        }

                        if (this.SyntheticMode == SyntheticModes.Fixed)
                        {
                            winstart = this.SyntheticFixedStart;
                            winlength = this.SyntheticFixedLength;
                        }
                        else
                        {
                            if (this.SyntheticMode == SyntheticModes.View)
                            {
                                winstart = this.ViewStart;
                                winlength = this.ViewLength;
                            }
                            else
                            {
                                if (this.SyntheticMode == SyntheticModes.FixedSymmetric)
                                {
                                    winstart = -(this.SyntheticFixedSymmetricLength >> 1);
                                    winlength = this.SyntheticFixedSymmetricLength;
                                }
                                else
                                    throw new ArgumentOutOfRangeException();
                            }
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
                        else
                        {
                            if (this.InfiniteMode == InfiniteModes.View)
                            {
                                winstart = this.ViewStart;
                                winlength = this.ViewLength;
                            }
                            else
                            {
                                if (this.InfiniteMode == InfiniteModes.FixedSymmetric)
                                {
                                    winstart = -(this.InfiniteFixedSymmetricLength >> 1);
                                    winlength = this.InfiniteFixedSymmetricLength;
                                }
                                else
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }
            }

            var win = new Window(wintype, winstart, winlength, signal.SampleRate, winmode);

            var winsignal = signal.Multiply(win);

            var frequencies = winsignal.Spectrum.Frequencies.Values.ToReadOnlyList();
            var values = this.GetYValues(winsignal.Spectrum).ToReadOnlyList();

            if (this.CustomResulutionEnabled)
            {
                if (this.CustomFrequencies == null)
                    this.CustomFrequencies = new UniformSeries(this.StartFrequency, this.StopFrequency, this.NumberOfPoints, this.Logarithmic);

                var fnew = this.CustomFrequencies.Values.ToReadOnlyList();

                values = Interpolation.AdaptiveInterpolation(frequencies, values, fnew, this.Logarithmic).ToReadOnlyList();
                frequencies = fnew;
            }

            if (this.Smoothing > 0)
                values = Interpolation.MovingAverage(frequencies, values, this.Smoothing).ToReadOnlyList();

            ret.Points.AddRange(values.Zip(frequencies, (m, f) => new DataPoint(f, m)));
            return ret;
        }

        protected abstract IEnumerable<double> GetYValues(ISpectrum spectrum);
    }
}