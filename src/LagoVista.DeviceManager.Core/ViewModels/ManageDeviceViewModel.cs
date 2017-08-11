using LagoVista.Client.Core.ViewModels;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.DeviceManager.Core.ViewModels
{
    public class ManageDeviceViewModel : FormViewModelBase<Device>
    {
        string _deviceRepoId;
        string _deviceId;

        public override Task<InvokeResult> SaveRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void BuildForm(EditFormAdapter form)
        {
            form.AddViewCell(nameof(Model.DeviceId));
            form.AddViewCell(nameof(Model.SerialNumber));
            form.AddViewCell(nameof(Model.DeviceType));
        }

        protected override string GetRequestUri()
        {
            _deviceRepoId = LaunchArgs.Parameters[MonitorDeviceViewModel.DeviceRepoId].ToString();
            _deviceId = LaunchArgs.Parameters[MonitorDeviceViewModel.DeviceId].ToString();

            return $"/api/device/{_deviceRepoId}/{_deviceId}";
        }
    }
}
