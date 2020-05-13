// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignalPlot.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using DspSharp.Signal;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using UTilities.Observable;
using Axis = OxyPlot.Axes.Axis;
using PdfExporter = OxyPlot.Pdf.PdfExporter;
using Series = OxyPlot.Series.Series;
using SvgExporter = OxyPlot.Wpf.SvgExporter;

namespace DspSharpPlot
{
    public abstract class SignalPlot : Observable
    {
        private double _Height = 275;
        private string _OutputPath;
        private double _Width = 450;
        private double _XMax;
        private double _XMin;
        private double _YMax;
        private double _YMin;

        protected SignalPlot()
        {
            this.Model.LegendBackground = OxyColor.FromArgb(150, 255, 255, 255);
            this.Model.LegendBorder = OxyColor.FromArgb(128, 0, 0, 0);
            this.Model.LegendOrientation = LegendOrientation.Horizontal;
            this.Model.LegendPosition = LegendPosition.TopLeft;
            this.Model.IsLegendVisible = true;

            this.XAxis.Position = AxisPosition.Bottom;
            this.YAxis.Position = AxisPosition.Left;

            this.Model.Axes.Add(this.XAxis);
            this.Model.Axes.Add(this.YAxis);

            this.Model.PlotMargins = new OxyThickness(double.NaN, double.NaN, 10, double.NaN);

            this.Model.DefaultFont = "CMU Sans Serif";
            this.Model.DefaultFontSize = 11;
            this.PropertyChanged += this.ConfigChanged;
        }

        public string DisplayName { get; set; }

        public double Height
        {
            get => this._Height;
            set => this.SetField(ref this._Height, value);
        }

        public PlotModel Model { get; } = new PlotModel();

        public string OutputPath
        {
            get => this._OutputPath;
            set => this.SetField(ref this._OutputPath, value);
        }

        public IList<ISignal> Signals { get; } = new List<ISignal>();

        public double Width
        {
            get => this._Width;
            set => this.SetField(ref this._Width, value);
        }

        public abstract Axis XAxis { get; }

        public double XMax
        {
            get => this._XMax;
            set
            {
                this.SetField(ref this._XMax, value);
                this.XAxis.Maximum = this.XMax;
            }
        }

        public double XMin
        {
            get => this._XMin;
            set
            {
                this.SetField(ref this._XMin, value);
                this.XAxis.Minimum = this.XMin;
                this.Update(true);
            }
        }

        public abstract Axis YAxis { get; }

        public double YMax
        {
            get => this._YMax;
            set
            {
                this.SetField(ref this._YMax, value);
                this.YAxis.Maximum = this.YMax;
            }
        }

        public double YMin
        {
            get => this._YMin;
            set
            {
                this.SetField(ref this._YMin, value);
                this.YAxis.Minimum = this.YMin;
            }
        }

        public void Update(bool updateData)
        {
            if (!updateData)
            {
                this.Model.InvalidatePlot(false);
                return;
            }

            this.Model.Series.Clear();

            foreach (var signal in this.Signals)
                this.Model.Series.Add(this.CreateGraph(signal));

            this.Model.InvalidatePlot(true);
        }

        protected abstract Series CreateGraph(ISignal signal);

        private void ConfigChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Update(true);
        }

        private void SavePlot()
        {
            Stream stream = new FileStream(this.OutputPath, FileMode.Create, FileAccess.Write, FileShare.None);
            IExporter exporter = null;

            if (this.OutputPath.EndsWith("svg", true, CultureInfo.CurrentCulture))
            {
                exporter = new SvgExporter
                {
                    Width = this.Width,
                    Height = this.Height
                };
            }
            else if (this.OutputPath.EndsWith("pdf", true, CultureInfo.CurrentCulture))
            {
                exporter = new PdfExporter
                {
                    Width = this.Width,
                    Height = this.Height
                };
            }
            else if (this.OutputPath.EndsWith("png", true, CultureInfo.CurrentCulture))
            {
                exporter = new PngExporter
                {
                    Width = (int)this.Width,
                    Height = (int)this.Height
                };
            }

            exporter?.Export(this.Model, stream);
        }
    }
}