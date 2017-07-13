using LagoVista.Client.Core.Resources;
using LagoVista.Core.Commanding;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.ViewModels.Orgs
{
    public class AcceptInviteViewModel : AppViewModelBase
    {
        public AcceptInviteViewModel()
        {
            AcceptInviteCommnad = new RelayCommand(AcceptInvite);
            AcceptAndRegisterCommand = new RelayCommand(AcceptInvite);
            AcceptAndLoginCommand = new RelayCommand(AcceptInvite);
        }

        private async Task<InvokeResult> SendAcceptInvite()
        {
            return (await RestClient.GetAsync<InvokeResult>("/api/org/inviteuser/accept/{inviteid}")).Result;
        }

        public async void AcceptInvite()
        {
            if ((await PerformNetworkOperation(SendAcceptInvite)).Successful)
            {
                await Popups.ShowAsync(ClientResources.Accept_AcceptLoggedInSuccessful);
                await ViewModelNavigation.GoBackAsync();
            }
        }

        public void AcceptInviteAndLogin()
        {

        }

        public void AcceptInviteAndRegister()
        {

        }


        public RelayCommand AcceptInviteCommnad { get; private set; }

        public RelayCommand AcceptAndRegisterCommand { get; private set; }
        public RelayCommand AcceptAndLoginCommand { get; private set; }

        public bool IsLoggedIn
        {
            get { return AuthManager.IsAuthenticated; }
        }

        public bool IsNotLoggedIn
        {
            get { return !AuthManager.IsAuthenticated; }
        }
    }
}
