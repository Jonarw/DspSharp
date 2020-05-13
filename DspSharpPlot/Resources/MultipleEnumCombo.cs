using UmtUtilities.Controls;

namespace DspSharpPlot.Resources
{
    public class MultipleEnumCombo : EnumCombo
    {
        public MultipleEnumCombo()
        {
            this.IsEditable = true;
            this.IsReadOnly = true;
            this.Focusable = false;
        }

        protected override void OnInvalidItem()
        {
            this.Text = "[multiple]";
        }
    }
}