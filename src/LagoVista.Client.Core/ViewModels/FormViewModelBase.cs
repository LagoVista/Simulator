﻿using LagoVista.Client.Core.Net;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using LagoVista.Core.Models;
using LagoVista.Core.ViewModels;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.ViewModels
{
    public class FormViewModelBase<TModel> : IoTAppViewModelBase where TModel : new()
    {
        IFormRestClient<TModel> _restClient;

        public FormViewModelBase()
        {
            _restClient = new FormRestClient<TModel>(base.RestClient);
        }


        public IFormRestClient<TModel> RestClient { get { return _restClient; } }


        TModel _model;
        public TModel Model
        {
            get { return _model; }
            set { Set(ref _model, value); }
        }

        IDictionary<string, FormField> _view;
        public IDictionary<string, FormField> View
        {
            get { return _view; }
            set { Set(ref _view, value); }
        }

        public bool ViewToModel(EditFormAdapter form, TModel model)
        {
            var modelProperties = typeof(TModel).GetTypeInfo().DeclaredProperties;

            if (!form.Validate())
            {
                return false;
            }

            foreach (var formItem in FormAdapter.FormItems)
            {
                var prop = modelProperties.Where(prp => prp.Name.ToLower() == formItem.Name.ToLower()).FirstOrDefault();
                switch (formItem.FieldType)
                {
                    case "Bool":
                    case FormField.FieldType_CheckBox:
                        if (bool.TryParse(formItem.Value, out bool result))
                        {
                            prop.SetValue(model, result);
                        }
                        break;
                    case FormField.FieldType_Picker:
                        if (String.IsNullOrEmpty(formItem.Value))
                        {
                            prop.SetValue(model, null);
                        }
                        else
                        {
                            if (prop.PropertyType == typeof(String))
                            {
                                prop.SetValue(model, formItem.Options.Where(opt => opt.Key == formItem.Value).First().Key);
                            }
                            else
                            {
                                var eh = Activator.CreateInstance(prop.PropertyType) as EntityHeader;
                                eh.Id = formItem.Value;
                                eh.Text = formItem.Options.Where(opt => opt.Key == formItem.Value).First().Label;
                                prop.SetValue(model, eh);
                            }
                        }
                        break;
                    case FormField.FieldType_Integer:
                        if (!String.IsNullOrEmpty(formItem.Value))
                        {
                            if (int.TryParse(formItem.Value, out int intValue))
                            {
                                prop.SetValue(model, intValue);
                            }
                        }

                        break;
                    case FormField.FieldType_Decimal:
                        if (!String.IsNullOrEmpty(formItem.Value))
                        {
                            if (double.TryParse(formItem.Value, out double intValue))
                            {
                                prop.SetValue(model, intValue);
                            }
                        }

                        break;
                    case "NameSpace":
                    case FormField.FieldType_MultilineText:
                    case FormField.FieldType_Text:
                    case FormField.FieldType_Key:
                        prop.SetValue(model, formItem.Value);
                        break;
                }
            }

            return true;
        }

        public void ModelToView(TModel model, EditFormAdapter form)
        {
            var modelProperties = typeof(TModel).GetTypeInfo().DeclaredProperties;

            foreach (var formItem in form.FormItems)
            {
                var prop = modelProperties.Where(prp => prp.Name.ToLower() == formItem.Name.ToLower()).FirstOrDefault();
                var value = prop.GetValue(model);

                switch (formItem.FieldType)
                {
                    case FormField.FieldType_Picker:
                        if (value != null)
                        {
                            if (value.GetType() == typeof(String))
                            {
                                formItem.Value = value.ToString();
                            }
                            else
                            {
                                var entityHeader = value as EntityHeader;
                                formItem.Value = entityHeader.Id;
                            }
                        }
                        break;
                    case "Bool":
                    case "NameSpace":
                    case FormField.FieldType_CheckBox:
                    case FormField.FieldType_Integer:
                    case FormField.FieldType_Decimal:
                    case FormField.FieldType_MultilineText:
                    case FormField.FieldType_Text:
                    case FormField.FieldType_Key:
                        if (value != null)
                        {
                            formItem.Value = value.ToString();
                        }
                        break;
                }
            }
        }

        public bool IsEdit
        {
            get { return LaunchArgs != null && LaunchArgs.LaunchType == LaunchTypes.Edit; }
        }

        public bool IsCreate
        {
            get { return LaunchArgs != null && LaunchArgs.LaunchType == LaunchTypes.Create; }
        }


        EditFormAdapter _form;
        public EditFormAdapter FormAdapter
        {
            get { return _form; }
            set
            {
                _form = value;
                RaisePropertyChanged();
            }
        }

        public override Task ReloadedAsync()
        {
            if (FormAdapter != null)
            {
                FormAdapter.Refresh();
            }

            return base.ReloadedAsync();
        }
    }
}
