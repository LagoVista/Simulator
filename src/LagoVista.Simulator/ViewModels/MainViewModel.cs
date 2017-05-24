using LagoVista.Core.Commanding;
using LagoVista.IoT.Simulator.Admin.Models;
using LagoVista.Simulator.ViewModels.Auth;
using LagoVista.Simulator.ViewModels.Simulator;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels
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
            ShowViewModel<SimulatorEditorViewModel>();
        }

        public async override Task InitAsync()
        {
            
            IsBusy = true;
            var listResponse = await RestClient.GetForOrgAsync($"/api/org/{AuthManager.User.CurrentOrganization.Id}/simulators", null);
            IsBusy = false;
        }

        public async void Logout()
        {
            await AuthManager.LogoutAsync();
            ViewModelNavigation.SetAsNewRoot<LoginViewModel>();
        }


        ObservableCollection<SimulatorSummary> _simulators;
        public ObservableCollection<SimulatorSummary> Simulators
        {
            get { return _simulators; }
            set { Set(ref _simulators, value); }
        }

        SimulatorSummary _selectedSimulator;
        public SimulatorSummary SelectedSimulator
        {
            get { return _selectedSimulator; }
            set { Set(ref _selectedSimulator, value); }
        }


        public RelayCommand AddNewSimulatorCommand { get; private set; }

        public RelayCommand LogoutCommand { get; private set; }


    }
}
