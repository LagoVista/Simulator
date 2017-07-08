using LagoVista.Core.Commanding;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Client.Core.ViewModels.Auth
{
    public class ResetPasswordViewModel : IoTAppViewModelBase
    {

        private String _emailAddress;

        public String EmailAddress
        {
            get { return _emailAddress; }
            set { Set(ref _emailAddress, value); }
        }

        public RelayCommand SendReetPasswordLink { get; set; }

    }
}
