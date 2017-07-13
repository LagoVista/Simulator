using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.Resources;
using LagoVista.Core.Commanding;
using LagoVista.Core.Validation;
using LagoVista.UserAdmin.Models.DTOs;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.ViewModels.Auth
{
    public class ChangePasswordViewModel : AppViewModelBase
    {
        public ChangePasswordViewModel(IRestClient rawRestClient)
        {
            ChangePasswordCommand = new RelayCommand(ChangePassword);
            CancelCommand = new RelayCommand(() => ViewModelNavigation.GoBackAsync());
            Model = new UserAdmin.Models.DTOs.ChangePassword();
        }

        public async Task<InvokeResult> SendResetPassword()
        {
            return await RestClient.PostAsync("/api/auth/changepassword", Model, new System.Threading.CancellationTokenSource());
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
            
            if((await PerformNetworkOperation(SendResetPassword)).Successful)
            {
                await Popups.ShowAsync(ClientResources.ChangePassword_Success);
                await ViewModelNavigation.GoBackAsync();
            }
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
