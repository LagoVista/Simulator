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
using System.Collections.Generic;
using System.Net;
using LagoVista.Client.Core.ViewModels.Orgs;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.Client.Core.ViewModels.Users
{
    public class VerifyUserViewModel : IoTAppViewModelBase
    {
        Timer _timer;
        

        public VerifyUserViewModel()
        {
            SendEmailConfirmationCommand = new RelayCommand(SendEmailConfirmation);
            SendSMSConfirmationCommand = new RelayCommand(SendSMSConfirmation, ValidPhoneNumber);
            ConfirmEnteredSMSCommand = new RelayCommand(ConfirmSMSCode, () => !String.IsNullOrEmpty(SMSCode));
            LogoutCommand = new RelayCommand(Logout);
        }

        public void SendEmailConfirmation()
        {
            PerformNetworkOperation(async () =>
            {
                var result = await RestClient.GetAsync("/api/verify/sendconfirmationemail", new CancellationTokenSource());
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

        public bool ValidPhoneNumber()
        {
            return !(String.IsNullOrEmpty(PhoneNumber));
        }

        public void ConfirmSMSCode()
        {
            PerformNetworkOperation(async () =>
            {
                var vm = new VerfiyPhoneNumber();
                vm.PhoneNumber = PhoneNumber;
                vm.SMSCode = SMSCode;
                var json = JsonConvert.SerializeObject(vm);
                var result = await RestClient.PostAsync("/api/verify/sms", json, new CancellationTokenSource());
                if (result.Success)
                {
                    if (result.ToInvokeResult().Successful)
                    {
                        var refreshResult = await RefreshUserFromServerAsync();
                        if (refreshResult.Successful)
                        {
                            Logger.AddCustomEvent(LagoVista.Core.PlatformSupport.LogLevel.Error, "VerifyUserViewModel_HandleURIActivation", "EmailConfirmed", new KeyValuePair<string, string>("userid", AuthManager.User.Id));
                            await Popups.ShowAsync(ClientResources.Verify_SMS_Confirmed);
                            if (AuthManager.User.EmailConfirmed && AuthManager.User.PhoneNumberConfirmed)
                            {
                                await ViewModelNavigation.NavigateAndCreateAsync<OrgEditorViewModel>();
                            }
                        }
                        else
                        {
                            await ShowServerErrorMessageAsync(result.ToInvokeResult());
                        }                        
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

        public override void HandleURIActivation(Uri uri, Dictionary<string, string> kvps)
        {          
            if(!kvps.ContainsKey("code"))
            {
                Logger.AddCustomEvent(LagoVista.Core.PlatformSupport.LogLevel.Error, "VerifyUserViewModel_HandleURIActivation", "Missing Code", new KeyValuePair<string, string>("queryString", uri.Query));
                return;
            }

            if (!kvps.ContainsKey("userid"))
            {
                Logger.AddCustomEvent(LagoVista.Core.PlatformSupport.LogLevel.Error, "VerifyUserViewModel_HandleURIActivation", "Missing User ID", new KeyValuePair<string, string>("queryString", uri.Query));
                return;
            }

            var code = kvps["code"];
            var userId = kvps["userid"];

            if(userId != AuthManager.User.Id)
            {
                Logger.AddCustomEvent(LagoVista.Core.PlatformSupport.LogLevel.Error, "VerifyUserViewModel_HandleURIActivation", "Link/User Id Mismatch", 
                    new KeyValuePair<string, string>("linkUser", userId),
                     new KeyValuePair<string, string>("currentUser", AuthManager.User.Id));
                return;
            }
            
            PerformNetworkOperation(async () =>
            {
                var vm = new ConfirmEmail();
                vm.ReceivedCode = code;
                var json = JsonConvert.SerializeObject(vm);
                var result = await RestClient.PostAsync("/api/verify/email", json, new CancellationTokenSource());
                if (result.Success)
                {
                    if (result.ToInvokeResult().Successful)
                    {
                        var refreshResult = await RefreshUserFromServerAsync();
                        if(refreshResult.Successful)
                        {
                            Logger.AddCustomEvent(LagoVista.Core.PlatformSupport.LogLevel.Error, "VerifyUserViewModel_HandleURIActivation", "EmailConfirmed", new KeyValuePair<string, string>("userid", userId));
                            await Popups.ShowAsync(ClientResources.Verify_Email_Confirmed);
                            if (AuthManager.User.EmailConfirmed && AuthManager.User.PhoneNumberConfirmed)
                            {
                                await ViewModelNavigation.NavigateAndCreateAsync<OrgEditorViewModel>();
                            }
                        }
                        else
                        {
                            await ShowServerErrorMessageAsync(result.ToInvokeResult());
                        }
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
                var vm = new VerfiyPhoneNumber();
                vm.PhoneNumber = PhoneNumber;
                var json = JsonConvert.SerializeObject(vm);
                var result = await RestClient.PostAsync("/api/verify/sendsmscode", json, new CancellationTokenSource());
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
