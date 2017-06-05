using LagoVista.XPlat.Core.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using LagoVista.XPlat.Core.Controls.FormControls;

namespace LagoVista.XPlat.Core
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

        public void Refresh()
        {
            foreach(var field in _formControls)
            {
                field.Refresh();
            }
        }

        public void SetViewVisibility(string name, bool isVisible)
        {
            var field = _formControls.Where(fld => fld.Field.Name == name.ToFieldKey()).First();
            field.IsVisible = isVisible;
        }

        public EditFormAdapter Form
        {
            get { return (EditFormAdapter)base.GetValue(FormProperty); }
            set {
                base.SetValue(FormProperty, value);
                value.SetValidationMethod(Validate);
                value.SetRefreshMethod(Refresh);
                value.SetVisibilityMethod(SetViewVisibility);
                Populate();
            }
        }

        public static BindableProperty FormProperty = BindableProperty.Create(
                                                            propertyName: nameof(Form),
                                                            returnType: typeof(EditFormAdapter),
                                                            declaringType: typeof(FormViewer),
                                                            defaultValue: null,
                                                            defaultBindingMode: BindingMode.Default,
                                                            propertyChanged: HandleFormFieldsAssigned);

        private static void HandleFormFieldsAssigned(BindableObject bindable, object oldValue, object newValue)
        {
            var button = (FormViewer)bindable;
            button.Form = newValue as EditFormAdapter;
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
                        case FormViewer.MULTILINE: AddChild(new TextAreaRow(this, field)); break;
                        case FormViewer.CHECKBOX: AddChild(new CheckBoxRow(this, field)); break;
                        case FormViewer.ENTITYHEADERPICKER: AddChild(new EntityHeaderPicker(this, field)); break;
                        case FormViewer.PICKER:
                            var picker = new SelectRow(this, field);
                            picker.OptionSelected += Picker_OptionSelected;
                            AddChild(picker);
                            break;
                        case FormViewer.CHILDLIST:
                            var childListControl = new ChildListRow(this, field);
                            if(Form.ChildLists.ContainsKey(field.Name))
                            {
                                childListControl.ChildItems = Form.ChildLists[field.Name];
                            }
                            childListControl.Add += ChildListControl_Add;
                            childListControl.ItemSelected += ChildListControl_ItemSelected;
                            AddChild(childListControl);
                            break;
                        default: AddChild(new TextEditRow(this, field)); break;
                    }
                }
            }
        }        


        private void Picker_OptionSelected(object sender, OptionSelectedEventArgs e)
        {
            Form.InvokeOptionSelected(e);
        }

        private void ChildListControl_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            Form.InvokeItemSelected(e);
        }

        private void ChildListControl_Add(object sender, string e)
        {
            Form.InvokeAdd(e);
        }
    }
}
