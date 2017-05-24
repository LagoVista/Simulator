using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LagoVista.Simulator.Controls.FormControls
{
    public class TextAreaRow : FormControl
    {
        Editor _editor;
        Label _label;

        public TextAreaRow(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            _label = new Label();
            _editor = new Editor();
            _editor.HeightRequest = 120;
            _label.Text = field.Label;

            Children.Add(_label);
            Children.Add(_editor);
        }
    }
}
