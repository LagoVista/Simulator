using System.Threading.Tasks;
using LagoVista.Client.Core.Net;
using LagoVista.Core.Authentication.Interfaces;
using LagoVista.Core.Authentication.Models;
using LagoVista.Core.Commanding;
using LagoVista.Core.Models;
using LagoVista.UserAdmin.Models.Users;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Client.Core.Resources;

namespace LagoVista.Simulator.Core.ViewModels.Auth
{
    public class LoginViewModel : IoTAppViewModelBase<UserInfo>
    {
        IAuthClient _authClient;

        public LoginViewModel(IAuthClient authClient)
        {
            LoginCommand = new RelayCommand(Login);
            _authClient = authClient;
        }


        public async void Login()
        {
            IsBusy = true;
            var loginInfo = new AuthRequest()
            {
                Email = EmailAddress,
                Password = Password,
                UserName = EmailAddress,
                GrantType = "password"
            };

            var result = await _authClient.LoginAsync(loginInfo);

            if (result.Success)
            {
                var authResult = result.Result;
                AuthManager.AuthToken = authResult.AuthToken;
                AuthManager.AuthTokenExpiration = authResult.AuthTokenExpiration;
                AuthManager.RefreshToken = authResult.RefreshToken;
                AuthManager.RefreshTokenExpiration = authResult.RefreshTokenExpiration;
                AuthManager.IsAuthenticated = true;

                var user = await RestClient.GetAsync("/api/user");
                AuthManager.User = user.Model;
                await AuthManager.PersistAsync();
                await ViewModelNavigation.NavigateAsync<MainViewModel>();
                IsBusy = false;
            }
            else
            {
                IsBusy = false;
                await Popups.ShowAsync(ClientResources.Auth_FailedLogin);
            }
        }

        public RelayCommand LoginCommand { get; private set; }

        private string _emailAddress;
        private string _password;

        public string EmailAddress
        {
            get { return _emailAddress; }
            set { Set(ref _emailAddress, value); }
        }

        public string Password
        {
            get { return _password; }
            set { Set(ref _password, value); }
        }
    }
}
