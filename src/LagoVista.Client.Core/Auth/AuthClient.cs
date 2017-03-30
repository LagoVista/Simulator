using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LagoVista.Core.Authentication.Interfaces;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.Interfaces;
using System.Net.Http;
using Newtonsoft.Json;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.Authentication.Models;

namespace LagoVista.Client.Core.Auth
{
    public class AuthClient 
    {
        HttpClient _client;
        public AuthClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<IAPIResponse<AuthResponse>> LoginAsync(IRemoteLoginModel loginInfo, CancellationTokenSource cancellationTokenSource = null)
        {
            var json =  JsonConvert.SerializeObject(loginInfo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/v1/auth", content);
            if (response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<APIResponse<AuthResponse>>(resultContent);
            }
            else
            {
                return APIResponse<AuthResponse>.FromFailedStatusCode(response.StatusCode);
            }
        }

        public Task<IAPIResponse> ResetPasswordAsync(string emailAddress, CancellationTokenSource cancellationTokenSource = null)
        {
            throw new NotImplementedException();
        }
    }
}
