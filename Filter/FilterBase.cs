using System;
using System.Collections.Generic;
using Filter.Signal;

namespace Filter
{
    /// <summary>
    ///     Base class for all types of filters.
    /// </summary>
    public abstract class FilterBase : Observable, IFilter
    {
        internal const double DefaultSamplerate = 44100;

        private bool _Enabled = true;
        private double _Samplerate = DefaultSamplerate;

        /// <summary>
        ///     Gets or sets the samplerate of the <see cref="FilterBase" /> object.
        /// </summary>
        public double Samplerate
        {
            get { return this._Samplerate; }
            set
            {
                this.SetField(ref this._Samplerate, value); 
                this.OnSamplerateChanged();
            }
        }

        public bool HasInfiniteImpulseResponse { get; } = false;

        protected virtual void OnSamplerateChanged()
        {
        }

        /// <summary>
        ///     Indicates whether the <see cref="FilterBase" /> object has an effect. If false, its impulse response is an ideal
        ///     dirac pulse.
        /// </summary>
        public bool HasEffect
        {
            get { return this.Enabled && this.HasEffectOverride; }
        }

        /// <summary>
        ///     Specifies whether the <see cref="FilterBase" /> object has an effect or not. If the derived class returns
        ///     <c>false</c>, the base class functions will not obtain the impulse or
        ///     frequency response of the child class.
        /// </summary>
        protected abstract bool HasEffectOverride { get; }

        /// <summary>
        ///     Determines whether the <see cref="FilterBase" /> object is enabled. 
        /// </summary>
        public bool Enabled
        {
            get { return this._Enabled; }
            set { this.SetField(ref this._Enabled, value); }
        }

        /// <summary>
        ///     Invoked when the <see cref="FilterBase" /> object has changed.
        /// </summary>
        public event ChangeEventHandler Changed;

        public abstract IEnumerable<double> Process(IEnumerable<double> signal);

        /// <summary>
        ///     Should be called every time the <see cref="FilterBase" /> object is changed in a way that influences its filter
        ///     effect.
        /// </summary>
        protected void OnChange()
        {
            this.OnChangeOverride();
            this.Changed?.Invoke(this);
        }

        /// <summary>
        ///     Can be overridden by a child class to perform a certain action every time the <see cref="FilterBase" /> is changed.
        /// </summary>
        protected virtual void OnChangeOverride()
        {
        }
    }
}