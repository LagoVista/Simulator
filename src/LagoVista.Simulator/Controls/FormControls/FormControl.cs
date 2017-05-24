using LagoVista.Core.Models.UIMetaData;
using Xamarin.Forms;

namespace LagoVista.Simulator.Controls.FormControls
{
    public abstract class FormControl : StackLayout
    {
        FormField _field;
        FormViewer _viewer;
        public FormControl(FormViewer viewer, FormField field) 
        {
            _field = field;
            _viewer = viewer;
        }
    }
}