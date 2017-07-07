using LagoVista.Client.Core.Resources;
using LagoVista.Core.Authentication.Models;
using LagoVista.Core.Commanding;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.Core.ViewModels;
using LagoVista.UserAdmin.Models.DTOs;
using LagoVista.UserAdmin.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.ViewModels.Users
{
    public class RegisterUserViewModel : IoTAppViewModelBase<RegisterUserDTO>
    {
        public RegisterUserViewModel(IAppConfig appConfig, IDeviceInfo deviceInfo, IClientAppInfo clientAppInfo)
        {
            RegisterModel = new RegisterUserDTO();
            RegisterModel.AppId = appConfig.AppName;
            RegisterModel.DeviceId = deviceInfo.DeviceUniqueId;
            RegisterModel.ClientType = "mobileapp";
            RegisterCommand = new RelayCommand(Register);
        }

        public async void Register()
        {
            if (String.IsNullOrEmpty(RegisterModel.FirstName))
            {
                await Popups.ShowAsync(ClientResources.Register_FirstName_Required);
                return;
            }

            if (String.IsNullOrEmpty(RegisterModel.LastName))
            {
                await Popups.ShowAsync(ClientResources.Register_LastName_Required);
                return;
            }


            if (String.IsNullOrEmpty(RegisterModel.Email))
            {
                await Popups.ShowAsync(ClientResources.Register_Email_Required);
                return;
            }


            var emailRegEx = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!emailRegEx.Match(RegisterModel.Email).Success)
            {
                await Popups.ShowAsync(ClientResources.Register_Email_Invalid);
                return;
            }

            if (String.IsNullOrEmpty(RegisterModel.Password))
            {
                await Popups.ShowAsync(ClientResources.Register_Password_Required);
                return;
            }


            var passwordRegEx = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
            if (!passwordRegEx.Match(RegisterModel.Password).Success)
            {
                await Popups.ShowAsync(ClientResources.Register_Password_Requirements);
                return;
            }

            if (RegisterModel.Password != ConfirmPassword)
            {
                await Popups.ShowAsync(ClientResources.Register_Password_Confirm_NoMatch);
                return;
            }

            await PerformNetworkOperation(async () =>
            {
                var result = await RestClient.PostAsync<InvokeResult<AuthResponse>>("/api/user/register", RegisterModel);
                // MAJOR BROKEN :(
                if (result.Successful && result.Result != null && result.Result.Successful)
                {
                    var authResult = result.Result.Result;
                    /* Make sure our Access Token is saved so the REST service can use it */
                    AuthManager.AccessToken = authResult.AccessToken;
                    AuthManager.AccessTokenExpirationUTC = authResult.AccessTokenExpiresUTC;
                    AuthManager.RefreshToken = authResult.RefreshToken;
                    AuthManager.RefreshTokenExpirationUTC = authResult.RefreshTokenExpiresUTC;
                    AuthManager.AppInstanceId = authResult.AppInstanceId;
                    AuthManager.IsAuthenticated = true;

                    var user = await RestClient.GetAsync<UserInfo>("/api/user");
                    AuthManager.User = user.Model;
                    await AuthManager.PersistAsync();

                    Logger.AddKVPs(new KeyValuePair<string, string>("Email", AuthManager.User.Email));

                    await ViewModelNavigation.NavigateAsync<VerifyUserViewModel>();
                }
                else
                {
                    if (!result.Successful)
                    {
                        await ShowServerErrorMessage(result);
                    }
                    else if (result.Result != null)
                    {
                        await ShowServerErrorMessage(result.Result);
                    }
                }
            });
        }

        public RegisterUserDTO RegisterModel { get; private set; }


        private String _confirmPassword;
        public String ConfirmPassword
        {
            get { return _confirmPassword; }
            set { Set(ref _confirmPassword, value); }
        }

        public RelayCommand RegisterCommand { get; private set; }
    }
}
