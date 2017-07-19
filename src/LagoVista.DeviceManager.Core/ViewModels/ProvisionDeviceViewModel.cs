using LagoVista.Client.Core.ViewModels;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.Core;

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
            if(fieldName == nameof(Model.DeviceType))
            {
                await ViewModelNavigation.NavigateAndPickAsync<DeviceTypePickerViewModel>(this, DeviceTypePicked);
            }
        }

        public void DeviceTypePicked(object obj)
        {
           if(!(obj is DeviceTypeSummary))
            {
                throw new Exception("Must return DeviceTypeSummary from picker.");
            }

            var deviceTypeSummary = obj as DeviceTypeSummary;

            Model.DeviceType = new LagoVista.Core.Models.EntityHeader() { Id = deviceTypeSummary.Id, Text = deviceTypeSummary.Name };
            Model.DeviceConfiguration = new LagoVista.Core.Models.EntityHeader() { Id = deviceTypeSummary.DefaultDeviceConfigId, Text = deviceTypeSummary.DefaultDeviceConfigName };
            View[nameof(Model.DeviceType).ToFieldKey()].Display = Model.DeviceType.Text;
            View[nameof(Model.DeviceType).ToFieldKey()].Value = Model.DeviceType.Id;

            FormAdapter.Refresh();
        }

        protected override void BuildForm(EditFormAdapter form)
        {
            form.AddViewCell(nameof(Model.DeviceId));
            form.AddViewCell(nameof(Model.SerialNumber));
            form.AddViewCell(nameof(Model.DeviceType));
        }

        protected override string GetRequestUri()
        {
            return $"/api/device/{LaunchArgs.ParentId}/factory";
        }
    }
}
