using LagoVista.Client.Core.Resources;
using LagoVista.Client.Core.ViewModels.Orgs;
using LagoVista.Client.Core.ViewModels.Users;
using LagoVista.Core.Authentication.Interfaces;
using LagoVista.Core.Authentication.Models;
using LagoVista.Core.Commanding;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.ViewModels;
using LagoVista.UserAdmin.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Client.Core.ViewModels.Auth
{
    public class LoginViewModel : FormViewModelBase<UserInfo>
    {
        IAuthClient _authClient;
        IAppConfig _appConfig;
        IClientAppInfo _clientAppInfo;
        IDeviceInfo _deviceInfo;

        public LoginViewModel(IAuthClient authClient, IClientAppInfo clientAppInfo, IAppConfig appConfig, IDeviceInfo deviceInfo)
        {
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
            ForgotPasswordCommand = new RelayCommand(ForgotPassword);
            _authClient = authClient;
            _clientAppInfo = clientAppInfo;
            _appConfig = appConfig;
            _deviceInfo = deviceInfo;
        }

        public async void Register()
        {
            await ViewModelNavigation.NavigateAsync<RegisterUserViewModel>();
        }

        public async void ForgotPassword()
        {
            await ViewModelNavigation.NavigateAsync<SendResetPasswordLinkViewModel>();
        }

        public async void Login()
        {
            IsBusy = true;
            var loginInfo = new AuthRequest()
            {
                AppId = _appConfig.AppId,
                DeviceId = _deviceInfo.DeviceUniqueId,
                AppInstanceId = AuthManager.AppInstanceId,
                ClientType = "mobileapp",
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
                
                if(AuthManager.User.EmailConfirmed && AuthManager.User.PhoneNumberConfirmed)
                {
                    // If no org, have them add an org....
                    if (EntityHeader.IsNullOrEmpty(AuthManager.User.CurrentOrganization))
                    {
                        launchArgs.ViewModelType = typeof(OrgEditorViewModel);
                    }
                    else
                    {
                        // We are good, so show main screen.
                        launchArgs.ViewModelType = _clientAppInfo.MainViewModel;
                    }
                }
                else 
                {
                    // Show verify user screen.
                    launchArgs.ViewModelType = typeof(VerifyUserViewModel);
                }
                
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
        public RelayCommand ForgotPasswordCommand { get; private set; }


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
