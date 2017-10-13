using LagoVista.Client.Core.ViewModels;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using LagoVista.Core;
using LagoVista.IoT.DeviceAdmin.Models;
using System.Collections.Generic;
using LagoVista.Core.Attributes;
using System.Linq;

namespace LagoVista.DeviceManager.Core.ViewModels
{
    public class ManageDeviceViewModel : FormViewModelBase<Device>
    {
        string _deviceRepoId;
        string _deviceId;

        public ManageDeviceViewModel()
        {
        }

        public override Task<InvokeResult> SaveRecordAsync()
        {
            throw new NotImplementedException();
        }

        private  void ShowProperties()
        {
            PerformNetworkOperation(async () =>
            {
                var result = await RestClient.GetAsync<List<CustomField>>($"/api/deviceconfig/{Model.DeviceConfiguration.Id}/properties");
                var fields = new Dictionary<string, FormField>();
                var adapter = new EditFormAdapter(Model.Properties, fields, ViewModelNavigation);
                if (result.Result != null)
                {
                    foreach (var field in result.Result)
                    {
                        var formField = FormField.Create(field.Key, new FormFieldAttribute());

                        formField.Label = field.Label;

                        switch (field.FieldType.Value)
                        {
                            case ParameterTypes.State:
                                formField.Options = new List<EnumDescription>();
                                foreach (var state in field.StateSet.Value.States)
                                {
                                    formField.Options.Add(new EnumDescription() { Key = state.Key, Label = state.Name, Name = state.Name });
                                }

                                formField.FieldType = FormField.FieldType_Picker;

                                var initialState = field.StateSet.Value.States.Where(st => st.IsInitialState).FirstOrDefault();
                                if (initialState != null)
                                {
                                    formField.Value = initialState.Key;
                                }

                                break;
                            case ParameterTypes.String:
                                formField.FieldType = FormField.FieldType_Text;
                                formField.Value = field.DefaultValue;
                                break;
                            case ParameterTypes.DateTime:
                                formField.FieldType = FormField.FieldType_DateTime;
                                formField.Value = field.DefaultValue;
                                break;
                            case ParameterTypes.Decimal:
                                formField.FieldType = FormField.FieldType_Decimal;
                                formField.Value = field.DefaultValue;
                                break;
                            case ParameterTypes.GeoLocation:
                                formField.FieldType = FormField.FieldType_Text;
                                formField.Value = field.DefaultValue;
                                break;
                            case ParameterTypes.TrueFalse:
                                formField.FieldType = FormField.FieldType_CheckBox;
                                formField.Value = field.DefaultValue;
                                break;
                            case ParameterTypes.ValueWithUnit:
                                formField.FieldType = FormField.FieldType_Decimal;
                                formField.Value = field.DefaultValue;
                                break;
                        }

                        formField.IsRequired = field.IsRequired;
                        formField.IsUserEditable = !field.IsReadOnly;

                        fields.Add(field.Key, formField);
                        adapter.AddViewCell(field.Key);
                    }
                }

                CustomFieldAdapter = adapter;
            });
        }
        
        EditFormAdapter _form;
        public EditFormAdapter CustomFieldAdapter
        {
            get { return _form; }
            set
            {
                _form = value;
                RaisePropertyChanged();
            }
        }

        protected override void BuildForm(EditFormAdapter form)
        {
            View[nameof(Model.DeviceId).ToFieldKey()].IsUserEditable = LaunchArgs.LaunchType == LaunchTypes.Create;

            form.AddViewCell(nameof(Model.DeviceId));
            form.AddViewCell(nameof(Model.SerialNumber));
            form.AddViewCell(nameof(Model.DeviceType));

            ShowProperties();
        }

        protected override string GetRequestUri()
        {
            _deviceRepoId = LaunchArgs.Parameters[MonitorDeviceViewModel.DeviceRepoId].ToString();
            _deviceId = LaunchArgs.Parameters[MonitorDeviceViewModel.DeviceId].ToString();

            return $"/api/device/{_deviceRepoId}/{_deviceId}";
        }        
    }
}
