﻿using LagoVista.Core.Models.UIMetaData;
using LagoVista.Simulator.Controls.FormControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace LagoVista.Simulator.Controls
{
    public class FormViewer : StackLayout
    {

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
            Margin = new Thickness(10);
        }

        public Object FormFields
        {
            get { return (ObservableCollection<FormField>)base.GetValue(FormFieldsProperty); }
            set {
                base.SetValue(FormFieldsProperty, value);
                Populate();
            }
        }

        public static BindableProperty FormFieldsProperty = BindableProperty.Create(
                                                            propertyName: nameof(FormFields),
                                                            returnType: typeof(ObservableCollection<FormField>),
                                                            declaringType: typeof(FormViewer),
                                                            defaultValue: null,
                                                            defaultBindingMode: BindingMode.Default,
                                                            propertyChanged: HandleFormFieldsAssigned);

        private static void HandleFormFieldsAssigned(BindableObject bindable, object oldValue, object newValue)
        {
            var button = (FormViewer)bindable;
            button.FormFields = newValue as ObservableCollection<FormField>;
        }
        
        private void AddChild(FormControl field)
        {
            _formControls.Add(field);
            Children.Add(field);
        }

        private List<FormControl> _formControls;

        protected void Populate()
        {
            _formControls.Clear();
            Children.Clear();

            if (FormFields != null)
            {
                foreach (var field in FormFields as ObservableCollection<FormField>)
                {
                    switch (field.FieldType)
                    {
                        case FormViewer.MULTILINE: AddChild(new FormControls.TextAreaRow(this, field)); break;
                        case FormViewer.CHECKBOX: AddChild(new FormControls.CheckBoxRow(this, field)); break;
                        case FormViewer.ENTITYHEADERPICKER: AddChild(new FormControls.EntityHeaderPicker(this, field)); break;
                        case FormViewer.PICKER: AddChild(new FormControls.SelectRow(this, field)); break;
                        case FormViewer.CHILDLIST: AddChild(new FormControls.ChildListRow(this, field)); break;
                        default: AddChild(new FormControls.TextEditRow(this, field)); break;
                    }
                }
            }
        }

    }
}