// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterBase.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DspSharp.Filter
{
    /// <summary>
    /// Base class for all types of filters.
    /// </summary>
    public abstract class FilterBase : Observable, IFilter
    {
        internal const double DefaultSamplerate = 48000;
        private bool _Enabled = true;
        private string _Name;

        protected FilterBase(double samplerate)
        {
            this.Samplerate = samplerate;
        }

        /// <summary>
        ///  Invoked when the filter object has changed.
        /// </summary>
        public event EventHandler<EventArgs> ResponseChanged;

        public static IReadOnlyList<double> DefaultSampleRates { get; } = new[] { 44100d, 48000, 88200, 96000, 192000 };

        /// <inheritdoc/>
        public string DisplayName
        {
            get => this._Name;
            set => this.SetField(ref this._Name, value);
        }

        /// <inheritdoc/>
        public bool Enabled
        {
            get => this._Enabled;
            set => this.SetField(ref this._Enabled, value);
        }

        /// <inheritdoc/>
        public bool HasEffect => this.Enabled && this.HasEffectOverride;

        /// <inheritdoc/>
        public virtual bool HasInfiniteImpulseResponse => true;

        /// <inheritdoc/>
        public double Samplerate { get; }

        /// <summary>
        /// Specifies whether the filter object has an effect.
        /// </summary>
        protected virtual bool HasEffectOverride => true;

        /// <inheritdoc/>
        public IEnumerable<double> Process(IEnumerable<double> input)
        {
            if (this.HasEffect)
                return this.ProcessOverride(input);

            return input;
        }

        /// <summary>
        /// Can be overridden by a child class to perform a certain action every time the filter is changed.
        /// </summary>
        protected virtual void OnChange()
        {
        }

        /// <summary>
        /// Processes the specified signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>The processed signal.</returns>
        protected abstract IEnumerable<double> ProcessOverride(IEnumerable<double> signal);

        /// <summary>
        /// Should be called every time the filter object is changed in a way that alters its filter effect.
        /// </summary>
        protected void RaiseChangedEvent()
        {
            this.OnChange();
            this.ResponseChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}