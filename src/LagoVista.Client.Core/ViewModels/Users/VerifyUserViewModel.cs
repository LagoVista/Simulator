using LagoVista.Client.Core.Net;
using LagoVista.Core.Commanding;
using LagoVista.Core.Models;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.UserAdmin.ViewModels.VerifyIdentity;
using System;
using System.Threading;
using LagoVista.Core;
using Newtonsoft.Json;

namespace LagoVista.Client.Core.ViewModels.Users
{
    public class VerifyUserViewModel : IoTAppViewModelBase
    {
        Timer _timer;
        IRawRestClient _restClient;

        public VerifyUserViewModel(IRawRestClient restClient)
        {
            SendEmailConfirmationCommand = new RelayCommand(SendEmailConfirmation);
            SendSMSConfirmationCommand = new RelayCommand(SendSMSConfirmation);
            ConfirmEnteredSMSCommand = new RelayCommand(ConfirmSMSCode, () => !String.IsNullOrEmpty(SMSCode));
            _restClient = restClient;
        }

        public async void SendEmailConfirmation()
        {
            var result = await _restClient.GetAsync("/api/verify/sendconfirmationemail", new CancellationTokenSource());

        }

        public async void ConfirmSMSCode()
        {
            var vm = new VerifyPhoneNumberViewModel();
            vm.PhoneNumber = PhoneNumber;
            vm.Code = SMSCode;
            var json = JsonConvert.SerializeObject(vm);
            var result = await _restClient.PostAsync("/api/verify/sms", json, new CancellationTokenSource());
        }

        public async void SendSMSConfirmation()
        {
            var vm = new VerifyPhoneNumberViewModel();
            vm.PhoneNumber = PhoneNumber;
            var json = JsonConvert.SerializeObject(vm);
            var result = await _restClient.PostAsync("/api/verify/sendsmscode", json, new CancellationTokenSource());
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                Set(ref _phoneNumber, value);
                ConfirmEnteredSMSCommand.RaiseCanExecuteChanged();
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
    }
}
