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
using System;

namespace LagoVista.Simulator.Core.ViewModels
{
    public class MainViewModel : ListViewModelBase<IoT.Simulator.Admin.Models.SimulatorSummary>
    {
        public MainViewModel()
        {
            AddNewSimulatorCommand = new RelayCommand(AddNewSimulator);
            SettingsCommand = new RelayCommand(ToggleSettings);

            MenuItems = new List<MenuItem>()
            {
                new MenuItem()
                {
                    Command = new RelayCommand(() => ViewModelNavigation.NavigateAsync<UserOrgsViewModel>(this)),
                    Name = ClientResources.MainMenu_SwitchOrgs,
                    FontIconKey = "fa-users"
                },
                new MenuItem()
                {
                    Command = new RelayCommand(() => ViewModelNavigation.NavigateAsync<ChangePasswordViewModel>(this)),
                    Name = ClientResources.MainMenu_ChangePassword,
                    FontIconKey = "fa-key"
                },
                new MenuItem()
                {
                    Command = new RelayCommand(() => ViewModelNavigation.NavigateAsync<InviteUserViewModel>(this)),
                    Name = ClientResources.MainMenu_InviteUser,
                    FontIconKey = "fa-user"
                },
                new MenuItem()
                {
                    Command = new RelayCommand(() => Logout()),
                    Name = ClientResources.Common_Logout,
                    FontIconKey = "fa-sign-out"
                }
            };
        }

        public void AddNewSimulator()
        {
            ViewModelNavigation.NavigateAndCreateAsync<SimulatorEditorViewModel>(this);
        }       

        public void ToggleSettings()
        {
            MenuVisible = !MenuVisible;
        }

        protected override void ItemSelected(SimulatorSummary model)
        {
            SelectedItem = null;
            ViewModelNavigation.NavigateAndEditAsync<SimulatorViewModel>(this, model.Id);
        }

        protected override string GetListURI()
        {
            return $"/api/org/{AuthManager.User.CurrentOrganization.Id}/simulators";
        }

        public RelayCommand AddNewSimulatorCommand { get; private set; }

        public RelayCommand LogoutCommand { get; private set; }

        public RelayCommand SettingsCommand { get; private set; }
    }
}
