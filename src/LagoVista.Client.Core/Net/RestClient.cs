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
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using Newtonsoft.Json;
using LagoVista.Core.Interfaces;
using Newtonsoft.Json.Serialization;

namespace LagoVista.Client.Core.Net
{


    public class RestClient<TModel> : IRestClient<TModel> where TModel : new()
    {
        HttpClient _httpClient;
        IAuthManager _authManager;
        ITokenManager _tokenManager;
        ILogger _logger;
        INetworkService _networkService;

        public RestClient(HttpClient httpClient, IAuthManager authManager, ITokenManager tokenManager, ILogger logger, INetworkService networkService)
        {
            _networkService = networkService;
            _logger = logger;
            _httpClient = httpClient;
            _authManager = authManager;
            _tokenManager = tokenManager;
        }

        public async Task<InvokeResult> AddAsync(string path, TModel model, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource(15 * 1000); /* Abort after 15 seconds */
            }

            if (!await _tokenManager.ValidateTokenAsync(_authManager, cancellationTokenSource))
            {
                var errs = new InvokeResult();
                errs.Errors.Add(new ErrorMessage("could Not Add Item: " + System.Net.HttpStatusCode.Unauthorized));
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authManager.AccessToken);

            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(path, content, cancellationTokenSource.Token);
                if (response.IsSuccessStatusCode)
                {
                    var responseJSON = await response.Content.ReadAsStringAsync();
                    var serializerSettings = new JsonSerializerSettings();
                    serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                    return JsonConvert.DeserializeObject<InvokeResult>(responseJSON, serializerSettings);
                }
                else
                {
                    var result = new InvokeResult();
                    result.Errors.Add(new ErrorMessage("failure code response"));
                    return result;
                }
            }
            catch (Exception)
            {
                var result = new InvokeResult();
                result.Errors.Add(new ErrorMessage("failure code response"));
                return result;
            }
        }

        public async Task<InvokeResult<TResponseModel>> PostAsync<TResponseModel>(string path, TModel model, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource(15 * 1000); /* Abort after 15 seconds */
            }

            if (!await _tokenManager.ValidateTokenAsync(_authManager, cancellationTokenSource))
            {
                var errs = new InvokeResult();
                errs.Errors.Add(new ErrorMessage("could Not Add Item: " + System.Net.HttpStatusCode.Unauthorized));
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authManager.AccessToken);

            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(path, content, cancellationTokenSource.Token);
                if (response.IsSuccessStatusCode)
                {
                    var responseJSON = await response.Content.ReadAsStringAsync();
                    var serializerSettings = new JsonSerializerSettings();
                    serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                    return JsonConvert.DeserializeObject<InvokeResult<TResponseModel>>(responseJSON, serializerSettings);
                }
                else
                {
                    var result = new InvokeResult<TResponseModel>();
                    result.Errors.Add(new ErrorMessage("failure code response"));
                    return result;
                }
            }
            catch (Exception)
            {
                var result = new InvokeResult<TResponseModel>();
                result.Errors.Add(new ErrorMessage("failure code response"));
                return result;
            }
        }


        public Task<DetailResponse<TModel>> CreateNewAsync(string path, CancellationTokenSource cancellationTokenSource = null)
        {
            return GetAsync(path, cancellationTokenSource);
        }

        public Task<InvokeResult> DeleteAsync(string path, TModel model, CancellationTokenSource cancellationTokenSource = null)
        {
            throw new NotImplementedException();
        }

        public async Task<DetailResponse<TModel>> GetAsync(string path, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource(15 * 1000); /* Abort after 15 seconds */
            }

            if (!await _tokenManager.ValidateTokenAsync(_authManager, cancellationTokenSource))
            {
                var errs = new InvokeResult();
                errs.Errors.Add(new ErrorMessage("could Not Add Item: " + System.Net.HttpStatusCode.Unauthorized));
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authManager.AccessToken);

            var response = await _httpClient.GetAsync(path, cancellationTokenSource.Token);
            if (response.IsSuccessStatusCode)
            {
                var responseJSON = await response.Content.ReadAsStringAsync();
                var serializerSettings = new JsonSerializerSettings();

                serializerSettings.ContractResolver = new Utils.JsonNamesHelper();

                return JsonConvert.DeserializeObject<DetailResponse<TModel>>(responseJSON, serializerSettings);
            }
            else
            {
                var result = new InvokeResult();
                result.Errors.Add(new ErrorMessage("failure code response"));
                return null;
            }
        }

        public async Task<DetailResponse<TResponseModel>> GetAsync<TResponseModel>(string path, CancellationTokenSource cancellationTokenSource = null) where TResponseModel : new()
        {
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource(15 * 1000); /* Abort after 15 seconds */
            }

            if (!await _tokenManager.ValidateTokenAsync(_authManager, cancellationTokenSource))
            {
                var errs = new InvokeResult();
                errs.Errors.Add(new ErrorMessage("could Not Add Item: " + System.Net.HttpStatusCode.Unauthorized));
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authManager.AccessToken);

            var response = await _httpClient.GetAsync(path, cancellationTokenSource.Token);
            if (response.IsSuccessStatusCode)
            {
                var responseJSON = await response.Content.ReadAsStringAsync();
                var serializerSettings = new JsonSerializerSettings();

                serializerSettings.ContractResolver = new Utils.JsonNamesHelper();

                return JsonConvert.DeserializeObject<DetailResponse<TResponseModel>>(responseJSON, serializerSettings);
            }
            else
            {
                var result = new InvokeResult();
                result.Errors.Add(new ErrorMessage("failure code response"));
                return null;
            }
        }

        public async Task<InvokeResult> UpdateAsync(String path, TModel model, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource(15 * 1000); /* Abort after 15 seconds */
            }

            if (!await _tokenManager.ValidateTokenAsync(_authManager, cancellationTokenSource))
            {
                var errs = new InvokeResult();
                errs.Errors.Add(new ErrorMessage("could Not Add Item: " + System.Net.HttpStatusCode.Unauthorized));
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authManager.AccessToken);

            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(path, content, cancellationTokenSource.Token);
                if (response.IsSuccessStatusCode)
                {
                    var responseJSON = await response.Content.ReadAsStringAsync();
                    var serializerSettings = new JsonSerializerSettings();
                    serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                    return JsonConvert.DeserializeObject<InvokeResult>(responseJSON, serializerSettings);
                }
                else
                {
                    var result = new InvokeResult();
                    result.Errors.Add(new ErrorMessage("failure code response"));
                    return result;
                }
            }
            catch (Exception)
            {
                var result = new InvokeResult();
                result.Errors.Add(new ErrorMessage("failure code response"));
                return result;
            }
        }
    }

    public class RestClient<TModel, TSummaryModel> : RestClient<TModel>, IRestClient<TModel, TSummaryModel> where TModel : new() where TSummaryModel : class
    {
        HttpClient _httpClient;
        IAuthManager _authManager;
        ITokenManager _tokenManager;
        ILogger _logger;
        INetworkService _networkService;
        public RestClient(HttpClient httpClient, IAuthManager authManager, ITokenManager tokenManager, ILogger logger, INetworkService networkService) : base(httpClient, authManager, tokenManager, logger, networkService)
        {
            _networkService = networkService;
            _logger = logger;
            _httpClient = httpClient;
            _authManager = authManager;
            _tokenManager = tokenManager;
        }

        public async Task<ListResponse<TSummaryModel>> GetForOrgAsync(string path, TModel model, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource(15 * 1000); /* Abort after 15 seconds */
            }

            if (!await _tokenManager.ValidateTokenAsync(_authManager, cancellationTokenSource))
            {
                var errs = new InvokeResult();
                errs.Errors.Add(new ErrorMessage("could Not Add Item: " + System.Net.HttpStatusCode.Unauthorized));
            }


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authManager.AccessToken);

            try
            {
                var response = await _httpClient.GetAsync(path, cancellationTokenSource.Token);
                if (response.IsSuccessStatusCode)
                {
                    var serializerSettings = new JsonSerializerSettings();
                    serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                    var responseJSON = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ListResponse<TSummaryModel>>(responseJSON);
                }
                else
                {
                    var result = new InvokeResult();
                    result.Errors.Add(new ErrorMessage("failure code response"));
                    return null;
                }
            }
            catch (HttpRequestException)
            {
                var result = new InvokeResult();
                result.Errors.Add(new ErrorMessage("Could Not Connect"));
                return null;
            }
        }
    }
}
