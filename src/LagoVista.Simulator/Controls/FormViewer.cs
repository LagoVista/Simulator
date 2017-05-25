using LagoVista.Core.Models.UIMetaData;
using LagoVista.Simulator.Controls.FormControls;
using LagoVista.Simulator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace LagoVista.Simulator.Controls
{
    public class FormViewer : ScrollView
    {
        StackLayout _container;

        public const string MULTILINE = "MultiLineText";
        public const string CHECKBOX = "CheckBox";
        public const string PICKER = "Picker";
        public const string ENTITYHEADERPICKER = "EntityHeaderPicker";
        public const string TEXT = "Text";
        public const string KEY = "Key";
        public const string CHILDLIST = "ChildList";
        public const string DECIMAL = "Decimal";
        public const string INTEGER = "Integer";

        public FormViewer()
        {
             _formControls = new List<FormControl>();

            _container = new StackLayout();
            Content = _container;
        }

        public bool Validate()
        {
            var valid = true;
            foreach(var field in _formControls)
            {
                valid &= field.Validate();
            }

            return valid;
        }

        public EditForm Form
        {
            get { return (EditForm)base.GetValue(FormProperty); }
            set {
                base.SetValue(FormProperty, value);
                value.SetValidationMethod(Validate);
                Populate();

            }
        }

        public static BindableProperty FormProperty = BindableProperty.Create(
                                                            propertyName: nameof(Form),
                                                            returnType: typeof(EditForm),
                                                            declaringType: typeof(FormViewer),
                                                            defaultValue: null,
                                                            defaultBindingMode: BindingMode.Default,
                                                            propertyChanged: HandleFormFieldsAssigned);

        private static void HandleFormFieldsAssigned(BindableObject bindable, object oldValue, object newValue)
        {
            var button = (FormViewer)bindable;
            button.Form = newValue as EditForm;
        }
        
        private void AddChild(FormControl field)
        {
            _formControls.Add(field);
            _container.Children.Add(field);
        }

        private List<FormControl> _formControls;

        protected void Populate()
        {
            _formControls.Clear();
            _container.Children.Clear();

            if (Form != null)
            {
                foreach (var field in Form.FormItems)
                {
                    switch (field.FieldType)
                    {
                        case FormViewer.MULTILINE: AddChild(new FormControls.TextAreaRow(this, field)); break;
                        case FormViewer.CHECKBOX: AddChild(new FormControls.CheckBoxRow(this, field)); break;
                        case FormViewer.ENTITYHEADERPICKER: AddChild(new FormControls.EntityHeaderPicker(this, field)); break;
                        case FormViewer.PICKER: AddChild(new FormControls.SelectRow(this, field)); break;
                        case FormViewer.CHILDLIST:
                            var childListControl = new FormControls.ChildListRow(this, field);
                            if(Form.ChildLists.ContainsKey(field.Name))
                            {
                                childListControl.ChildItems = Form.ChildLists[field.Name];
                            }
                            childListControl.Add += ChildListControl_Add;
                            AddChild(childListControl);
                            break;
                        default: AddChild(new FormControls.TextEditRow(this, field)); break;
                    }
                }
            }
        }

        private void ChildListControl_Add(object sender, string e)
        {
            Form.InvokeAdd(e);
        }
    }
}
