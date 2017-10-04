using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Commanding;
using LagoVista.Core.IOC;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Validation;
using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using LagoVista.Simulator.Core.Utils;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace LagoVista.Simulator.Core.ViewModels.Simulator
{
    public class SimulatorViewModel : AppViewModelBase
    {
        IMQTTDeviceClient _mqttClient;
        ITCPClient _tcpClient;
        IUDPClient _udpClient;
        EventHubClient _eventHubClient;
        DeviceClient _azureIoTHubClient;
        QueueClient _queueClient;

        bool _isConnected;


        Task ReceivingTask;


        public SimulatorViewModel()
        {
            ConnectCommand = new RelayCommand(Connect);
            DisconnectCommand = new RelayCommand(Disconnect);
            ShowMessageTemplatesCommand = new RelayCommand(ShowMessageTemplates);
            ShowReceivedMessagesCommand = new RelayCommand(ShowReceivedMessages);
        }

        public async void EditSimulator()
        {
            if (_isConnected)
            {
                await DisconnectAsync();
            }
            await ViewModelNavigation.NavigateAndEditAsync<SimulatorEditorViewModel>(this, Model.Id);
        }

        private async Task<InvokeResult> LoadSimulator()
        {
            var simulatorResponse = await RestClient.GetAsync<DetailResponse<LagoVista.IoT.Simulator.Admin.Models.Simulator>>($"/api/simulator/{LaunchArgs.ChildId}");
            if (simulatorResponse.Successful)
            {
                Model = simulatorResponse.Result.Model;
                MessageTemplates = simulatorResponse.Result.Model.MessageTemplates;

                switch (Model.DefaultTransport.Value)
                {
                    case TransportTypes.TCP:
                    case TransportTypes.UDP:
                    case TransportTypes.AMQP:
                    case TransportTypes.AzureIoTHub:
                    case TransportTypes.AzureServiceBus:
                    case TransportTypes.MQTT:
                        ConnectButtonVisible = true;
                        DisconnectButtonVisible = false;
                        break;
                    default:
                        MessageTemplatesVisible = true;
                        ConnectButtonVisible = false;
                        DisconnectButtonVisible = false;
                        break;
                }

                return InvokeResult.Success;
            }
            else
            {
                return simulatorResponse.ToInvokeResult();
            }
        }

        private void StartReceiveThread()
        {
            Task.Run(async () =>
            {
                while (_isConnected)
                {
                    var response = await _tcpClient.ReceiveAsync();


                }
            });
        }

        public async void Connect()
        {
            try
            {
                IsBusy = true;
                switch (Model.DefaultTransport.Value)
                {

                    case TransportTypes.AzureEventHub:
                    case TransportTypes.AMQP:
                        {
                            var connectionString = $"Endpoint=sb://{Model.DefaultEndPoint}.servicebus.windows.net/;SharedAccessKeyName={Model.AccessKeyName};SharedAccessKey={Model.AccessKey}";
                            var bldr = new EventHubsConnectionStringBuilder(connectionString)
                            {
                                EntityPath = Model.HubName
                            };

                            _eventHubClient = EventHubClient.CreateFromConnectionString(bldr.ToString());
                            _isConnected = true;
                        }

                        break;
                    case TransportTypes.AzureServiceBus:
                        {
                            var connectionString = $"Endpoint=sb://{Model.DefaultEndPoint}.servicebus.windows.net/;SharedAccessKeyName={Model.AccessKeyName};SharedAccessKey={Model.AccessKey}";
                            var bldr = new ServiceBusConnectionStringBuilder(connectionString)
                            {
                                EntityPath = Model.HubName
                            };

                            _queueClient = new QueueClient(bldr, ReceiveMode.PeekLock, Microsoft.Azure.ServiceBus.RetryExponential.Default);
                            ShowMessageTemplates();
                            ConnectionIconVisible = true;
                            ConnectionColor = "green";
                            ConnectButtonVisible = false;
                            DisconnectButtonVisible = true;
                            ViewSelectorVisible = true;

                            _queueClient.RegisterMessageHandler(async (msg, token) =>
                            {
                                await Task.Delay(1);
                            }, async (exception) =>
                            {
                                await Task.Delay(1);
                            });

                        }
                        break;
                    case TransportTypes.AzureIoTHub:
                        {
                            var connectionString = $"HostName={Model.DefaultEndPoint};DeviceId={Model.DeviceId};SharedAccessKey={Model.AccessKey}";
                            _azureIoTHubClient = DeviceClient.CreateFromConnectionString(connectionString, Microsoft.Azure.Devices.Client.TransportType.Amqp_Tcp_Only);
                            await _azureIoTHubClient.OpenAsync();
                            ReceivingTask = Task.Run(ReceiveDataFromAzure);
                            _isConnected = true;
                        }
                        break;
                    case TransportTypes.MQTT:
                        _mqttClient = SLWIOC.Create<IMQTTDeviceClient>();
                        _mqttClient.ShowDiagnostics = true;
                        _mqttClient.BrokerHostName = Model.DefaultEndPoint;
                        _mqttClient.BrokerPort = Model.DefaultPort;
                        _mqttClient.DeviceId = Model.UserName;
                        _mqttClient.Password = Model.Password;
                        var result = await _mqttClient.ConnectAsync();
                        if (result.Result == ConnAck.Accepted)
                        {
                            _isConnected = true;
                            _mqttClient.MessageReceived += _mqttClient_CommandReceived;
                            ShowMessageTemplates();
                            ConnectionIconVisible = true;
                            ConnectionColor = "green";
                            ConnectButtonVisible = false;
                            DisconnectButtonVisible = true;
                            if (!String.IsNullOrEmpty(Model.Subscription))
                            {
                                ViewSelectorVisible = true;
                                _mqttClient.Subscribe(Model.Subscription.Replace("{deviceid}", _model.DeviceId));
                            }
                        }
                        else
                        {
                            await Popups.ShowAsync($"{Resources.SimulatorCoreResources.Simulator_ErrorConnecting}: {result.Message}");
                        }

                        break;
                    case TransportTypes.TCP:
                        _tcpClient = SLWIOC.Create<ITCPClient>();
                        await _tcpClient.ConnectAsync(Model.DefaultEndPoint, Model.DefaultPort);
                        _isConnected = true;
                        StartReceiveThread();
                        break;
                    case TransportTypes.UDP:
                        _udpClient = SLWIOC.Create<IUDPClient>();
                        await _udpClient.ConnectAsync(Model.DefaultEndPoint, Model.DefaultPort);
                        _isConnected = true;
                        StartReceiveThread();
                        break;
                }

                RightMenuIcon = Client.Core.ViewModels.RightMenuIcon.None;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await Popups.ShowAsync(ex.Message);
                _isConnected = false;
            }
            finally
            {
                ConnectCommand.RaiseCanExecuteChanged();
                DisconnectCommand.RaiseCanExecuteChanged();
                IsBusy = false;
            }
        }

        private void _mqttClient_CommandReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Debug.WriteLine(e.Topic);
        }

        public override Task<bool> CanCancelAsync()
        {
            if (_isConnected)
            {
                Disconnect();
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(true);
            }
        }



        public async Task DisconnectAsync()
        {
            switch (Model.DefaultTransport.Value)
            {
                case TransportTypes.AMQP:

                    ConnectButtonVisible = true;
                    break;
                case TransportTypes.AzureEventHub:

                    ConnectButtonVisible = true;
                    break;
                case TransportTypes.AzureIoTHub:
                    if (_azureIoTHubClient != null)
                    {
                        await _azureIoTHubClient.CloseAsync();
                        _azureIoTHubClient.Dispose();
                        _azureIoTHubClient = null;
                    }

                    ConnectButtonVisible = true;
                    break;
                case TransportTypes.MQTT:
                    if (_mqttClient != null)
                    {
                        _mqttClient.Disconnect();
                        _mqttClient = null;

                    }

                    ConnectButtonVisible = true;
                    break;
                case TransportTypes.TCP:
                    if (_tcpClient != null)
                    {
                        await _tcpClient.DisconnectAsync();
                        _tcpClient.Dispose();
                    }

                    ConnectButtonVisible = true;
                    break;
                case TransportTypes.UDP:
                    if (_udpClient != null)
                    {
                        await _udpClient.DisconnectAsync();
                        _udpClient.Dispose();
                    }
                    ConnectButtonVisible = true;
                    break;
            }

            _isConnected = false;

            DisconnectButtonVisible = false;
            ViewSelectorVisible = false;
            MessageTemplatesVisible = false;
            ReceivedMessagesVisible = false;
            ConnectionColor = "red";


            RightMenuIcon = Client.Core.ViewModels.RightMenuIcon.None;
        }

        public void ShowReceivedMessages()
        {
            ReceivedMessagesVisible = true;
            MessageTemplatesVisible = false;
        }

        public void ShowMessageTemplates()
        {
            ReceivedMessagesVisible = false;
            MessageTemplatesVisible = true;
        }

        public async void Disconnect()
        {
            await DisconnectAsync();
        }

        public bool CanConnect()
        {
            return Model != null && !_isConnected && (Model.DefaultTransport.Value == TransportTypes.AMQP ||
                            Model.DefaultTransport.Value == TransportTypes.MQTT ||
                             Model.DefaultTransport.Value == TransportTypes.AzureEventHub ||
                             Model.DefaultTransport.Value == TransportTypes.AzureIoTHub ||
                             Model.DefaultTransport.Value == TransportTypes.AzureServiceBus ||
                            Model.DefaultTransport.Value == TransportTypes.TCP ||
                            Model.DefaultTransport.Value == TransportTypes.UDP);
        }

        public bool CanDisconnect()
        {
            return Model != null && _isConnected && (Model.DefaultTransport.Value == TransportTypes.AMQP ||
                            Model.DefaultTransport.Value == TransportTypes.MQTT ||
                             Model.DefaultTransport.Value == TransportTypes.AzureEventHub ||
                             Model.DefaultTransport.Value == TransportTypes.AzureIoTHub ||
                             Model.DefaultTransport.Value == TransportTypes.AzureServiceBus ||
                            Model.DefaultTransport.Value == TransportTypes.TCP ||
                            Model.DefaultTransport.Value == TransportTypes.UDP);
        }

        public override void Edit()
        {
            EditSimulator();
        }

        public override async Task InitAsync()
        {
            var result = await PerformNetworkOperation(LoadSimulator);
            if (!result.Successful)
            {
                await this.ViewModelNavigation.GoBackAsync();
            }
        }

        public override Task ReloadedAsync()
        {
            return PerformNetworkOperation(LoadSimulator);
        }

        List<MessageTemplate> _messageTemplates;
        public List<MessageTemplate> MessageTemplates
        {
            get { return _messageTemplates; }
            set { Set(ref _messageTemplates, value); }
        }

        public RelayCommand ConnectCommand { get; set; }
        public RelayCommand DisconnectCommand { get; set; }

        public RelayCommand ShowReceivedMessagesCommand { get; set; }
        public RelayCommand ShowMessageTemplatesCommand { get; set; }


        MessageTemplate _selectedMessageTemplate;
        public MessageTemplate SelectedMessageTemplate
        {
            get { return _selectedMessageTemplate; }
            set
            {
                if (value != null && _selectedMessageTemplate != value)
                {
                    if (Model.DefaultTransport.Value == TransportTypes.AMQP ||
                        Model.DefaultTransport.Value == TransportTypes.TCP ||
                        Model.DefaultTransport.Value == TransportTypes.UDP ||
                        Model.DefaultTransport.Value == TransportTypes.AzureIoTHub ||
                        Model.DefaultTransport.Value == TransportTypes.AzureServiceBus ||
                        Model.DefaultTransport.Value == TransportTypes.MQTT)
                    {
                        if (!_isConnected)
                        {
                            Popups.ShowAsync(Resources.SimulatorCoreResources.Simulator_PleaseConnect);
                            _selectedMessageTemplate = null;
                            RaisePropertyChanged();
                            return;
                        }
                    }

                    var launchArgs = new ViewModelLaunchArgs()
                    {
                        ViewModelType = typeof(Messages.SendMessageViewModel),
                        Parent = value,
                        LaunchType = LaunchTypes.Other,
                    };

                    if (_eventHubClient != null) launchArgs.Parameters.Add("ehclient", _eventHubClient);

                    if (_mqttClient != null) launchArgs.Parameters.Add("mqttclient", _mqttClient);
                    if (_tcpClient != null) launchArgs.Parameters.Add("tcpclient", _tcpClient);
                    if (_udpClient != null) launchArgs.Parameters.Add("udpclient", _udpClient);
                    if (_azureIoTHubClient != null) launchArgs.Parameters.Add("azureIotHubClient", _azureIoTHubClient);

                    ViewModelNavigation.NavigateAsync(launchArgs);
                }

                _selectedMessageTemplate = value;
                RaisePropertyChanged();
            }
        }

        private async Task ReceiveDataFromAzure()
        {
            while (_azureIoTHubClient != null)
            {
                var message = await _azureIoTHubClient.ReceiveAsync();
                if (message != null)
                {
                    try
                    {
                        var responseMessage = System.Text.UTF8Encoding.ASCII.GetString(message.GetBytes());
                        DispatcherServices.Invoke(() =>
                        {
                            Debug.WriteLine(message.To);
                            Debug.WriteLine(message.MessageId);
                            Debug.WriteLine(responseMessage);

                            foreach (var prop in message.Properties)
                            {
                                Debug.WriteLine($"\t\t{prop.Key}={prop.Value}");
                            }

                        });
                        // Received a new message, display it
                        // We received the message, indicate IoTHub we treated it
                        await _azureIoTHubClient.CompleteAsync(message);
                    }
                    catch
                    {
                        await _azureIoTHubClient.RejectAsync(message);
                    }
                }
            }
        }


        private string _connectionColor = "red";
        public string ConnectionColor
        {
            get { return _connectionColor; }
            set { Set(ref _connectionColor, value); }
        }


        private bool _viewSelectorVisible = false;
        public bool ViewSelectorVisible
        {
            get { return _viewSelectorVisible; }
            set
            {
                _viewSelectorVisible = value;
                RaisePropertyChanged();
            }
        }

        private bool _connetionIconVisible = true;
        public bool ConnectionIconVisible
        {
            get { return _connetionIconVisible; }
            set
            {
                _connetionIconVisible = value;
                RaisePropertyChanged();
            }
        }

        private bool _connectButtonVisible = false;
        public bool ConnectButtonVisible
        {
            get { return _connectButtonVisible; }
            set
            {
                _connectButtonVisible = value;
                RaisePropertyChanged();
            }
        }

        private bool _disconnectButtonVisbile = false;
        public bool DisconnectButtonVisible
        {
            get { return _disconnectButtonVisbile; }
            set
            {
                _disconnectButtonVisbile = value;
                RaisePropertyChanged();
            }
        }

        private bool _messageTemplatesVisible = false;
        public bool MessageTemplatesVisible
        {
            get { return _messageTemplatesVisible; }
            set
            {
                _messageTemplatesVisible = value;
                RaisePropertyChanged();
            }
        }

        private bool _receiveeMessageVisible = false;
        public bool ReceivedMessagesVisible
        {
            get { return _receiveeMessageVisible; }
            set
            {
                _receiveeMessageVisible = value;
                RaisePropertyChanged();
            }
        }

        LagoVista.IoT.Simulator.Admin.Models.Simulator _model;
        public LagoVista.IoT.Simulator.Admin.Models.Simulator Model
        {
            get { return _model; }
            set { Set(ref _model, value); }
        }
    }
}
