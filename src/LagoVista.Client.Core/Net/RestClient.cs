using LagoVista.Core.Models;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.Net
{


    public class RestClient<TModel>
    {
        HttpClient _httpClient;
        IAuthManager _authManager;
        ITokenManager _tokenManager;
        ILogger _logger;
        INetworkService _networkserice;

        public RestClient(HttpClient httpClient, IAuthManager authManager, ITokenManager tokenManager, ILogger logger, INetworkService networkService)
        {
            _networkserice = networkService;
            _logger = logger;
            _httpClient = httpClient;
            _authManager = authManager;
            _tokenManager = tokenManager;
        }

        public async Task<APIResponse> DeleteAsync(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) 
        {
            if (!await _tokenManager.ValidateTokenAsync(_authManager, cancellationTokenSource))
            {
                return APIResponse.CreateFailed(System.Net.HttpStatusCode.Unauthorized);
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authManager.AuthToken);

            try
            {
                var response = await _httpClient.DeleteAsync(path, cancellationTokenSource.Token);
                return (response.IsSuccessStatusCode) ? APIResponse.CreateOK() : APIResponse.CreateFailed(response.StatusCode);
            }
            catch (Exception ex)
            {
                return APIResponse.CreateForException(ex);
            }
        }

        public Task<APIResponse> GetAsync(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) 
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse> GetAsync<TRequestModel>(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) 
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse> PostAsync(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) 
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse<TResponse>> PostAsync<TResponse>(string path, TModel model, CancellationTokenSource cancellationTokenSource = null)  where TResponse : ModelBase
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse> PutAsync(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) 
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse<TResponse>> PutAsync<TResponse>(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TResponse : ModelBase
        {
            throw new NotImplementedException();
        }


    }
}
