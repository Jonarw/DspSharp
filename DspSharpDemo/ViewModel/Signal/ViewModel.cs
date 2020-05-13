// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Windows.Input;
using DspSharp.Signal;
using UmtUtilities;
using UmtUtilities.ViewModel;
using UTilities.Observable.DataAnnotations;

namespace DspSharpDemo.ViewModel.Signal
{
    public class ViewModel : ViewModelBase
    {
        private ICommand _CancelCommand;
        private ISignal _CreatedSignal;
        private bool _DialogResult;
        private ICommand _OkCommand;
        private SignalType _SelectedSignalType;
        private SignalFactories.SignalFactory _SignalFactory;

        public ViewModel(double samplerate)
        {
            this.Samplerate = samplerate;
            this.UpdateSignalFactory();
        }

        /// <summary>
        ///     Gets the close command.
        /// </summary>
        public ICommand CancelCommand => this._CancelCommand ?? (this._CancelCommand = new RelayCommand(param => this.Cancel()));

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
        public ICommand OkCommand => this._OkCommand ?? (this._OkCommand = new RelayCommand(param => this.Ok(), o => this.SignalFactory != null && !this.SignalFactory.HasErrors));

        public double Samplerate { get; }

        public SignalType SelectedSignalType
        {
            get { return this._SelectedSignalType; }
            set { this.SetField(ref this._SelectedSignalType, value); }
        }

        [Observed]
        public SignalFactories.SignalFactory SignalFactory
        {
            get { return this._SignalFactory; }
            private set { this.SetField(ref this._SignalFactory, value); }
        }

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.SelectedSignalType))
                this.UpdateSignalFactory();

            base.OnPropertyChanged(sender, e);
        }

        private void Cancel()
        {
            this.DialogResult = false;
        }

        private void Ok()
        {
            this.CreatedSignal = this.SignalFactory.CreateItem();
            this.DialogResult = true;
        }

        private void UpdateSignalFactory()
        {
            this.SignalFactory = SignalFactoryFactory.CreateSignalFactory(this.SelectedSignalType, this.Samplerate);
        }
    }
}