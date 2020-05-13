using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using UmtUtilities.Converters;

namespace DspSharpPlot.Resources
{
    public class NullableDoubleConverter : ValueConverter<double?, string>
    {
        protected override string Convert(double? value)
        {
            return value == null ? "[multiple]" : value.ToString();
        }

        protected override double? ConvertBack(string value)
        {
            return double.TryParse(value, out var ret) ? ret : 0;
        }
    }

    public class NullableIntConverter : ValueConverter<int?, string>
    {
        protected override string Convert(int? value)
        {
            return value == null ? "[multiple]" : value.ToString();
        }

        protected override int? ConvertBack(string value)
        {
            return int.TryParse(value, out var ret) ? ret : 0;
        }
    }

    public class NullableColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as Color? ?? Color.FromArgb(0, 0, 0, 100);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as Color?;
        }
    }
}