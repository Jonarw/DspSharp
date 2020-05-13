using OxyPlot;
using UmtUtilities.Converters;

namespace DspSharpPlot.Resources
{
    public class BartekThemeConverter : ValueConverterNotNull<OxyColor, bool>
    {
        protected override bool Convert(OxyColor value)
        {
            return value == OxyColors.Yellow;
        }

        protected override OxyColor ConvertBack(bool value)
        {
            return value ? OxyColors.Yellow : OxyColors.Automatic;
        }
    }
}