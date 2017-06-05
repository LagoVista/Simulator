using LagoVista.Core.Models.UIMetaData;
using System;
using System.Linq;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core.Controls.FormControls
{
    public class SelectRow : FormControl
    {
        public event EventHandler<OptionSelectedEventArgs> OptionSelected;

        Label _label;
        Picker _picker;
        Label _validationMessage;
        public SelectRow(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            _label = new Label();
            _label.TextColor = Color.FromRgb(0x5B, 0x5B, 0x5B);
            _label.Text = field.Label;
            _picker = new Picker();

            _validationMessage = new Label();
            _validationMessage.TextColor = Color.Red;
            _validationMessage.Text = field.RequiredMessage;
            _validationMessage.IsVisible = false;

            if (field.Options != null)
            {
                var options = field.Options.Select(opt => opt.Label).ToList();
                if (!String.IsNullOrEmpty(field.Watermark))
                {
                    options.Insert(0, field.Watermark);
                }
                else
                {
                    options.Insert(0, "-select-");
                }

                _picker.ItemsSource = options;
                _picker.SelectedIndex = 0;
            }

            var selectedItem = field.Options.Where(opt => opt.Key == field.Value).FirstOrDefault();
            if(selectedItem != null)
            {
                var index = field.Options.IndexOf(selectedItem);
                _picker.SelectedIndex = index + 1;
            }
            else
            {
                _picker.SelectedIndex = 0;
            }

            Children.Add(_label);
            Children.Add(_picker);
            Children.Add(_validationMessage);

            _picker.SelectedIndexChanged += _picker_SelectedIndexChanged;
            Margin = new Thickness(10, 10, 20, 10);
        }

        private void _picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_picker.SelectedIndex == 0)
            {
                _validationMessage.IsVisible = Field.IsRequired;
                Field.Value = null;
            }
            else
            {
                _validationMessage.IsVisible = false;
                Field.Value = Field.Options[_picker.SelectedIndex - 1].Key;
            }

            OptionSelected?.Invoke(this, new OptionSelectedEventArgs() { Key = Field.Name, Value = Field.Value });
        }

        public override bool Validate()
        {
            if (Field.IsRequired)
            {
                if (String.IsNullOrEmpty(Field.Value))
                {
                    _validationMessage.IsVisible = true;
                    return false;
                }
            }

            return true;
        }
    }

    public class OptionSelectedEventArgs : EventArgs
    {
        public String Key { get; set; }
        public String Value { get; set; }
    }
}
