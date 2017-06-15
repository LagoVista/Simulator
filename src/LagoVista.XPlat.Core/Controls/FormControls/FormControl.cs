using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using LagoVista.Core.Models.UIMetaData;
using System;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core.Controls.FormControls
{
    public abstract class FormControl : StackLayout
    {
        FormField _field;
        FormViewer _viewer;

        public FormControl(FormViewer viewer, FormField field)
        {
            _field = field;
            _viewer = viewer;
            IsVisible = field.IsVisible;
        }

        public FormField Field { get { return _field; } }
        protected FormViewer Viewer { get { return _viewer; } }

        public abstract bool Validate();

        public virtual void Refresh()
        {

        }

        public IAppStyle AppStyle { get { return SLWIOC.Get<IAppStyle>(); } }

        public FieldTypes FieldType
        {
            get
            {
                if (Enum.TryParse<FieldTypes>(Field.FieldType, out FieldTypes fieldType))
                {
                    return fieldType;
                }
                else
                {
                    throw new Exception("Could not create field type from field type: " + Field.FieldType);
                }
            }
        }
    }
}