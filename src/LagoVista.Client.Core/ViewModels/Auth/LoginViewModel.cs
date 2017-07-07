using LagoVista.Client.Core.Resources;
using LagoVista.Client.Core.ViewModels.Users;
using LagoVista.Core.Authentication.Interfaces;
using LagoVista.Core.Authentication.Models;
using LagoVista.Core.Commanding;
using LagoVista.Core.Models;
using LagoVista.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Client.Core.ViewModels.Auth
{
    public class LoginViewModel : IoTAppViewModelBase<UserInfo>
    {
        IAuthClient _authClient;
        IClientAppInfo _clientAppInfo;

        public LoginViewModel(IAuthClient authClient, IClientAppInfo clientAppInfo)
        {
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
            _authClient = authClient;
            _clientAppInfo = clientAppInfo;
        }

        public async void Register()
        {
            await ViewModelNavigation.NavigateAsync<RegisterUserViewModel>();
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

            if (result.Successful)
            {
                var authResult = result.Result;
                AuthManager.AccessToken = authResult.AccessToken;
                AuthManager.AccessTokenExpirationUTC = authResult.AccessTokenExpiresUTC;
                AuthManager.RefreshToken = authResult.RefreshToken;
                AuthManager.AppInstanceId = authResult.AppInstanceId;
                AuthManager.RefreshTokenExpirationUTC = authResult.RefreshTokenExpiresUTC;
                AuthManager.IsAuthenticated = true;
                
                var user = await RestClient.GetAsync("/api/user");
                AuthManager.User = user.Model;
                await AuthManager.PersistAsync();
                var launchArgs = new ViewModelLaunchArgs();
                launchArgs.ViewModelType = _clientAppInfo.MainViewModel;
                launchArgs.LaunchType = LaunchTypes.View;
                await ViewModelNavigation.NavigateAsync(launchArgs);
                IsBusy = false;
            }
            else
            {
                IsBusy = false;
                await Popups.ShowAsync(ClientResources.Auth_FailedLogin);
            }
        }

        public RelayCommand LoginCommand { get; private set; }
        public RelayCommand RegisterCommand { get; private set; }

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
