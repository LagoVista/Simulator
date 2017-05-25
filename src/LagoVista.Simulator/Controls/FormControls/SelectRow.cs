using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LagoVista.Simulator.Controls.FormControls
{
    public class SelectRow : FormControl
    {
        Label _label;
        Picker _picker;
        Label _validationMessage;
        public SelectRow(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            _label = new Label();
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

            Children.Add(_label);
            Children.Add(_picker);
            Children.Add(_validationMessage);

            _picker.SelectedIndexChanged += _picker_SelectedIndexChanged;
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
}
