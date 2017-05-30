using LagoVista.Core.Commanding;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels.Simulator
{
    public class SimulatorViewModel : SimulatorViewModelBase<IoT.Simulator.Admin.Models.Simulator>
    {
        public SimulatorViewModel()
        {
            EditSimulatorCommand = new RelayCommand(EditSimulator);
            ConnectCommand = new RelayCommand(Connect, CanConnect);
            DisconnectCommand = new RelayCommand(Disconnect, CanDisconnect);

        }

        public async void EditSimulator()
        {
            await ViewModelNavigation.NavigateAndEditAsync<SimulatorEditorViewModel>(Model.Id);
        }

        public void Connect()
        {

        }

        public void Disconnect()
        {

        }

        public bool CanConnect()
        {
            return true;
        }

        public bool CanDisconnect()
        {
            return false;
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
               }
               else
               {
                   await Popups.ShowAsync("Sorry, could not load simulator, please try again later.");
               }

               return true;
           });
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
                    ViewModelNavigation.NavigateAsync(new ViewModelLaunchArgs()
                    {
                        ViewModelType = typeof(Messages.SendMessageViewModel),
                        Parent = value,
                        LaunchType = LaunchTypes.Other
                    });
                }

                _selectedMessageTemplate = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand EditSimulatorCommand { get; private set; }
    }
}
