using LagoVista.Client.Core.Net;
using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.ViewModels;
using System.Net.Http;
using System.Reflection;
using System.Linq;
using System;
using LagoVista.Core.Models;
using LagoVista.Core.Commanding;
using System.Threading.Tasks;
using System.Collections.Generic;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Client.Core.Resources;
using LagoVista.Client.Core.ViewModels;

namespace LagoVista.Simulator.Core.ViewModels
{

    public class SimulatorViewModelBase : XPlatViewModel
    {
        protected HttpClient HttpClient { get { return SLWIOC.Get<HttpClient>(); } }
        protected IAuthManager AuthManager { get { return SLWIOC.Get<IAuthManager>(); } }
        protected ITokenManager TokenManager { get { return SLWIOC.Get<ITokenManager>(); } }
        protected INetworkService NetworkService { get { return SLWIOC.Get<INetworkService>(); } }
    }

    public class SimulatorViewModelBase<TModel, TSummaryModel> : SimulatorViewModelBase where TModel : new() where TSummaryModel : class
    {
        IRestClient<TModel, TSummaryModel> _restClient;

        public SimulatorViewModelBase()
        {
            _restClient = new RestClient<TModel, TSummaryModel>(HttpClient, AuthManager, TokenManager, Logger, NetworkService);

        }


        public IRestClient<TModel, TSummaryModel> RestClient { get { return _restClient; } }

        TModel _model;
        public TModel Model
        {
            get { return _model; }
            set { Set(ref _model, value); }
        }

        
    }

    public class SimulatorViewModelBase<TModel> : SimulatorViewModelBase where TModel : new()
    {
        IRestClient<TModel> _restClient;

        public SimulatorViewModelBase()
        {
            _restClient = new RestClient<TModel>(HttpClient, AuthManager, TokenManager, Logger, NetworkService);
        }


        public IRestClient<TModel> RestClient { get { return _restClient; } }


        TModel _model;
        public TModel Model
        {
            get { return _model; }
            set { Set(ref _model, value); }
        }

        IDictionary<string, FormField> _view;
        public IDictionary<string, FormField> View
        {
            get { return _view; }
            set { Set(ref _view, value); }
        }        

        public bool ViewToModel(EditFormAdapter form, TModel model)
        {
            var modelProperties = typeof(TModel).GetTypeInfo().DeclaredProperties;

            if (!form.Validate())
            {
                return false;
            }

            foreach (var formItem in FormAdapter.FormItems)
            {
                var prop = modelProperties.Where(prp => prp.Name.ToLower() == formItem.Name.ToLower()).FirstOrDefault();
                switch (formItem.FieldType)
                {
                    case FormField.FieldType_CheckBox:
                        if (bool.TryParse(formItem.Value, out bool result))
                        {
                            prop.SetValue(model, result);
                        }
                        break;
                    case FormField.FieldType_Picker:
                        if (String.IsNullOrEmpty(formItem.Value))
                        {
                            prop.SetValue(model, null);
                        }
                        else
                        {
                            var eh = Activator.CreateInstance(prop.PropertyType) as EntityHeader;
                            eh.Id = formItem.Value;
                            eh.Text = formItem.Options.Where(opt => opt.Key == formItem.Value).First().Label;

                            prop.SetValue(model, eh);
                        }
                        break;
                    case FormField.FieldType_Integer:
                        if (!String.IsNullOrEmpty(formItem.Value))
                        {
                            if (int.TryParse(formItem.Value, out int intValue))
                            {
                                prop.SetValue(model, intValue);
                            }
                        }

                        break;
                    case FormField.FieldType_Decimal:
                        if (!String.IsNullOrEmpty(formItem.Value))
                        {
                            if (double.TryParse(formItem.Value, out double intValue))
                            {
                                prop.SetValue(model, intValue);
                            }
                        }

                        break;
                    case FormField.FieldType_MultilineText:
                    case FormField.FieldType_Text:
                    case FormField.FieldType_Key:
                        prop.SetValue(model, formItem.Value);
                        break;
                }
            }

            return true;
        }

        public async Task<bool> PerformNetworkOperation(Func<Task<bool>> action)
        {
            if(!IsNetworkConnected)
            {
                await Popups.ShowAsync(ClientResources.Common_NoConnection);
                return false;
            }

            IsBusy = true;

            try
            {
                await action();
            }
            catch (Exception ex)
            {
                await Popups.ShowAsync(ClientResources.Common_ErrorCommunicatingWithServer + "\r\n\r\n" + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
            return true;
        }

        public void ModelToView(TModel model, EditFormAdapter form)
        {
            var modelProperties = typeof(TModel).GetTypeInfo().DeclaredProperties;

            foreach (var formItem in form.FormItems)
            {
                var prop = modelProperties.Where(prp => prp.Name.ToLower() == formItem.Name.ToLower()).FirstOrDefault();
                var value = prop.GetValue(model);

                switch (formItem.FieldType)
                {
                    case FormField.FieldType_Picker:
                        if (value != null)
                        {
                            var entityHeader = value as EntityHeader;
                            formItem.Value = entityHeader.Id;
                        }
                        break;
                    case FormField.FieldType_CheckBox:
                    case FormField.FieldType_Integer:
                    case FormField.FieldType_Decimal:
                    case FormField.FieldType_MultilineText:
                    case FormField.FieldType_Text:
                    case FormField.FieldType_Key:
                        if (value != null)
                        {
                            formItem.Value = value.ToString();
                        }
                        break;
                }
            }
        }


        public bool IsEdit
        {
            get { return LaunchArgs != null && LaunchArgs.LaunchType == LaunchTypes.Edit; }
        }

        public bool IsCreate
        {
            get { return LaunchArgs != null && LaunchArgs.LaunchType == LaunchTypes.Create; }
        }


        EditFormAdapter _form;
        public EditFormAdapter FormAdapter
        {
            get { return _form; }
            set
            {
                _form = value;
                RaisePropertyChanged();
            }
        }

        public override Task ReloadedAsync()
        {
            if (FormAdapter != null)
            {
                FormAdapter.Refresh();
            }

            return base.ReloadedAsync();
        }
    }
}
