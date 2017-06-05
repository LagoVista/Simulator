using LagoVista.Core.Commanding;
using LagoVista.Core;
using LagoVista.IoT.Simulator.Admin.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LagoVista.Client.Core.Resources;
using LagoVista.Simulator.Core.ViewModels.Simulator;
using LagoVista.Simulator.Core.ViewModels.Auth;

namespace LagoVista.Simulator.Core.ViewModels
{
    public class MainViewModel : SimulatorViewModelBase<IoT.Simulator.Admin.Models.Simulator, IoT.Simulator.Admin.Models.SimulatorSummary>
    {
        public MainViewModel()
        {
            AddNewSimulatorCommand = new RelayCommand(AddNewSimulator);
            LogoutCommand = new RelayCommand(Logout);
        }


        public void AddNewSimulator()
        {
            ViewModelNavigation.NavigateAndCreateAsync<SimulatorEditorViewModel>();
        }

        private Task LoadSimulators()
        {
            return PerformNetworkOperation(async () =>
            {
                Simulators = null;
                var listResponse = await RestClient.GetForOrgAsync($"/api/org/{AuthManager.User.CurrentOrganization.Id}/simulators", null);
                if (listResponse == null)
                {
                    await Popups.ShowAsync(ClientResources.Common_ErrorCommunicatingWithServer);
                }
                else
                {
                    Simulators = listResponse.Model.ToObservableCollection();
                }
            });
        }

        public override Task InitAsync()
        {
            return LoadSimulators();
        }

        public override Task ReloadedAsync()
        {
            SelectedSimulator = null;
            return LoadSimulators();
        }

        public async void Logout()
        {
            await AuthManager.LogoutAsync();
            await ViewModelNavigation.SetAsNewRootAsync<LoginViewModel>();
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
    }
}
