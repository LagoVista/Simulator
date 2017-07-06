using LagoVista.Client.Core.Net;
using LagoVista.Core.Commanding;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Client.Core.ViewModels.Users
{
    public class VerifyUserViewModel : IoTAppViewModelBase
    {
        public VerifyUserViewModel(IRestClient restClient)
        {
            SendEmailConfirmationCommand = new RelayCommand(SendEmailConfirmation);
            SendSMSConfirmationCommand = new RelayCommand(SendSMSConfirmation);
        }

        public void SendEmailConfirmation()
        {
            var client = new RestClient<UserInfo>(HttpClient, AuthManager, TokenManager, Logger, NetworkService);
        }

        public void SendSMSConfirmation()
        {

        }


        public RelayCommand SendEmailConfirmationCommand { get; private set; }
        public RelayCommand SendSMSConfirmationCommand { get; private set; }
    }
}
