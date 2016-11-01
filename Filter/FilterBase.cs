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

        protected FilterBase(double samplerate)
        {
            this.Samplerate = samplerate;
        }

        /// <summary>
        ///     Gets or sets the samplerate of the <see cref="FilterBase" /> object.
        /// </summary>
        public double Samplerate { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has infinite impulse response.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has an infinite impulse response; otherwise, <c>false</c>.
        /// </value>
        public bool HasInfiniteImpulseResponse { get; } = false;

        /// <summary>
        ///     Indicates whether the <see cref="FilterBase" /> object has an effect.
        /// </summary>
        public bool HasEffect
        {
            get { return this.Enabled && this.HasEffectOverride; }
        }

        /// <summary>
        ///     Specifies whether the filter object has an effect or not.
        /// </summary>
        protected abstract bool HasEffectOverride { get; }

        /// <summary>
        ///     Determines whether the filter object is enabled. 
        /// </summary>
        public bool Enabled
        {
            get { return this._Enabled; }
            set { this.SetField(ref this._Enabled, value); }
        }

        /// <summary>
        ///     Invoked when the filter object has changed.
        /// </summary>
        public event ChangeEventHandler Changed;

        /// <summary>
        /// Processes the specified signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>The processed signal.</returns>
        public abstract IEnumerable<double> Process(IEnumerable<double> signal);

        /// <summary>
        ///     Should be called every time the filter object is changed in a way that alters its filter effect.
        /// </summary>
        protected void OnChange()
        {
            this.OnChangeOverride();
            this.Changed?.Invoke(this, new FilterChangedEventArgs());
        }

        /// <summary>
        ///     Can be overridden by a child class to perform a certain action every time the filter is changed.
        /// </summary>
        protected virtual void OnChangeOverride()
        {
        }
    }
}