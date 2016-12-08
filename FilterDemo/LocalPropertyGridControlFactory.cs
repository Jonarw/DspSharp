using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using PropertyTools.Wpf;

namespace FilterTest
{

    /// <summary>
    ///     Slightly adapted PropertyControlFactory that creates appropriate controls for enums.
    /// </summary>
    public class LocalPropertyGridControlFactory : PropertyGridControlFactory
    {
        public override FrameworkElement CreateControl(PropertyItem pi, PropertyControlFactoryOptions options)
        {
            if ((pi.ItemsSourceDescriptor != null) || (pi.ItemsSource != null))
            {
                return this.CreateComboBoxControl(pi);
            }

            if (pi.Is(typeof(Collection<string>)))
            {
                return this.CreateListControl(pi);
            }

            if (pi.Is(typeof(ICommand)))
            {
                return this.CreateCommandControl(pi);
            }

            return base.CreateControl(pi, options);
        }

        protected virtual FrameworkElement CreateCommandControl(PropertyItem property)
        {
            var c = new Button();
            c.SetBinding(ButtonBase.CommandProperty, property.CreateBinding());
            return c;
        }

        protected override FrameworkElement CreateEnumControl(PropertyItem property, PropertyControlFactoryOptions options)
        {
            var enumType = TypeHelper.GetEnumType(property.Descriptor.PropertyType);

            var c = new EnumCombo { Enum = enumType };
            c.SetBinding(Selector.SelectedValueProperty, property.CreateBinding());
            return c;
        }

        protected virtual FrameworkElement CreateListControl(PropertyItem property)
        {
            var c = new ListBox();
            c.SetBinding(ItemsControl.ItemsSourceProperty, property.CreateBinding());
            return c;
        }
    }
}