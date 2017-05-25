using LagoVista.Core.Models.UIMetaData;
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
            _editor.Text = field.Value;
            _editor.HeightRequest = 120;
            _editor.TextChanged += _editor_TextChanged;
            _label.Text = field.Label;

            Children.Add(_label);
            Children.Add(_editor);
        }

        private void _editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            Field.Value = e.NewTextValue;
        }

        public override bool Validate()
        {
            return true;
        }
    }
}
