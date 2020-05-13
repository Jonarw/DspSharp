using System;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Wpf;
using UmtUtilities;
using UmtUtilities.DialogProvider;
using UmtUtilities.ViewModel;
using PdfExporter = OxyPlot.Pdf.PdfExporter;
using SvgExporter = OxyPlot.Wpf.SvgExporter;

namespace DspSharpPlot
{
    public abstract class PlotViewModelBase : ViewModelBase
    {
        private IExporter _Exporter;
        private ExporterType _ExporterType;

        protected PlotViewModelBase(IDialogProvider dialogProvider) : base(dialogProvider)
        {
            this.OxyModel = new PlotModel
            {
                LegendBackground = OxyColor.FromArgb(150, 255, 255, 255),
                LegendBorder = OxyColor.FromArgb(128, 0, 0, 0),
                LegendOrientation = LegendOrientation.Horizontal,
                LegendPosition = LegendPosition.TopLeft,
                IsLegendVisible = false,
                DefaultFontSize = 11,
                DefaultFont = "Nunito Sans",
            };

            this.UpdateExporter();
            this.ExportCommand = new RelayCommand(this.Export);
        }

        public ICommand ExportCommand { get; }

        public IExporter Exporter
        {
            get => this._Exporter;
            private set => this.SetField(ref this._Exporter, value);
        }

        public ExporterType ExporterType
        {
            get => this._ExporterType;
            set
            {
                this.SetField(ref this._ExporterType, value);
                this.UpdateExporter();
            }
        }

        public PlotModel OxyModel { get; }

        private void Export()
        {
            var dialog = new SaveFileDialog
                {Filter = $"{this.ExporterType.ToString().ToUpper()} files|*.{this.ExporterType.ToString().ToLower()}", DefaultExt = this.ExporterType.ToString().ToLower()};
            if (dialog.ShowDialog() == true)
                this.Exporter.Export(this.OxyModel, new FileStream(dialog.FileName, FileMode.Create));
        }

        private void UpdateExporter()
        {
            switch (this.ExporterType)
            {
            case ExporterType.Png:
                this.Exporter = new PngExporter {Height = 750, Width = 1000};
                break;
            case ExporterType.Pdf:
                this.Exporter = new PdfExporter {Height = 750, Width = 1000};
                break;
            case ExporterType.Svg:
                this.Exporter = new SvgExporter {Height = 750, Width = 1000};
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}