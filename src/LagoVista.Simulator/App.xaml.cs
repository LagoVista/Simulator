using LagoVista.Client.Core.Models;
using LagoVista.Core.IOC;
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

            MainPage = new LagoVista.Simulator.Views.SplashView();
        }

        protected override void OnStart()
        {
            var serverInfo = new ServerInfo()
            {
                RootUrl = "localhost",
                Port = 5001
            };

            var client = new HttpClient();
            client.BaseAddress = serverInfo.BaseAddress;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            SLWIOC.RegisterSingleton<ServerInfo>(serverInfo);
            SLWIOC.RegisterSingleton<HttpClient>(client);

            var navigation = new ViewModelNavigation(this);
            navigation.Add<ViewModels.Auth.LoginViewModel, Views.Auth.Login>();
            navigation.Add<ViewModels.MainViewModel, Views.MainView>();



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
