using System.ComponentModel;
using System.Windows;
using OxyPlot;
using UmtUtilities.DialogProvider;

namespace DspSharpPlot
{
    public partial class PlotWindow : Window
    {
        public PlotWindow(bool fastMode = false)
        {
            this.Plotter = new PlotViewModel(new DialogProvider(this));
            if (fastMode)
            {
                this.FastPlotModel = this.Plotter.OxyModel;
                this.NormalPlotModel = null;
            }
            else
            {
                this.NormalPlotModel = this.Plotter.OxyModel;
                this.FastPlotModel = null;
            }

            this.InitializeComponent();
        }

        public PlotViewModel Plotter { get; }
        public PlotModel NormalPlotModel { get; }
        public PlotModel FastPlotModel { get; }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Dispatcher.InvokeShutdown();
            base.OnClosing(e);
        }
    }
}