using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using LagoVista.IoT.Runtime.Core.Models.Messaging;
using System;
using System.Diagnostics;
using LagoVista.Core;
using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace LagoVista.DeviceManager.Core.ViewModels
{
    public class MonitorDeviceViewModel : MonitoringViewModelBase
    {
        public const string DeviceRepoId = "DEVICEREPOID";
        public const string DeviceId = "DEVICEID";

        string _deviceRepoId;
        string _deviceId;

        Device _device;

        public MonitorDeviceViewModel()
        {
            EditDeviceCommand = new RelayCommand(EditDevice);
            DeviceMessages = new ObservableCollection<DeviceArchive>();
        }

        public async override Task InitAsync()
        {
            _deviceRepoId = LaunchArgs.Parameters[DeviceRepoId].ToString();
            _deviceId = LaunchArgs.Parameters[DeviceId].ToString();

            await base.InitAsync();

            await PerformNetworkOperation(async () =>
            {
                var response = await RestClient.GetAsync<DetailResponse<Device>>($"/api/device/{_deviceRepoId}/{_deviceId}/metadata");
                if (response.Successful)
                {
                    Device = response.Result.Model;
                }
            });
        }

        public void EditDevice()
        {
            var launchArgs = new ViewModelLaunchArgs()
            {
                ViewModelType = typeof(ManageDeviceViewModel),
                LaunchType = LaunchTypes.Edit
            };

            launchArgs.Parameters.Add(MonitorDeviceViewModel.DeviceRepoId, _deviceRepoId);
            launchArgs.Parameters.Add(MonitorDeviceViewModel.DeviceId, _deviceId);

            ViewModelNavigation.NavigateAsync(launchArgs);
        }

        public override void HandleMessage(Notification notification)
        {

            if (!String.IsNullOrEmpty(notification.PayloadType))
            {
                switch (notification.PayloadType)
                {
                    case "DeviceArchive":
                        var archive = JsonConvert.DeserializeObject<DeviceArchive>(notification.Payload);
                        DispatcherServices.Invoke(() =>
                        {
                            DeviceMessages.Insert(0,archive);
                        });
                        break;
                    case "LagoVista.IoT.DeviceManagement.Core.Models.Device":
                        DispatcherServices.Invoke(() =>
                        {
                            Device = JsonConvert.DeserializeObject<Device>(notification.Payload);
                        });

                        break;
                }
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
            //            return "/api/wsuri/instance/5E78188E767349D681898F0AD8CD1FFC/normal";
        }

        public Device Device
        {
            get { return _device; }
            set {
                Set(ref _device, value);
               
                var attrs = new ObservableCollection<KeyValuePair<string, object>>();
                if (value != null)
                {
                    foreach (var attr in Device.Attributes)
                    {
                        attrs.Add(new KeyValuePair<string, object>(attr.Name, attr.Value));
                    }
                }

                DeviceAttributes = attrs;
            }
        }

        public RelayCommand EditDeviceCommand { get; private set; }

        private ObservableCollection<DeviceArchive> _deviceMessasges;
        public ObservableCollection<DeviceArchive> DeviceMessages
        {
            get { return _deviceMessasges; }
            set { Set(ref _deviceMessasges, value); }
        }


        ObservableCollection<KeyValuePair<string, object>> _deviceAttributes;
        public ObservableCollection<KeyValuePair<string, object>> DeviceAttributes
        {
            get { return _deviceAttributes; }
            set { Set(ref _deviceAttributes, value); }
        }
    }
}
