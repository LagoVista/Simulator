#define ENV_LOCAL
//#define ENV_DEV
//#define ENV_PROD

using LagoVista.Client.Core;
using LagoVista.Client.Core.Auth;
using LagoVista.Client.Core.Models;
using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Client.Core.ViewModels.Users;
using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using LagoVista.Core.ViewModels;
using LagoVista.Simulator.Core.ViewModels;
using LagoVista.Simulator.Core.ViewModels.Auth;
using LagoVista.Simulator.Core.ViewModels.Messages;
using LagoVista.Simulator.Core.ViewModels.Simulator;
using LagoVista.XPlat.Core.Services;
using System.Reflection;

using Xamarin.Forms;


namespace LagoVista.Simulator
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();


           InitServices();
        }

        private void InitServices()
        {
#if ENV_PROD
            var serverInfo = new ServerInfo()
            {
                SSL = true,
                RootUrl = "api.nuviot.com",
            };
#elif ENV_DEV
            var serverInfo = new ServerInfo()
            {
                SSL = true,
                RootUrl = "dev-api.nuviot.com",
            };
#elif ENV_LOCAL
            var serverInfo = new ServerInfo()
            {
                SSL = false,
                RootUrl = "localhost:5001",
            };
#endif

            LagoVista.XPlat.Core.Startup.Init(this);
            LagoVista.Client.Core.Startup.Init(serverInfo);

            var clientAppInfo = new ClientAppInfo()
            {
                MainViewModel = typeof(MainViewModel)
            };

            SLWIOC.RegisterSingleton<IClientAppInfo>(clientAppInfo);

            SLWIOC.RegisterSingleton<IAppConfig>(new AppConfig());
            var navigation = new ViewModelNavigation(this);
            navigation.Add<SplashViewModel, Views.SplashView>();
            navigation.Add<LoginViewModel, Views.Auth.Login>();
            navigation.Add<MainViewModel, Views.MainView>();
            navigation.Add<SimulatorViewModel, Views.Simulator.SimulatorView>();
            navigation.Add<SimulatorEditorViewModel, Views.Simulator.SimulatorEditorView>();
            navigation.Add<MessageEditorViewModel, Views.Messages.MessageEditorView>();
            navigation.Add<SendMessageViewModel, Views.Messages.SendMessageView>();
            navigation.Add<MessageHeaderViewModel, Views.Messages.MessageHeaderView>();
            navigation.Add<DynamicAttributeViewModel, Views.Messages.DynamicAttributeView>();
            navigation.Add<RegisterUserViewModel, Views.Users.RegisterView>();
            navigation.Add<VerifyUserViewModel, Views.Users.VerifyUserView>();

            navigation.Start<SplashViewModel>();

            SLWIOC.RegisterSingleton<IViewModelNavigation>(navigation);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
