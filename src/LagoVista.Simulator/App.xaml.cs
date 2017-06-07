using LagoVista.Client.Core;
using LagoVista.Client.Core.Auth;
using LagoVista.Client.Core.Models;
using LagoVista.Client.Core.Net;
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
            var serverInfo = new ServerInfo()
            {
                SSL = true,
                RootUrl = "dev-api.nuviot.com",
            };

            LagoVista.Client.Core.Startup.Init(serverInfo);
            LagoVista.XPlat.Core.Startup.Init(this);

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
