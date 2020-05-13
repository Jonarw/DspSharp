using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AutoBiquad
{
    public static class CsvImporter
    {
        public static (List<double> x, List<double> y) ImportXyData(string fileName, char separator = ',', char decimalPoint = '.')
        {
            var fi = new FileInfo(fileName);
            var x = new List<double>();
            var y = new List<double>();
            if (!fi.Exists)
                return (x, y);

            var file = File.ReadLines(fileName);

            foreach (var line in file)
            {
                var fields = line.Split(separator);

                if (fields.Length < 2)
                    continue;

                if (!TryParseNumber(fields[0], out var xd, decimalPoint))
                    continue;

                if (!TryParseNumber(fields[1], out var yd, decimalPoint))
                    continue;

                x.Add(xd);
                y.Add(yd);
            }

            return (x, y);
        }

        public static bool TryParseNumber(string text, out double result, char decimalPoint = '.')
        {
            return double.TryParse(
                text.Replace(decimalPoint, '.').Replace(" ", string.Empty),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out result);
        }
    }
}