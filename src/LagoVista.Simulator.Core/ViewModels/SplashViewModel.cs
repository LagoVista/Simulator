using LagoVista.Client.Core.ViewModels;
using LagoVista.Client.Core.ViewModels.Users;
using LagoVista.Core.Commanding;
using LagoVista.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Simulator.Core.ViewModels
{
    public class SplashViewModel : IoTAppViewModelBase
    {

        private bool _notLoggedIn = false;
        public bool NotLoggedIn
        {
            get { return _notLoggedIn; }
            set { Set(ref _notLoggedIn, value); }
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }

        public SplashViewModel()
        {
            LoginCommand = new RelayCommand(Login);
            NotLoggedIn = false;
            IsLoading = true;
        }

        public override async Task InitAsync()
        {
            await AuthManager.LoadAsync();
            if (AuthManager.IsAuthenticated)
            {
                if (EntityHeader.IsNullOrEmpty(AuthManager.User.CurrentOrganization))
                {
                    Logger.AddKVPs(new KeyValuePair<string, string>("Email", AuthManager.User.Email));
                }
                else
                {
                    Logger.AddKVPs(new KeyValuePair<string, string>("Email", AuthManager.User.Email), new KeyValuePair<string, string>("OrgId", AuthManager.User.CurrentOrganization.Text));
                }

                if (!AuthManager.User.EmailConfirmed || !AuthManager.User.PhoneNumberConfirmed)
                {
                    await ViewModelNavigation.SetAsNewRootAsync<VerifyUserViewModel>();
                }
                else
                {
                    await ViewModelNavigation.SetAsNewRootAsync<MainViewModel>();
                }
            }
            else
            {
                NotLoggedIn = true;
                IsLoading = false;
            }
        }

        public async void Login()
        {
            await ViewModelNavigation.SetAsNewRootAsync<Auth.LoginViewModel>();
        }

        public RelayCommand LoginCommand { get; private set; }
    }
}