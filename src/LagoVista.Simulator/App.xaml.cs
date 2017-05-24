using LagoVista.Client.Core.Auth;
using LagoVista.Client.Core.Models;
using LagoVista.Client.Core.Net;
using LagoVista.Core.Authentication.Interfaces;
using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Managers;
using LagoVista.IoT.Simulator.Admin.Repos;
using LagoVista.XPlat.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

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
                RootUrl = "localhost",
                Port = 5000
            };

            SLWIOC.RegisterSingleton<ServerInfo>(serverInfo);
            SLWIOC.RegisterSingleton<IAuthManager, AuthManager>();
            SLWIOC.RegisterSingleton<ITokenManager, TokenManager>();
            

            var client = new HttpClient();
            client.BaseAddress = serverInfo.BaseAddress;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            SLWIOC.RegisterSingleton<HttpClient>(client);
            
            var navigation = new ViewModelNavigation(this);
            navigation.Add<ViewModels.SplashViewModel, Views.SplashView>();
            navigation.Add<ViewModels.Auth.LoginViewModel, Views.Auth.Login>();
            navigation.Add<ViewModels.MainViewModel, Views.MainView>();
            navigation.Add<ViewModels.Simulator.SimulatorViewModel, Views.Simulator.SimulatorView>();
            navigation.Add<ViewModels.Simulator.SimulatorEditorViewModel, Views.Simulator.SimulatorEditorView>();
            navigation.Add<ViewModels.Messages.MessageEditorViewModel, Views.Messages.MessageEditorView>();
            navigation.Add<ViewModels.Messages.SendMessageViewModel, Views.Messages.SendMessageView>();

            navigation.Start<ViewModels.SplashViewModel>();

            SLWIOC.RegisterSingleton<IViewModelNavigation>(navigation);
            SLWIOC.RegisterSingleton<IAuthClient>(new AuthClient());
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
