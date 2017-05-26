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
using LagoVista.Simulator.Models;
using LagoVista.Core.Commanding;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels
{

    public class SimulatorViewModelBase : ViewModelBase
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
            SaveCommand = new RelayCommand(SaveAsync, CanSave);
        }

        public virtual void SaveAsync()
        {

        }

        public virtual bool CanSave()
        {
            return true;
        }

        public RelayCommand SaveCommand { get; private set; }

        public IRestClient<TModel> RestClient { get { return _restClient; } }


        TModel _model;
        public TModel Model
        {
            get { return _model; }
            set { Set(ref _model, value); }
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
                    case Controls.FormViewer.CHECKBOX:
                        if (bool.TryParse(formItem.Value, out bool result))
                        {
                            prop.SetValue(model, result);
                        }
                        break;
                    case Controls.FormViewer.PICKER:
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
                    case Controls.FormViewer.INTEGER:
                        if (!String.IsNullOrEmpty(formItem.Value))
                        {
                            if (int.TryParse(formItem.Value, out int intValue))
                            {
                                prop.SetValue(model, intValue);
                            }
                        }

                        break;
                    case Controls.FormViewer.DECIMAL:
                        if (!String.IsNullOrEmpty(formItem.Value))
                        {
                            if (double.TryParse(formItem.Value, out double intValue))
                            {
                                prop.SetValue(model, intValue);
                            }
                        }

                        break;
                    case Controls.FormViewer.MULTILINE:
                    case Controls.FormViewer.TEXT:
                    case Controls.FormViewer.KEY:
                        prop.SetValue(model, formItem.Value);
                        break;
                }
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
                    case Controls.FormViewer.PICKER:
                        if (value != null)
                        {
                            var entityHeader = value as EntityHeader;
                            formItem.Value = entityHeader.Id;
                        }
                        break;
                    case Controls.FormViewer.CHECKBOX:
                    case Controls.FormViewer.INTEGER:
                    case Controls.FormViewer.DECIMAL:
                    case Controls.FormViewer.MULTILINE:
                    case Controls.FormViewer.TEXT:
                    case Controls.FormViewer.KEY:
                        if (value != null)
                        {
                            formItem.Value = value.ToString();
                        }
                        break;
                }
            }
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
