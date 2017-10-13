using LagoVista.Client.Core.ViewModels;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.Core;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using LagoVista.Core.Attributes;

namespace LagoVista.DeviceManager.Core.ViewModels
{
    public class ProvisionDeviceViewModel : FormViewModelBase<Device>
    {
        public override Task<InvokeResult> SaveRecordAsync()
        {
            return PerformNetworkOperation(() =>
            {
                return FormRestClient.AddAsync($"/api/device/{LaunchArgs.ParentId}", this.Model);
            });
        }

        public async override void EHPickerTapped(string fieldName)
        {
            if (fieldName == nameof(Model.DeviceType))
            {
                await ViewModelNavigation.NavigateAndPickAsync<DeviceTypePickerViewModel>(this, DeviceTypePicked);
            }
        }

        public void DeviceTypePicked(object obj)
        {
            if (!(obj is DeviceTypeSummary))
            {
                throw new Exception("Must return DeviceTypeSummary from picker.");
            }

            var deviceTypeSummary = obj as DeviceTypeSummary;

            Model.DeviceType = new LagoVista.Core.Models.EntityHeader() { Id = deviceTypeSummary.Id, Text = deviceTypeSummary.Name };
            Model.DeviceConfiguration = new LagoVista.Core.Models.EntityHeader() { Id = deviceTypeSummary.DefaultDeviceConfigId, Text = deviceTypeSummary.DefaultDeviceConfigName };
            View[nameof(Model.DeviceType).ToFieldKey()].Display = Model.DeviceType.Text;
            View[nameof(Model.DeviceType).ToFieldKey()].Value = Model.DeviceType.Id;

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
                        Debug.WriteLine(field.Name);
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
            var rnd = new Random();

            var keyOne = new byte[32];
            var keyTwo = new byte[32];
            rnd.NextBytes(keyOne);
            rnd.NextBytes(keyTwo);

            Model.PrimaryAccessKey = Convert.ToBase64String(keyOne);
            Model.SecondaryAccessKey = Convert.ToBase64String(keyTwo);

            form.AddViewCell(nameof(Model.DeviceId));
            form.AddViewCell(nameof(Model.SerialNumber));
            form.AddViewCell(nameof(Model.DeviceType));
            form.AddViewCell(nameof(Model.PrimaryAccessKey));
            form.AddViewCell(nameof(Model.SecondaryAccessKey));
        }

        protected override string GetRequestUri()
        {
            return $"/api/device/{LaunchArgs.ParentId}/factory";
        }
    }
}
