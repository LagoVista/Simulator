using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LagoVista.Core.Authentication.Interfaces;
using System.Net.Http;
using Newtonsoft.Json;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.Authentication.Models;
using LagoVista.Core.IOC;
using Newtonsoft.Json.Serialization;

namespace LagoVista.Client.Core.Auth
{
    public class AuthClient : IAuthClient
    {
        public AuthClient()
        {
            
        }

        public async Task<APIResponse<AuthResponse>> LoginAsync(AuthRequest loginInfo, CancellationTokenSource cancellationTokenSource = null)
        {
            var client = SLWIOC.Get<HttpClient>();

            var json =  JsonConvert.SerializeObject(loginInfo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync("/api/v1/auth", content);
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
            catch(Exception ex)
            {
                return APIResponse<AuthResponse>.FromException(ex);
            }
        }

        public Task<APIResponse> ResetPasswordAsync(string emailAddress, CancellationTokenSource cancellationTokenSource = null)
        {
            throw new NotImplementedException();
        }
    }
}
