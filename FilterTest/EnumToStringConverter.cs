using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace FilterTest
{
    public class EnumToStringConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(Enum);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(destinationType == typeof(string)))
            {
                throw new ArgumentException("Can only convert to string.", nameof(destinationType));
            }

            if (!value.GetType().IsEnum)
            {
                throw new ArgumentException("Can only convert an instance of Enum.", nameof(value));
            }

            return GetEnumDescription((Enum)value);
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);



            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            return value.ToString();
        }
    }
}