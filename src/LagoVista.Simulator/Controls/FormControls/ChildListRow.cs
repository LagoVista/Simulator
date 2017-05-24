using LagoVista.Core.Models.UIMetaData;
using System.Linq;
using Xamarin.Forms;

namespace LagoVista.Simulator.Controls.FormControls
{
    public class ChildListRow : FormControl
    {
        Label _label;

        public ChildListRow(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            _label = new Label();
            _label.Text = field.Label;

            Children.Add(_label);
            Children.Add(_label);
        }
    }
}
