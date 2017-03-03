// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumCombo.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DspSharpDemo
{
    /// <summary>
    ///     Enhanced ComboBox for displaying quantities in various units.
    /// </summary>
    /// <seealso cref="System.Windows.Controls.ComboBox" />
    public class EnumCombo : ComboBox
    {
        /// <summary>
        ///     The value property.
        /// </summary>
        public static readonly DependencyProperty EnumProperty = DependencyProperty.Register(
            "Enum",
            typeof(Type),
            typeof(EnumCombo),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                EnumPropertyChanged));

        /// <summary>
        ///     Gets the available units.
        /// </summary>
        public Type Enum
        {
            get { return (Type)this.GetValue(EnumProperty); }
            set { this.SetValue(EnumProperty, value); }
        }

        private static void EnumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ec = (EnumCombo)d;
            var value = (Type)e.NewValue;
            if (value == null)
            {
                ec.ItemsSource = Enumerable.Empty<EnumItem>();
                return;
            }

            ec.ItemsSource = System.Enum.GetValues(value).Cast<Enum>().Select(en => new EnumItem(EnumToStringConverter.GetEnumDescription(en), en));
            ec.DisplayMemberPath = "DisplayName";
            ec.SelectedValuePath = "Enum";
        }

        public struct EnumItem
        {
            public EnumItem(string displayName, Enum @enum)
            {
                this.DisplayName = displayName;
                this.Enum = @enum;
            }

            public string DisplayName { get; }
            public Enum Enum { get; }
        }
    }
}