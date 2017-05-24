using LagoVista.Core.Models.UIMetaData;
using Xamarin.Forms;

namespace LagoVista.Simulator.Controls.FormControls
{
    public class CheckBoxRow : FormControl
    {
        Label _label;
        Switch _switch;

        public CheckBoxRow(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            _label = new Label();
            _label.Text = field.Label;
            _switch = new Switch();

            Children.Add(_label);
            Children.Add(_switch);
        }
    }
}
