using LagoVista.Client.Core.Net;
using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.ViewModels;
using System.Net.Http;

namespace LagoVista.Simulator.ViewModels
{

    public class SimulatorViewModelBase: ViewModelBase
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
    }


    public class SimulatorViewModelBase<TModel> : SimulatorViewModelBase where TModel : new() 
    {
        IRestClient<TModel> _restClient;

        public SimulatorViewModelBase()
        {
            _restClient = new RestClient<TModel>(HttpClient, AuthManager, TokenManager, Logger, NetworkService);
        }

        public IRestClient<TModel> RestClient { get { return _restClient; } }
    }
}
