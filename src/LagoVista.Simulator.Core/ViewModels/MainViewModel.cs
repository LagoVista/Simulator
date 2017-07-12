using LagoVista.Core.Commanding;
using LagoVista.Core;
using LagoVista.IoT.Simulator.Admin.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LagoVista.Client.Core.Resources;
using LagoVista.Simulator.Core.ViewModels.Simulator;
using System.Collections.Generic;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Client.Core.ViewModels.Auth;
using LagoVista.Client.Core.ViewModels.Orgs;
using LagoVista.Core.Validation;

namespace LagoVista.Simulator.Core.ViewModels
{
    public class MainViewModel : ListViewModelBase<IoT.Simulator.Admin.Models.Simulator, IoT.Simulator.Admin.Models.SimulatorSummary>
    {
        public MainViewModel()
        {
            AddNewSimulatorCommand = new RelayCommand(AddNewSimulator);
            LogoutCommand = new RelayCommand(Logout);
            SettingsCommand = new RelayCommand(ToggleSettings);

            MenuItems = new List<MenuItem>()
            {
                new MenuItem()
                {
                    Command = new RelayCommand(() => ViewModelNavigation.NavigateAsync<ChangePasswordViewModel>()),
                    Name = ClientResources.MainMenu_ChangePassword,
                    FontIconKey = "fa-key"
                },
                new MenuItem()
                {
                    Command = new RelayCommand(() => ViewModelNavigation.NavigateAsync<InviteUserViewModel>()),
                    Name = ClientResources.MainMenu_InviteUser,
                    FontIconKey = "fa-user"
                },
                new MenuItem()
                {
                    Command = LogoutCommand,
                    Name = ClientResources.Common_Logout,
                    FontIconKey = "fa-sign-out"
                }
            };
        }

        public void AddNewSimulator()
        {
            ViewModelNavigation.NavigateAndCreateAsync<SimulatorEditorViewModel>();
        }

        public async Task<InvokeResult> GetSimulatorsAsync()
        {
            Simulators = null;
            var listResponse = await FormRestClient.GetForOrgAsync($"/api/org/{AuthManager.User.CurrentOrganization.Id}/simulators", null);
            if (listResponse == null)
            {
                await Popups.ShowAsync(ClientResources.Common_ErrorCommunicatingWithServer);
            }
            else
            {
                Simulators = listResponse.Model.ToObservableCollection();
            }

            return InvokeResult.Success;
        }
       

        public void ToggleSettings()
        {
            MenuVisible = !MenuVisible;
        }

        public override Task InitAsync()
        {
            return PerformNetworkOperation(GetSimulatorsAsync);
        }

        public override Task ReloadedAsync()
        {
            SelectedSimulator = null;
            return PerformNetworkOperation(GetSimulatorsAsync);
        }


        ObservableCollection<SimulatorSummary> _simulators;
        public ObservableCollection<SimulatorSummary> Simulators
        {
            get { return _simulators; }
            set { Set(ref _simulators, value); }
        }

        /* so far will always be null just used to detect clicking on object */
        SimulatorSummary _selectedSimulator;
        public SimulatorSummary SelectedSimulator
        {
            get { return _selectedSimulator; }
            set
            {
                if (value != null && _selectedSimulator != value)
                {
                    ViewModelNavigation.NavigateAndEditAsync<SimulatorViewModel>(value.Id);
                }

                _selectedSimulator = value;

                RaisePropertyChanged();
            }
        }

        public RelayCommand AddNewSimulatorCommand { get; private set; }

        public RelayCommand LogoutCommand { get; private set; }

        public RelayCommand SettingsCommand { get; private set; }
    }
}
