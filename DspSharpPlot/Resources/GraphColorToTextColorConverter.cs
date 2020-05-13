using System.Windows.Media;
using OxyPlot;
using UmtUtilities.Converters;

namespace DspSharpPlot.Resources
{
    public class GraphColorToTextColorConverter : OneWayValueConverter<OxyColor, Brush>
    {
        protected override Brush Convert(OxyColor value)
        {
            return new SolidColorBrush(Color.FromArgb(255, value.R, value.G, value.B));
        }
    }
}