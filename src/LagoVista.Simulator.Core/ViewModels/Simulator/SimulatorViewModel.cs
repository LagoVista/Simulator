using LagoVista.Client.Core.Net;
using LagoVista.Core.Commanding;
using LagoVista.Core.IOC;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LagoVista.Simulator.Core.ViewModels.Simulator
{
    public class SimulatorViewModel : SimulatorViewModelBase<IoT.Simulator.Admin.Models.Simulator>
    {
        IMQTTDeviceClient _mqttClient;
        ITCPClient _tcpClient;
        IUDPClient _udpClient;

        bool _isConnected;


        public SimulatorViewModel()
        {
            ConnectCommand = new RelayCommand(Connect, CanConnect);
            DisconnectCommand = new RelayCommand(Disconnect, CanDisconnect);
        }

        public async void EditSimulator()
        {
            await ViewModelNavigation.NavigateAndEditAsync<SimulatorEditorViewModel>(Model.Id);
        }

        private Task LoadSimulator()
        {
            return PerformNetworkOperation(async () =>
            {
                var existingSimulator = await RestClient.CreateNewAsync($"/api/simulator/{LaunchArgs.ChildId}");
                if (existingSimulator != null)
                {
                    Model = existingSimulator.Model;
                    MessageTemplates = existingSimulator.Model.MessageTemplates;
                    ConnectCommand.RaiseCanExecuteChanged();
                    DisconnectCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged(nameof(ConnectionVisible));
                }
                else
                {
                    await Popups.ShowAsync("Sorry, could not load simulator, please try again later.");
                }

                return true;
            });
        }


        public async void Connect()
        {
            try
            {
                switch (Model.DefaultTransport.Value)
                {
                    case TransportTypes.MQTT:
                        _mqttClient = SLWIOC.Create<IMQTTDeviceClient>();
                        _mqttClient.BrokerHostName = Model.DefaultEndPoint;
                        _mqttClient.BrokerPort = Model.DefaultPort;
                        _mqttClient.DeviceId = Model.UserName;
                        _mqttClient.Password = Model.Password;

                        var result = await _mqttClient.ConnectAsync();
                        Debug.WriteLine(result);

                        break;
                    case TransportTypes.TCP:

                        _tcpClient = SLWIOC.Create<ITCPClient>();
                        await _tcpClient.ConnectAsync(Model.DefaultEndPoint, Model.DefaultPort);
                        break;
                    case TransportTypes.UDP:
                        break;
                }

                _isConnected = true;
                
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
            }
        }

        public async void Disconnect()
        {

            switch (Model.DefaultTransport.Value)
            {
                case TransportTypes.MQTT:
                    if(_mqttClient != null)
                    {
                        _mqttClient.Disconnect();
                        _mqttClient = null;
                    }
                    break;
                case TransportTypes.TCP:
                    if (_tcpClient != null)
                    {
                        try
                        {
                            await _tcpClient.CloseAsync();
                            _tcpClient.Dispose();
                        }
                        catch (Exception)
                        {

                        }
                        finally
                        {
                            _tcpClient = null;
                        }
                    }
                    break;
                case TransportTypes.UDP:
                    break;
            }

            _isConnected = false;
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
        }

        public bool CanConnect()
        {
            return Model != null && !_isConnected && (Model.DefaultTransport.Value == TransportTypes.AMQP ||
                            Model.DefaultTransport.Value == TransportTypes.MQTT ||
                            Model.DefaultTransport.Value == TransportTypes.TCP ||
                            Model.DefaultTransport.Value == TransportTypes.UDP);
        }

        public bool CanDisconnect()
        {
            return Model != null && _isConnected && (Model.DefaultTransport.Value == TransportTypes.AMQP ||
                            Model.DefaultTransport.Value == TransportTypes.MQTT ||
                            Model.DefaultTransport.Value == TransportTypes.TCP ||
                            Model.DefaultTransport.Value == TransportTypes.UDP);
        }

        public bool ConnectionVisible
        {
            get
            {
                return Model != null && (Model.DefaultTransport.Value == TransportTypes.AMQP ||
                                 Model.DefaultTransport.Value == TransportTypes.MQTT ||
                                 Model.DefaultTransport.Value == TransportTypes.TCP ||
                                 Model.DefaultTransport.Value == TransportTypes.UDP);
            }
        }

        public override void Edit()
        {
            EditSimulator();
        }

        public override Task InitAsync()
        {
            return LoadSimulator();
        }

        public override Task ReloadedAsync()
        {
            return LoadSimulator();
        }

        List<MessageTemplate> _messageTemplates;
        public List<MessageTemplate> MessageTemplates
        {
            get { return _messageTemplates; }
            set { Set(ref _messageTemplates, value); }
        }

        public RelayCommand ConnectCommand { get; set; }
        public RelayCommand DisconnectCommand { get; set; }

        MessageTemplate _selectedMessageTemplate;
        public MessageTemplate SelectedMessageTemplate
        {
            get { return _selectedMessageTemplate; }
            set
            {
                if (value != null && _selectedMessageTemplate != value)
                {
                    var launchArgs = new ViewModelLaunchArgs()
                    {
                        ViewModelType = typeof(Messages.SendMessageViewModel),
                        Parent = value,
                        LaunchType = LaunchTypes.Other,
                    };

                    if (_tcpClient != null) launchArgs.Parameters.Add("tcpclient", _tcpClient);
                    if (_mqttClient != null) launchArgs.Parameters.Add("mqttclient", _mqttClient);
                    if (_udpClient != null) launchArgs.Parameters.Add("udpclient", _udpClient);

                    ViewModelNavigation.NavigateAsync(launchArgs);
                }

                _selectedMessageTemplate = value;
                RaisePropertyChanged();
            }
        }

    }
}
