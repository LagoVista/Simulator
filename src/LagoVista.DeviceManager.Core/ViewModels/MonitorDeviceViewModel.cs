using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using LagoVista.IoT.Runtime.Core.Models.Messaging;
using System;
using System.Linq;
using System.Diagnostics;
using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using LagoVista.IoT.DeviceAdmin.Models;
using System.Net.Http;

namespace LagoVista.DeviceManager.Core.ViewModels
{
    public class MonitorDeviceViewModel : MonitoringViewModelBase
    {
        public const string DeviceRepoId = "DEVICEREPOID";
        public const string DeviceId = "DEVICEID";

        string _deviceRepoId;
        string _deviceId;

        Device _device;

        List<InputCommandEndPoint> _inputCommandEndPoints;

        public MonitorDeviceViewModel()
        {
            EditDeviceCommand = new RelayCommand(EditDevice);
            DeviceMessages = new ObservableCollection<DeviceArchive>();
            SendCommand = new RelayCommand(Send);
            CancelSendCommand = new RelayCommand(CancelSend);
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
                    foreach (var endpoint in Device.InputCommandEndPoints)
                    {
                        Debug.WriteLine(endpoint.InputCommand.Name);
                        Debug.WriteLine(endpoint.EndPoint);
                    }
                    ViewReady = true;
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

        public async void Send()
        {
            await PerformNetworkOperation(async () =>
            {
                var queryString = String.Empty;
                foreach(var param in InputCommandParameters)
                {
                    if(!String.IsNullOrEmpty(queryString))
                    {
                        queryString += "&";
                    }

                    queryString += param.ToQueryStringPair();
                }

                var client = new HttpClient();
                var endPoint = _inputCommandEndPoints.Where(inp => inp.InputCommand.Key == SelectedInputCommand.Key).FirstOrDefault();
                if (endPoint != null)
                {
                    var uri = String.IsNullOrEmpty(queryString) ? endPoint.EndPoint : $"{endPoint.EndPoint}?{queryString}";
                    var result = await client.GetAsync(uri);
                    if(!result.IsSuccessStatusCode)
                    {
                        await Popups.ShowAsync(result.ReasonPhrase + $" ({result.StatusCode})");
                    }
                }
            });

            SelectedInputCommand = InputCommands.FirstOrDefault();
            DeviceStatusVisible = true;
            InputCommandVisible = false;
        }

        public void CancelSend()
        {
            SelectedInputCommand = InputCommands.FirstOrDefault();
            DeviceStatusVisible = true;
            InputCommandVisible = false;
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
                            DeviceMessages.Insert(0, archive);
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
            set
            {


                var attrs = new ObservableCollection<KeyValuePair<string, object>>();
                var stateMachines = new ObservableCollection<KeyValuePair<string, object>>();
                var inputCommands = new ObservableCollection<InputCommand>();

                if (value != null)
                {
                    foreach (var attr in value.Attributes)
                    {
                        attrs.Add(new KeyValuePair<string, object>(attr.Name, attr.Value));
                    }

                    foreach (var stm in value.States)
                    {
                        var state = stm.StateSet.Value.States.Where(stat => stat.Key == stm.Value).FirstOrDefault();
                        if (state != null)
                        {
                            stateMachines.Add(new KeyValuePair<string, object>(stm.Name, state.Name));
                        }
                        else
                        {
                            stateMachines.Add(new KeyValuePair<string, object>(stm.Name, stm.Value));
                        }
                    }

                    /* We only get these when the device is first loaded, not when the device
                     * is updated via web socket */
                    if (value.InputCommandEndPoints != null)
                    {
                        _inputCommandEndPoints = value.InputCommandEndPoints;
                        foreach (var cmd in _inputCommandEndPoints)
                        {
                            inputCommands.Add(cmd.InputCommand);
                        }

                        HasInputCommands = inputCommands.Count > 0;
                        if (HasInputCommands)
                        {
                            inputCommands.Insert(0, new InputCommand()
                            {
                                Key = "N/A",
                                Name = Resources.DeviceManagerResources.MonitorDevice_SelectOutputCommand
                            });
                            InputCommands = inputCommands;
                            SelectedInputCommand = inputCommands[0];
                        }
                        else
                        {
                            SelectedInputCommand = null;
                        }
                    }
                }
                
                StateMachines = stateMachines;
                DeviceAttributes = attrs;
                Set(ref _device, value);
            }
        }

        private void ShowInputCommand(InputCommand inputCommand)
        {
            InputCommandVisible = true;
            DeviceStatusVisible = false;

            InputCommandParameters = new ObservableCollection<Models.InputCommandParameter>();
            foreach(var param in inputCommand.Parameters)
            {
                InputCommandParameters.Add(new Models.InputCommandParameter(param));
            }
        }

        public bool _viewReady = false;
        public bool ViewReady
        {
            get { return _viewReady; }
            set { Set(ref _viewReady, value); }
        }

        public RelayCommand EditDeviceCommand { get; private set; }

        private ObservableCollection<DeviceArchive> _deviceMessasges;
        public ObservableCollection<DeviceArchive> DeviceMessages
        {
            get { return _deviceMessasges; }
            set { Set(ref _deviceMessasges, value); }
        }

        ObservableCollection<KeyValuePair<string, object>> _stateMachines;
        public ObservableCollection<KeyValuePair<string, object>> StateMachines
        {
            get { return _stateMachines; }
            set { Set(ref _stateMachines, value); }
        }

        ObservableCollection<KeyValuePair<string, object>> _deviceAttributes;
        public ObservableCollection<KeyValuePair<string, object>> DeviceAttributes
        {
            get { return _deviceAttributes; }
            set { Set(ref _deviceAttributes, value); }
        }

        public ObservableCollection<InputCommand> _inputCommands;
        public ObservableCollection<InputCommand> InputCommands
        {
            get { return _inputCommands; }
            set { Set(ref _inputCommands, value); }
        }

        InputCommand _selectedInputCommand;
        public InputCommand SelectedInputCommand
        {
            get { return _selectedInputCommand; }
            set
            {
                if (value != null)
                {
                    if (value != InputCommands.FirstOrDefault())
                    {
                        ShowInputCommand(value);
                    }                    
                }

                Set(ref _selectedInputCommand, value);
            }
        }

        RelayCommand _sendCommand;
        public RelayCommand SendCommand
        {
            get { return _sendCommand; }
            set { Set(ref _sendCommand, value); }
        }


        RelayCommand _cacnelSendCommand;
        public RelayCommand CancelSendCommand
        {
            get { return _cacnelSendCommand; }
            set { Set(ref _cacnelSendCommand, value); }
        }

        private bool _hasInputCommands = false;
        public bool HasInputCommands
        {
            get { return _hasInputCommands; }
            set { Set(ref _hasInputCommands, value); }
        }

        private bool _inputCommandVisible = false;
        public bool InputCommandVisible
        {
            get { return _inputCommandVisible; }
            set { Set(ref _inputCommandVisible, value); }
        }

        private bool _deviceStatusVisible = true;
        public bool DeviceStatusVisible
        {
            get { return _deviceStatusVisible; }
            set { Set(ref _deviceStatusVisible, value); }
        }

        ObservableCollection<Models.InputCommandParameter> _inputCommandParameters;
        public ObservableCollection<Models.InputCommandParameter> InputCommandParameters
        {
            get { return _inputCommandParameters; }
            set { Set(ref _inputCommandParameters, value); }
        }
    }
}
