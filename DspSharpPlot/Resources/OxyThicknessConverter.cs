using System;
using OxyPlot;
using UmtUtilities.Converters;

namespace DspSharpPlot.Resources
{
    public class OxyThicknessConverter : ValueConverterNotNull<OxyThickness, string>
    {
        private const char SEPARATOR = ';';

        protected override string Convert(OxyThickness value)
        {
            return $"{value.Left}{SEPARATOR}{value.Top}{SEPARATOR}{value.Right}{SEPARATOR}{value.Bottom}";
        }

        protected override OxyThickness ConvertBack(string value)
        {
            var numbers = value.Split(SEPARATOR);
            if (numbers.Length == 1)
            {
                try
                {
                    var val = System.Convert.ToInt32(numbers[0]);
                    return new OxyThickness(val);
                }
                catch (Exception)
                {
                    return default(OxyThickness);
                }
            }

            if (numbers.Length != 4)
                return default(OxyThickness);

            try
            {
                var left = System.Convert.ToInt32(numbers[0]);
                var top = System.Convert.ToInt32(numbers[1]);
                var right = System.Convert.ToInt32(numbers[2]);
                var bottom = System.Convert.ToInt32(numbers[3]);
                return new OxyThickness(left, top, right, bottom);
            }
            catch (Exception)
            {
                return default(OxyThickness);
            }
        }
    }
}