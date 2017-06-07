using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core.Controls.FormControls
{
    public class TextEditRow : FormControl
    {
        LagoVista.XPlat.Core.Entry _editor;
        LagoVista.XPlat.Core.Label _label;
        LagoVista.XPlat.Core.Label _validationMessage;

        public TextEditRow(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            _label = new LagoVista.XPlat.Core.Label();
            _label.Text = field.Label;
            _label.TextColor = Color.FromRgb(0x5B, 0x5B, 0x5B);

            _editor = new Entry();
            _editor.Text = field.Value;
            _editor.TextChanged += _editor_TextChanged;
            _editor.IsEnabled = field.IsUserEditable;

            switch(field.DataType)
            {
                case FormField.FieldType_Key:
                    _editor.Keyboard = Keyboard.Create(KeyboardFlags.None);
                    break;
                case FormField.FieldType_Decimal:
                case FormField.FieldType_Integer:
                    _editor.Keyboard = Keyboard.Numeric;
                    break;
            }

            _validationMessage = new LagoVista.XPlat.Core.Label();
            _validationMessage.TextColor = Color.Red;
            _validationMessage.Text = field.RequiredMessage;
            _validationMessage.IsVisible = false;

            Children.Add(_label);
            Children.Add(_editor);
            Children.Add(_validationMessage);
            Margin = new Thickness(10, 5, 0, 10);
        }

        public override bool Validate()
        {
            if(Field.IsRequired && String.IsNullOrEmpty(Field.Value))
            {
                _validationMessage.IsVisible = true;
                _validationMessage.Text = Field.RequiredMessage;
                return false;
            }

            if (!String.IsNullOrEmpty(Field.RegEx) && !String.IsNullOrEmpty(Field.Value))
            {
                var regEx = new Regex(Field.RegEx);
                if (!regEx.Match(Field.Value).Success)
                {
                    _validationMessage.Text = Field.RegExMessage;
                    _validationMessage.IsVisible = true;
                }
            }

            return true;
        }

        private void _editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.NewTextValue))
            {
                switch (Field.FieldType)
                {
                    case FormField.FieldType_Integer:
                        if (!int.TryParse(e.NewTextValue, out int value))
                        {
                            _editor.Text = e.OldTextValue;
                        }
                        break;
                }
            }

            Field.Value = e.NewTextValue;
            if(Field.IsRequired && String.IsNullOrEmpty(Field.Value))
            {
                _validationMessage.IsVisible = true;
                _validationMessage.Text = Field.RequiredMessage;
            }
            else if(!String.IsNullOrEmpty(Field.RegEx) && !String.IsNullOrEmpty(Field.Value))
            {
                var regEx = new Regex(Field.RegEx);
                if(!regEx.Match(Field.Value).Success)
                {
                    _validationMessage.Text = Field.RegExMessage;
                    _validationMessage.IsVisible = true;
                }
                else
                {
                    _validationMessage.IsVisible = false;
                }
            }
            else
            {
                _validationMessage.IsVisible = false;
            }
        }
    }
}
