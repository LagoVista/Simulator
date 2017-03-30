using LagoVista.Client.Core.Auth;
using LagoVista.Core.Authentication.Models;
using LagoVista.Core.IOC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LagoVista.Simulator
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var client = new AuthClient(SLWIOC.Get<HttpClient>());
            var auth = new AuthRequest()
            {
                UserName = EmailAddress.Text,
                Password = Password.Text
            };

            var result = await client.LoginAsync(auth);

            Debug.WriteLine(result.Success);
            if (result.Success)
            {
                Debug.WriteLine(result.Result.AuthToken);
            }
        }
    }
}
