using LagoVista.Core.Models.UIMetaData;
using Xamarin.Forms;

namespace LagoVista.Simulator.Controls.FormControls
{
    public class EntityHeaderPicker : FormControl
    {
        Label _label;

        public EntityHeaderPicker(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            _label = new Label();
            _label.Text = field.Label;

            Children.Add(_label);
            Margin = new Thickness(10, 10, 20, 10);
        }


        public override bool Validate()
        {
            return true;
        }
    }
}
