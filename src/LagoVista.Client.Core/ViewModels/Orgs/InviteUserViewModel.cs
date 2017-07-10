using LagoVista.Client.Core.Net;
using LagoVista.Core.Commanding;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Client.Core.ViewModels.Orgs
{
    public class InviteUserViewModel : IoTAppViewModelBase
    {
        IRestClient _rawRestClient;

        public InviteUserViewModel(IRestClient rawRestClient)
        {
            _rawRestClient = rawRestClient;
            InviteUserCommand = new RelayCommand(InviteUser);
            CancelCommand = new RelayCommand(() => ViewModelNavigation.GoBackAsync());
        }

        public void InviteUser()
        {

        }

        public String InviteUserHelpMessage
        {
            get;set;
        }

        public RelayCommand InviteUserCommand { get; private set; }
        public RelayCommand CancelCommand { get; set; }
    }
}
