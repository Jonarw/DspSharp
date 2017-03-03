// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumToStringConverter.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DspSharp.Algorithms;

namespace DspSharpDemo
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
                throw new ArgumentException("Can only convert to string.", nameof(destinationType));

            if (!value.GetType().IsEnum)
                throw new ArgumentException("Can only convert an instance of Enum.", nameof(value));

            return GetEnumDescription((Enum)value);
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            var attributes = fi.GetCustomAttributes(false);
            var d1 = attributes.OfType<DescriptionAttribute>().ToReadOnlyList();

            if (d1.Count > 0)
                return d1[0].Description;

            var d2 = attributes.OfType<PropertyTools.DataAnnotations.DescriptionAttribute>().ToReadOnlyList();

            if (d2.Count > 0)
                return d2[0].Description;

            return value.ToString();
        }
    }
}