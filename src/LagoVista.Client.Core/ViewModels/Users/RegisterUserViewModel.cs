using LagoVista.Client.Core.Resources;
using LagoVista.Core.Authentication.Models;
using LagoVista.Core.Commanding;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.ViewModels;
using LagoVista.UserAdmin.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.ViewModels.Users
{
    public class RegisterUserViewModel : IoTAppViewModelBase<RegisterViewModel>
    {
        public RegisterUserViewModel(IAppConfig appConfig, IDeviceInfo deviceInfo, IClientAppInfo clientAppInfo)
        {
            ViewModel = new RegisterViewModel();
            ViewModel.AppId = appConfig.AppName;
            ViewModel.DeviceId = deviceInfo.DeviceUniqueId;
            ViewModel.ClientType = "mobileapp";
            ViewModel.InstallationId = appConfig.InstallationId;
            RegisterCommand = new RelayCommand(Register);
        }

        public async void Register()
        {
            if (String.IsNullOrEmpty(ViewModel.FirstName))
            {
                await Popups.ShowAsync(ClientResources.Register_FirstName_Required);
                return;
            }

            if (String.IsNullOrEmpty(ViewModel.LastName))
            {
                await Popups.ShowAsync(ClientResources.Register_LastName_Required);
                return;
            }


            if (String.IsNullOrEmpty(ViewModel.Email))
            {
                await Popups.ShowAsync(ClientResources.Register_Email_Required);
                return;
            }


            var emailRegEx = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!emailRegEx.Match(ViewModel.Email).Success)
            {
                await Popups.ShowAsync(ClientResources.Register_Email_Invalid);
                return;
            }

            if (String.IsNullOrEmpty(ViewModel.Password))
            {
                await Popups.ShowAsync(ClientResources.Register_Password_Required);
                return;
            }


            var passwordRegEx = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
            if (!passwordRegEx.Match(ViewModel.Password).Success)
            {
                await Popups.ShowAsync(ClientResources.Register_Password_Requirements);
                return;
            }

            if (ViewModel.Password != ViewModel.ConfirmPassword)
            {
                await Popups.ShowAsync(ClientResources.Register_Password_Confirm_NoMatch);
                return;
            }

            await PerformNetworkOperation(async () =>
            {
                var result = await RestClient.PostAsync<AuthResponse>("/api/user/register", ViewModel);
                if (result.Successful)
                {                    
                    /* Make sure our Access Token is saved so the REST service can use it */
                    AuthManager.AccessToken = result.Result.AccessToken;
                    AuthManager.AccessTokenExpirationUTC = result.Result.AccessTokenExpiresUTC;
                    AuthManager.RefreshToken = result.Result.RefreshToken;
                    AuthManager.RefreshTokenExpirationUTC = result.Result.RefreshTokenExpiresUTC;
                    AuthManager.IsAuthenticated = true;

                    var user = await RestClient.GetAsync<UserInfo>("/api/user");
                    AuthManager.User = user.Model;
                    await AuthManager.PersistAsync();

                    Logger.AddKVPs(new KeyValuePair<string, string>("Email", AuthManager.User.Email));

                    await ViewModelNavigation.NavigateAsync<VerifyUserViewModel>();
                }
                else
                {
                    await ShowServerErrorMessage(result);
                }
            });


        }

        public RegisterViewModel ViewModel { get; private set; }


        public RelayCommand RegisterCommand { get; private set; }
    }
}
