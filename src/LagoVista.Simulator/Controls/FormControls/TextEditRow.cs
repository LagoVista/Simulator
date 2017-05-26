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
        Label _validationMessage;

        public TextEditRow(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            _label = new Label();
            _label.Text = field.Label;
            _label.TextColor = Color.FromRgb(0x5B, 0x5B, 0x5B);

            _editor = new Entry();
            _editor.Text = field.Value;
            _editor.TextChanged += _editor_TextChanged;

            _validationMessage = new Label();
            _validationMessage.TextColor = Color.Red;
            _validationMessage.Text = field.RequiredMessage;
            _validationMessage.IsVisible = false;

            

            Children.Add(_label);
            Children.Add(_editor);
            Children.Add(_validationMessage);
            Margin = new Thickness(10, 10, 20, 10);
        }

        public override bool Validate()
        {
            if(Field.IsRequired && String.IsNullOrEmpty(Field.Value))
            {
                _validationMessage.IsVisible = true;
                return false;
            }

            return true;
        }

        private void _editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.NewTextValue))
            {
                switch (Field.FieldType)
                {
                    case FormViewer.INTEGER:
                        if (!int.TryParse(e.NewTextValue, out int value))
                        {
                            _editor.Text = e.OldTextValue;
                        }
                        break;
                }
            }

            Field.Value = e.NewTextValue;
            if(Field.IsRequired)
            {
                _validationMessage.IsVisible = String.IsNullOrEmpty(Field.Value);
            }
        }
    }
}
