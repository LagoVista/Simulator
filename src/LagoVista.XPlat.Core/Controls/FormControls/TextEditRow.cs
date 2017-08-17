using LagoVista.Core.Attributes;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core.Controls.FormControls
{
    public class TextEditRow : FormControl
    {
        FormFieldHeader _header;
        FormFieldValidationMessage _validationMessage;
        Entry _editor;

        public TextEditRow(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            _header = new FormFieldHeader(field.Label);

            _editor = new Entry()
            {
                Text = field.Value,
                IsEnabled = field.IsUserEditable
            };
            _editor.TextChanged += _editor_TextChanged;

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

                _validationMessage = new FormFieldValidationMessage(field.RequiredMessage);

                Children.Add(_header);
                Children.Add(_editor);
                Children.Add(_validationMessage);
                Margin = new Thickness(10, 10, 20, 10);
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