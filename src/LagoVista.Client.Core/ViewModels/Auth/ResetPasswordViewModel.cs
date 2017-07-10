using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.Resources;
using LagoVista.Core.Commanding;
using LagoVista.UserAdmin.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Client.Core.ViewModels.Auth
{
    public class ResetPasswordViewModel : IoTAppViewModelBase
    {
        IRawRestClient _rawRestClient;

        public ResetPasswordViewModel(IRawRestClient rawRestClient)
        {
            _rawRestClient = rawRestClient;

            Model = new ResetPassword();

            ResetPasswordCommand = new RelayCommand(ResetPassword);
            CancelCommand = new RelayCommand(() => ViewModelNavigation.GoBackAsync());
        }

        public void ResetPassword()
        {
            PerformNetworkOperation(async () =>
            {
                var json = JsonConvert.SerializeObject(Model);
                var result = await _rawRestClient.PostAsync("/api/auth/resetpassword", json, new System.Threading.CancellationTokenSource());
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

        public ResetPassword Model
        {
            get; set;
        }


        public RelayCommand ResetPasswordCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
    }
}
