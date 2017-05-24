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

        public SelectRow(FormViewer formViewer, FormField field) : base(formViewer, field)
        {
            _label = new Label();
            _label.Text = field.Label;
            _picker = new Picker();
            if(field.Options != null)
            {
                _picker.ItemsSource = field.Options.Select(opt => opt.Label).ToList();
            }

            Children.Add(_label);
            Children.Add(_picker);
        }
    }
}
