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
using LagoVista.Simulator.Core.ViewModels;
using LagoVista.Simulator.Core.ViewModels.Auth;
using LagoVista.Simulator.Core.ViewModels.Messages;
using LagoVista.Simulator.Core.ViewModels.Simulator;
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

            foreach (var module in Plugin.Iconize.Iconize.Modules)
            {
                var name = module.FontName;
            }

           InitServices();
        }

        private void InitServices()
        {
            var serverInfo = new ServerInfo()
            {
                RootUrl = "localhost",
                Port = 5001
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
