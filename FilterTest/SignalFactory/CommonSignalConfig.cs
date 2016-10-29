using Filter;
using PropertyTools.DataAnnotations;

namespace FilterTest.SignalFactory
{
    public class CommonSignalConfig : Observable
    {
        private AvailableSignals _SignalType;

        [DisplayName("signal type")]
        [Category("basic settings")]
        [SortIndex(0)]
        public AvailableSignals SignalType
        {
            get { return this._SignalType; }
            set { this.SetField(ref this._SignalType, value); }
        }
    }
}