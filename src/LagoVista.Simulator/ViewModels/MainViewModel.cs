using LagoVista.Core.Authentication.Models;
using LagoVista.Core.Commanding;
using LagoVista.Core.IOC;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.ViewModels;
using LagoVista.Simulator.ViewModels.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            CallServiceCommand = new RelayCommand(CallService);
        }


        public async void CallService()
        {
            var client = SLWIOC.Get<HttpClient>();
            var result = await Storage.GetKVPAsync<AuthResponse>("AUTH");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",result.AuthToken);
            var response = await client.GetAsync("/api/v1/user");
            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<APIResponse<AuthResponse>>(resultContent);
            }
            else
            {
                var data = APIResponse<AuthResponse>.FromFailedStatusCode(response.StatusCode);
            }
        }

        public async void Logout()
        {
            await Storage.ClearKVP("AUTH");
            ViewModelNavigation.Navigate<LoginViewModel>();
        }

        public RelayCommand CallServiceCommand { get; private set; }
    }
}
