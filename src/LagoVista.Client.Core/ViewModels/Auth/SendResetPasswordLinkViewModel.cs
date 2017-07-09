using LagoVista.Client.Core.Net;
using LagoVista.Core.Commanding;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Client.Core.ViewModels.Auth
{
    public class SendResetPasswordLinkViewModel : IoTAppViewModelBase
    {
        IRawRestClient _rawRestClient;

        public SendResetPasswordLinkViewModel(IRawRestClient rawRestClient)
        {
            _rawRestClient = rawRestClient;

            SendResetPasswordLinkCommand = new RelayCommand(SendResetPasswordLink);
            CancelCommand = new RelayCommand(() => ViewModelNavigation.GoBackAsync());
        }

        public void SendResetPasswordLink()
        {

        }

        public RelayCommand SendResetPasswordLinkCommand { get; private set; }

        public RelayCommand CancelCommand { get; set; }
    }
}
