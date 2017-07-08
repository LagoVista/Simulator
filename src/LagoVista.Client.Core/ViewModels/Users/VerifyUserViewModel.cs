using LagoVista.Client.Core.Net;
using LagoVista.Core.Commanding;
using LagoVista.Core.Models;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.UserAdmin.ViewModels.VerifyIdentity;
using System;
using System.Threading;
using LagoVista.Core;
using Newtonsoft.Json;
using LagoVista.Client.Core.ViewModels.Auth;
using LagoVista.UserAdmin.Models.DTOs;
using LagoVista.Client.Core.Resources;

namespace LagoVista.Client.Core.ViewModels.Users
{
    public class VerifyUserViewModel : IoTAppViewModelBase
    {
        Timer _timer;
        IRawRestClient _restClient;

        public VerifyUserViewModel(IRawRestClient restClient)
        {
            SendEmailConfirmationCommand = new RelayCommand(SendEmailConfirmation);
            SendSMSConfirmationCommand = new RelayCommand(SendSMSConfirmation, ValidPhoneNumber);
            ConfirmEnteredSMSCommand = new RelayCommand(ConfirmSMSCode, () => !String.IsNullOrEmpty(SMSCode));
            LogoutCommand = new RelayCommand(Logout);
            _restClient = restClient;
        }

        public void SendEmailConfirmation()
        {
            PerformNetworkOperation(async () =>
            {
                var result = await _restClient.GetAsync("/api/verify/sendconfirmationemail", new CancellationTokenSource());
                if (result.Success)
                {
                    if (result.ToInvokeResult().Successful)
                    {
                        await Popups.ShowAsync(ClientResources.Verify_EmailSent);
                    }
                    else
                    {
                        await ShowServerErrorMessageAsync(result.ToInvokeResult());
                    }
                }
                else
                {
                    await Popups.ShowAsync(result.ErrorMessage);
                }
            });
        }

        public async void Logout()
        {
            await AuthManager.LogoutAsync();
            await ViewModelNavigation.SetAsNewRootAsync<LoginViewModel>();
        }

        public bool ValidPhoneNumber()
        {
            return !(String.IsNullOrEmpty(PhoneNumber));
        }

        public void ConfirmSMSCode()
        {
            PerformNetworkOperation(async () =>
            {
                var vm = new VerfiyPhoneNumberDTO();
                vm.PhoneNumber = PhoneNumber;
                vm.SMSCode = SMSCode;
                var json = JsonConvert.SerializeObject(vm);
                var result = await _restClient.PostAsync("/api/verify/sms", json, new CancellationTokenSource());
                if (result.Success)
                {
                    if (result.ToInvokeResult().Successful)
                    {
                        await Popups.ShowAsync(ClientResources.Verify_SMS_Confirmed);
                    }
                    else
                    {
                        await ShowServerErrorMessageAsync(result.ToInvokeResult());
                    }
                }
                else
                {
                    await Popups.ShowAsync(result.ErrorMessage);
                }
            });
        }

        public void SendSMSConfirmation()
        {
            PerformNetworkOperation(async () =>
            {
                var vm = new VerfiyPhoneNumberDTO();
                vm.PhoneNumber = PhoneNumber;
                var json = JsonConvert.SerializeObject(vm);
                var result = await _restClient.PostAsync("/api/verify/sendsmscode", json, new CancellationTokenSource());
                if (result.Success)
                {
                    if (result.ToInvokeResult().Successful)
                    {
                        await Popups.ShowAsync(ClientResources.Verify_SMSSent);
                    }
                    else
                    {
                        await ShowServerErrorMessageAsync(result.ToInvokeResult());
                    }
                }
                else
                {
                    await Popups.ShowAsync(result.ErrorMessage);
                }
            });
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                Set(ref _phoneNumber, value);
                SendSMSConfirmationCommand.RaiseCanExecuteChanged();
            }
        }

        private string _smsCode;
        public string SMSCode
        {
            get { return _smsCode; }
            set
            {
                Set(ref _smsCode, value);
                ConfirmEnteredSMSCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _confirmEmailStepVisible = false;
        public bool ConfirmEmailStepVisible
        {
            get { return _confirmEmailStepVisible; }
            set { Set(ref _confirmEmailStepVisible, value); }
        }


        public RelayCommand SendEmailConfirmationCommand { get; private set; }
        public RelayCommand SendSMSConfirmationCommand { get; private set; }
        public RelayCommand ConfirmEnteredSMSCommand { get; private set; }
        public RelayCommand LogoutCommand { get; private set; }
    }
}
