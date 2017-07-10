using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.Resources;
using LagoVista.Core.Commanding;
using LagoVista.UserAdmin.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace LagoVista.Client.Core.ViewModels.Auth
{
    public class ChangePasswordViewModel : IoTAppViewModelBase
    {
        IRestClient _rawRestClient;

        public ChangePasswordViewModel(IRestClient rawRestClient)
        {
            _rawRestClient = rawRestClient;

            ChangePasswordCommand = new RelayCommand(ChangePassword);
            CancelCommand = new RelayCommand(() => ViewModelNavigation.GoBackAsync());
        }

        public async void ChangePassword()
        {
            if (String.IsNullOrEmpty(Model.OldPassword))
            {
                await Popups.ShowAsync(ClientResources.ChangePassword_OldPasswordRequired);
                return;
            }

            if (String.IsNullOrEmpty(Model.NewPassword))
            {
                await Popups.ShowAsync(ClientResources.ChangePassword_NewPasswordRequired);
                return;
            }

            var passwordRegEx = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
            if (!passwordRegEx.Match(Model.NewPassword).Success)
            {
                await Popups.ShowAsync(ClientResources.Password_Requirements);
                return;
            }

            if (String.IsNullOrEmpty(ConfirmPassword))
            {
                await Popups.ShowAsync(ClientResources.ChangePassword_ConfirmNewPassword);
                return;
            }

            if (ConfirmPassword != Model.NewPassword)
            {
                await Popups.ShowAsync(ClientResources.ChangePassword_NewConfirmMatch);
                return;
            }

            Model.UserId = AuthManager.User.Id;
            
            await PerformNetworkOperation(async () =>
            {
                var json = JsonConvert.SerializeObject(Model);
                var result = await _rawRestClient.PostAsync("/api/auth/changepassword", json, new System.Threading.CancellationTokenSource());
                if (result.Success)
                {
                    await Popups.ShowAsync(ClientResources.ChangePassword_Confirmed);
                    await base.ViewModelNavigation.GoBackAsync();
                }
                else
                {
                    await ShowServerErrorMessageAsync(result.ToInvokeResult());
                }
            });
        }

        private String _confirmPassword;
        public String ConfirmPassword
        {
            get { return _confirmPassword; }
            set { Set(ref _confirmPassword, value); }
        }

        public ChangePassword Model { get; set; }

        public RelayCommand ChangePasswordCommand { get; private set; }
        public RelayCommand CancelCommand { get; set; }

    }
}
