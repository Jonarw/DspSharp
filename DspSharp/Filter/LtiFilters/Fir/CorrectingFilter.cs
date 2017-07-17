// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CorrectingFilter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DspSharp.Signal;
using DspSharp.Signal.Windows;
using DspSharp.Utilities.Collections;

#pragma warning disable 1591
// work in progress!

namespace DspSharp.Filter.LtiFilters.Fir
{
    public class CorrectingFilter : Convolver, ISignalBasedFilter
    {
        public enum PhaseTypes
        {
            LinearFilter,
            LinearResult,
            MinimumFilter
        }

        public enum UpdateModes
        {
            Automatic,
            Manual
        }

        private int _FilterLength;

        private double _MaxBoost = 6;

        private double _MinBoost = -20;

        private double _NegativeRatio = 0.5;

        private double _NegativeThreshold = -10;
        private FilterBase _Original;

        private FilterBase _originalLocal;

        private int _Oversampling;

        private PhaseTypes _PhaseType = PhaseTypes.LinearResult;

        private double _PositiveRatio = 0.5;

        private double _PositiveThreshold = 3;
        private FilterBase _Target;
        private FilterBase _targetLocal;

        private UpdateModes _UpdateMode;

        private Window _window;
        private WindowTypes _WindowType = WindowTypes.Hann;

        public CorrectingFilter(double samplerate) : base(samplerate)
        {
            this.Name = "correcting filter";
        }

        public int FilterLength
        {
            get { return this._FilterLength; }
            set { this.SetField(ref this._FilterLength, value); }
        }

        public override IReadOnlyList<double> ImpulseResponse { get; }

        public double MaxBoost
        {
            get { return this._MaxBoost; }
            set { this.SetField(ref this._MaxBoost, value); }
        }

        public double MinBoost
        {
            get { return this._MinBoost; }
            set { this.SetField(ref this._MinBoost, value); }
        }

        public double NegativeRatio
        {
            get { return this._NegativeRatio; }
            set { this.SetField(ref this._NegativeRatio, value); }
        }

        public double NegativeThreshold
        {
            get { return this._NegativeThreshold; }
            set { this.SetField(ref this._NegativeThreshold, value); }
        }

        public FilterBase Original
        {
            get { return this._Original; }
            set
            {
                if (!ReferenceEquals(value, this._Original))
                {
                    if (this._Original != null)
                        this._Original.Changed -= this.LocalChanged;
                    this.SetField(ref this._Original, value);
                    this._Original.Changed += this.LocalChanged;
                    if (this._UpdateMode == UpdateModes.Automatic)
                        this._originalLocal = this._Original;
                }
            }
        }

        public int Oversampling
        {
            get { return this._Oversampling; }
            set { this.SetField(ref this._Oversampling, value); }
        }

        public PhaseTypes PhaseType
        {
            get { return this._PhaseType; }
            set { this.SetField(ref this._PhaseType, value); }
        }

        public double PositiveRatio
        {
            get { return this._PositiveRatio; }
            set { this.SetField(ref this._PositiveRatio, value); }
        }

        public double PositiveThreshold
        {
            get { return this._PositiveThreshold; }
            set { this.SetField(ref this._PositiveThreshold, value); }
        }

        public FilterBase Target
        {
            get { return this._Target; }
            set
            {
                if (!ReferenceEquals(value, this._Target))
                {
                    if (this._Target != null)
                        this._Target.Changed -= this.LocalChanged;
                    this.SetField(ref this._Target, value);
                    this._Target.Changed += this.LocalChanged;
                    if (this._UpdateMode == UpdateModes.Automatic)
                        this._targetLocal = this._Target;
                }
            }
        }

        public UpdateModes UpdateMode
        {
            get { return this._UpdateMode; }
            set { this.SetField(ref this._UpdateMode, value); }
        }

        public WindowTypes WindowType
        {
            get { return this._WindowType; }
            set
            {
                if (this.SetField(ref this._WindowType, value))
                    this._window = null;
            }
        }

        protected override bool HasEffectOverride
        {
            get
            {
                if ((this._targetLocal == null) || (this._originalLocal == null))
                    return false;
                return true;
            }
        }

        public IReadOnlyObservableList<ISignal> AvailableSignals { get; set; }

        public void UpdateFilter()
        {
            if (this._UpdateMode == UpdateModes.Manual)
            {
                //this._originalLocal = this._Original.FixedCopy();
                //this._targetLocal = this._Target.FixedCopy();
            }
        }

        protected IEnumerable<double> GetImpulseResponse()
        {
            throw new NotImplementedException();
            //FftSeries freq = new FftSeries(this.Samplerate, this.FilterLength * this.Oversampling);
            ////FrequencyResponse.FrequencyResponseBase spec = this._targetLocal.GetFrequencyResponse(freq) / this._originalLocal.GetFrequencyResponse(freq);

            //IList<double> specdb = Dsp.LinearToDb(spec.GetMagnitudeResponse()).ToList();

            //for (int c = 0; c <= specdb.Count; c++)
            //{
            //    if (specdb[c] > this.PositiveThreshold)
            //    {
            //        specdb[c] = Math.Min(this.MaxBoost, this.PositiveThreshold + (specdb[c] - this.PositiveThreshold) * this.PositiveRatio);
            //    }
            //    else if (specdb[c] < this.NegativeThreshold)
            //    {
            //        specdb[c] = Math.Max(this.MinBoost, this.NegativeThreshold + (specdb[c] - this.NegativeThreshold) * this.NegativeRatio);
            //    }
            //}

            //FrequencyResponse.FrequencyResponseBase ret;
            //if (this.PhaseType == PhaseTypes.LinearResult)
            //{
            //    ret = new FrequencyResponse.FrequencyResponseBase(spec.Frequencies, Dsp.DbToLinear(specdb), spec.GetPhaseResponse(), 0);
            //}
            //else if (this.PhaseType == PhaseTypes.LinearFilter)
            //{
            //    ret = new FrequencyResponse.FrequencyResponseBase(spec.Frequencies, Dsp.DbToLinear(specdb), 0);
            //}
            //else if (this.PhaseType == PhaseTypes.MinimumFilter)
            //{
            //    throw new NotImplementedException();
            //    //ret = (new FrequencyResponse.FrequencyResponse(spec.Frequencies, Algorithms.Acoustic.DbToLinear(specdb), 0)).MinimumPhaseSpectrum();
            //}
            //else
            //{
            //    ret = new Constant(spec.Frequencies, Complex.One);
            //}

            //if (this._window == null)
            //{
            //    this._window = new Window(this.WindowType, this.FilterLength, this.Samplerate);
            //}

            //TimeDomainSignal imp = ret.GetImpulseResponse(this.Samplerate);
            //imp = imp.ShiftCircularToMaximum();

            //return imp * this._window;
        }

        private void LocalChanged(IFilter sender, FilterChangedEventArgs filterChangedEventArgs)
        {
            if (this._UpdateMode == UpdateModes.Automatic)
                this.RaiseChangedEvent();
        }
    }
}