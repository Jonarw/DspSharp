using System.Collections.Generic;
using PropertyTools.DataAnnotations;

namespace Filter.Signal
{
    /// <summary>
    ///     Base class for all signals.
    /// </summary>
    /// <seealso cref="Filter.Observable" />
    /// <seealso cref="Filter.Signal.ISignal" />
    public abstract class SignalBase : Observable, ISignal
    {
        private string _DisplayName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SignalBase" /> class.
        /// </summary>
        /// <param name="sampleRate">The sample rate.</param>
        protected SignalBase(double sampleRate)
        {
            this.SampleRate = sampleRate;
        }

        /// <summary>
        ///     Gets a section of the signal in time domain.
        /// </summary>
        /// <param name="start">The start of the section.</param>
        /// <param name="length">The length of the section.</param>
        /// <returns>
        ///     The specified section.
        /// </returns>
        public abstract IEnumerable<double> GetWindowedSignal(int start, int length);

        [Category("general")]
        [DisplayName("display name")]
        public string DisplayName
        {
            get { return this._DisplayName; }
            set { this.SetField(ref this._DisplayName, value); }
        }

        [DisplayName("sample rate")]
        public double SampleRate { get; }
    }
}