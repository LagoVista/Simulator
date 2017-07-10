﻿using LagoVista.Client.Core.Resources;
using LagoVista.Core.Commanding;
using LagoVista.Core.Validation;
using LagoVista.UserAdmin.Models.DTOs;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.ViewModels.Auth
{
    public class SendResetPasswordLinkViewModel : IoTAppViewModelBase
    {
        public SendResetPasswordLinkViewModel()
        {
            SendResetPasswordLinkCommand = new RelayCommand(SendResetPasswordLink);
            CancelCommand = new RelayCommand(() => ViewModelNavigation.GoBackAsync());
        }

        public async Task<InvokeResult> CallSendInvite()
        {
            var sendPwdResetLink = new SendResetPasswordLink();
            sendPwdResetLink.Email = EmailAddress;
            return (await RestClient.PostAsync("/api/auth/resetpassword/sendlink",sendPwdResetLink));
        }

        public async void SendResetPasswordLink()
        {
            if (String.IsNullOrEmpty(EmailAddress))
            {
                await Popups.ShowAsync(ClientResources.Register_Email_Required);
                return;
            }

            var emailRegEx = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!emailRegEx.Match(EmailAddress).Success)
            {
                await Popups.ShowAsync(ClientResources.Register_Email_Invalid);
                return;
            }

            if ((await PerformNetworkOperation(CallSendInvite)).Successful)
            {
                await ViewModelNavigation.GoBackAsync();
            }
        }

        private String _emailAddress;
        public String EmailAddress
        {
            get { return _emailAddress; }
            set { Set(ref _emailAddress, value); }
        }

        public RelayCommand SendResetPasswordLinkCommand { get; private set; }

        public RelayCommand CancelCommand { get; set; }
    }
}
