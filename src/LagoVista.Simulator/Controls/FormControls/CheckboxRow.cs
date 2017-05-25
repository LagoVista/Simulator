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
            _switch.Toggled += _switch_Toggled;

            if(bool.TryParse(field.Value, out bool _isToggled))
            {
                _switch.IsToggled = _isToggled;
            }

            Children.Add(_label);
            Children.Add(_switch);
        }

        private void _switch_Toggled(object sender, ToggledEventArgs e)
        {
            Field.Value = e.Value ? "true" : "false";
        }

        public override bool Validate()
        {
            return true;
        }
    }
}
