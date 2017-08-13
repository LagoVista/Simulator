using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using LagoVista.IoT.Runtime.Core.Models.Messaging;
using System;
using System.Diagnostics;

namespace LagoVista.DeviceManager.Core.ViewModels
{
    public class MonitorDeviceViewModel : MonitoringViewModelBase
    {
        public const string DeviceRepoId = "DEVICEREPOID";
        public const string DeviceId = "DEVICEID";

        IWebSocket _webSocket;

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

            await base.InitAsync();
        }

        public override void HandleMessage(Notification notification)
        {
            if (!String.IsNullOrEmpty(notification.PayloadType))
            {
                Debug.WriteLine("----");
                Debug.WriteLine(notification.PayloadType);
                Debug.WriteLine(notification.Payload);
                Debug.WriteLine("BYTES: " + notification.Payload.Length);
                Debug.WriteLine("----");
            }
            else
            {
                Debug.WriteLine(notification.Text);
            }
        }

        public override string GetChannelURI()
        {
            return $"/api/wsuri/device/{_deviceId}/normal";
        }

        public Device Device
        {
            get { return _device; }
            set { Set(ref _device, value); }
        }
    }
}
