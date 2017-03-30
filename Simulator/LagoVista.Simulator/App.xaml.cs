using LagoVista.Client.Core.Models;
using LagoVista.Core.IOC;
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

            MainPage = new LagoVista.Simulator.MainPage();
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
