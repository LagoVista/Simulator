using LagoVista.Core.Attributes;
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

            if (Enum.TryParse<FieldTypes>(field.FieldType, out FieldTypes fieldType))
            {
                switch (FieldType)
                {
                    case FieldTypes.Key:
                        _editor.Keyboard = Keyboard.Plain;
                        break;
                    case FieldTypes.Decimal:
                    case FieldTypes.Integer:
                        _editor.Keyboard = Keyboard.Numeric;
                        _editor.HorizontalTextAlignment = TextAlignment.End;
                        break;
                }

                _validationMessage = new LagoVista.XPlat.Core.Label();
                _validationMessage.TextColor = Color.Red;
                _validationMessage.Text = field.RequiredMessage;
                _validationMessage.IsVisible = false;

                Children.Add(_label);
                Children.Add(_editor);
                Children.Add(_validationMessage);
                Margin = new Thickness(10, 5, 20, 0);
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            _editor.Text = Field.Value;
        }


        public override bool Validate()
        {
            if (Field.IsRequired && String.IsNullOrEmpty(Field.Value))
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
                switch (FieldType)
                {
                    case FieldTypes.Integer:
                        {
                            if (!int.TryParse(e.NewTextValue, out int value))
                            {
                                _editor.Text = e.OldTextValue;
                            }
                        }
                        break;
                    case FieldTypes.Decimal:
                        {
                            if (!double.TryParse(e.NewTextValue, out double value))
                            {
                                _editor.Text = e.OldTextValue;
                            }
                        }
                        break;
                }
            }

            Field.Value = e.NewTextValue;
            IsDirty = OriginalValue != Field.Value;

            if (Field.IsRequired && String.IsNullOrEmpty(Field.Value))
            {
                _validationMessage.IsVisible = true;
                _validationMessage.Text = Field.RequiredMessage;
            }
            else if (!String.IsNullOrEmpty(Field.RegEx) && !String.IsNullOrEmpty(Field.Value))
            {
                var regEx = new Regex(Field.RegEx);
                if (!regEx.Match(Field.Value).Success)
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
