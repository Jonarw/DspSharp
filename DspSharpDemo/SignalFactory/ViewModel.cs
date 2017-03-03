// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Windows.Input;
using DspSharp;
using DspSharp.Signal;

namespace DspSharpDemo.SignalFactory
{
    public class ViewModel : Observable
    {
        private ICommand _CancelCommand;
        private ISignal _CreatedSignal;
        private bool _DialogResult;
        private ICommand _OkCommand;
        private SignalFactory _SpecificConfig;

        public ViewModel(double samplerate)
        {
            this.Samplerate = samplerate;
            this.CommonConfig.PropertyChanged += this.CommonConfigChanged;
            this.SignalTypeChanged();
        }

        /// <summary>
        ///     Gets the close command.
        /// </summary>
        public ICommand CancelCommand => this._CancelCommand ?? (this._CancelCommand = new RelayCommand(param => this.Cancel()));

        public CommonSignalConfig CommonConfig { get; } = new CommonSignalConfig();

        public ISignal CreatedSignal
        {
            get { return this._CreatedSignal; }
            set { this.SetField(ref this._CreatedSignal, value); }
        }

        public bool DialogResult
        {
            get { return this._DialogResult; }
            set { this.SetField(ref this._DialogResult, value); }
        }

        /// <summary>
        ///     Gets the close command.
        /// </summary>
        public ICommand OkCommand => this._OkCommand ?? (this._OkCommand = new RelayCommand(param => this.Ok(), o => this.SpecificConfig != null));

        public double Samplerate { get; }

        public SignalFactory SpecificConfig
        {
            get { return this._SpecificConfig; }
            private set { this.SetField(ref this._SpecificConfig, value); }
        }

        private void Cancel()
        {
            this.DialogResult = false;
        }

        private void CommonConfigChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CommonSignalConfig.SignalType))
                this.SignalTypeChanged();
        }

        private void Ok()
        {
            this.CreatedSignal = this.SpecificConfig.CreateSignal();
            this.DialogResult = true;
        }

        private void SignalTypeChanged()
        {
            this.SpecificConfig = SignalFactoryFactory.CreateSignalFactory(this.CommonConfig.SignalType, this.Samplerate);
        }
    }
}