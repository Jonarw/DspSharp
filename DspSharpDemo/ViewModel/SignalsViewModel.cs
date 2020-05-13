using DspSharp.Signal;
using DspSharpDemo.ViewModel.Signal;
using UmtUtilities.CollectionViewModel;
using UmtUtilities.DialogProvider;
using UmtUtilities.ViewModel;
using UTilities.Collections;
using UTilities.Factory;
using UTilities.Observable.DataAnnotations;

namespace DspSharpDemo.ViewModel
{
    public class SignalsViewModel : CollectionViewModelVm<ISignal>
    {
        public SignalsViewModel(DspSharpDemoViewModel viewModel, IDialogProvider dialogProvider = null) : base(
            new ObservableList<ISignal>(),
            new SignalFactory(viewModel),
            new FuncFactory<IViewModel<ISignal>, ISignal>(signal => new SignaViewModel(signal)),
            dialogProvider)
        {
            this.ViewModel = viewModel;
        }

        [ObservedCollection]
        public IObservableList<ISignal> SelectedSignals { get; } = new ObservableList<ISignal>();

        public SignalFactory SignalFactory => (SignalFactory)this.ItemFactory;

        public DspSharpDemoViewModel ViewModel { get; }
    }

    public class SignaViewModel : ViewModelBase<ISignal>
    {
        public SignaViewModel(ISignal model) : base(model)
        {
        }
    }

    public class SignalFactory : Factory<ISignal>
    {
        private IFactory<ISignal> _InternalFactory;
        private double _SampleRate = 48000;
        private SignalType _SignalType = SignalType.Dirac;

        public SignalFactory(DspSharpDemoViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.UpdateInternalFactory();
        }

        public IFactory<ISignal> InternalFactory
        {
            get { return this._InternalFactory; }
            set { this.SetField(ref this._InternalFactory, value); }
        }

        public double SampleRate
        {
            get { return this._SampleRate; }
            set { this.SetField(ref this._SampleRate, value); }
        }

        public SignalType SignalType
        {
            get { return this._SignalType; }
            set
            {
                this.SetField(ref this._SignalType, value);
                this.UpdateInternalFactory();
            }
        }

        public DspSharpDemoViewModel ViewModel { get; }

        public override ISignal CreateItem()
        {
            return this.InternalFactory.CreateItem();
        }

        private void UpdateInternalFactory()
        {
            this.InternalFactory = SignalFactoryFactory.CreateSignalFactory(this.SignalType, this.SampleRate);
        }
    }
}