using LagoVista.Core.Models.UIMetaData;
using System;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core.Controls.FormControls
{
    public class EntityHeaderPicker : FormControl
    {
        public event EventHandler<string> PickerTapped;

        Label _label;
        Label _linkLabel;

        public EntityHeaderPicker(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            _label = new Label();
            _label.FontAttributes = FontAttributes.Bold;
            _label.Text = field.Label;

            _linkLabel = new Label();
            _linkLabel.TextColor = Color.Blue;
            
            Children.Add(_label);
            Margin = new Thickness(10, 10, 20, 10);

            Children.Add(_label);
            Children.Add(_linkLabel);

            var tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Tapped += TapRecognizer_Tapped;
            _linkLabel.GestureRecognizers.Add(tapRecognizer);

            Refresh();
        }

        public override void Refresh()
        {
            if (string.IsNullOrEmpty(Field.Value))
            {
                _linkLabel.Text = Field.Watermark;
            }
            else
            {
                _linkLabel.Text = Field.Display;
            }
        }

        private void TapRecognizer_Tapped(object sender, System.EventArgs e)
        {
            PickerTapped?.Invoke(this, Field.Name.ToPropertyName());
        }

        public override bool Validate()
        {
            return !(String.IsNullOrEmpty(Field.Value)) || !Field.IsRequired;          
        }
    }
}
