using LagoVista.Client.Core.Resources;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Client.Core.ViewModels.Auth;
using LagoVista.Client.Core.ViewModels.Orgs;
using LagoVista.Core.Commanding;
using System;
using System.Collections.Generic;
using System.Text;
using LagoVista.IoT.Deployment.Admin.Models;

namespace LagoVista.PlatformManager.Core.ViewModels
{
    public class MainViewModel : ListViewModelBase<IoT.Deployment.Admin.Models.DeploymentInstanceSummary>
    {
        public MainViewModel()
        {
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

        protected override async void ItemSelected(DeploymentInstanceSummary model)
        {
            await NavigateAndViewAsync<InstanceViewModel>(model.Id);
        }

        protected override string GetListURI()
        {
            return $"/api/org/{AuthManager.User.CurrentOrganization.Id}/deployment/instances";
        }
    }
}
