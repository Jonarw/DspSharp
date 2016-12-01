using System.Windows;
using System.Windows.Data;
using Filter.Signal;

namespace FilterTest.SignalFactory
{
    /// <summary>
    ///     Interaction logic for SignalDialog.xaml
    /// </summary>
    public partial class SignalDialog : Window
    {
        public static DependencyProperty CreatedSignalProperty = DependencyProperty.Register(
            "CreatedSignal",
            typeof (ISignal),
            typeof (SignalDialog),
            new PropertyMetadata());

        public SignalDialog(double samplerate)
        {
            this.InitializeComponent();
            this.DataContext = new ViewModel(samplerate);
            var binding = new Binding(nameof(ViewModel.CreatedSignal)) {Source = this.DataContext};
            this.SetBinding(CreatedSignalProperty, binding);

        }

        public ISignal CreatedSignal
        {
            get { return (ISignal)this.GetValue(CreatedSignalProperty); }
            set { this.SetValue(CreatedSignalProperty, value); }
        }
    }
}