using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.DeviceManager.Core.ViewModels
{
    public class MonitorDeviceViewModel : AppViewModelBase
    {
        public const string DeviceRepoId = "DEVICEREPOID";
        public const string DeviceId = "DEVICEID";

        string _deviceRepoId;
        string _deviceId;

        Device _device;

        public async override Task InitAsync()
        {
            _deviceRepoId = LaunchArgs.Parameters[DeviceRepoId].ToString();
            _deviceId = LaunchArgs.Parameters[DeviceId].ToString();

            await PerformNetworkOperation(async () =>
            {
                var response = await RestClient.GetAsync<DetailResponse<Device>>($"/api/device/{_deviceRepoId}/{_deviceId}");
                if (response.Successful)
                {
                    Device = response.Result.Model;
                }
            });
        }

        public Device Device
        {
            get { return _device; }
            set { Set(ref _device, value); }
        }
    }
}
