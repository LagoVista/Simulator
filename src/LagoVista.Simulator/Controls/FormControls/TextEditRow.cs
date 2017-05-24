using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LagoVista.Simulator.Controls.FormControls
{
    public class TextEditRow : FormControl
    {
        Entry _editor;
        Label _label;        

        public TextEditRow(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            _label = new Label();
            _editor = new Entry();
            _editor.TextChanged += _editor_TextChanged;

            _label.Text = field.Label;

            Children.Add(_label);
            Children.Add(_editor);
        }

        private void _editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            Field.Value = e.NewTextValue;
        }
    }
}
