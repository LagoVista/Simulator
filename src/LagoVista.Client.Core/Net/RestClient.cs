using LagoVista.Core.Models;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.PlatformSupport;
using System;
using System.Threading;
using System.Threading.Tasks;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LagoVista.Client.Core.Net
{

    public class FormRestClient<TModel> : IFormRestClient<TModel> where TModel : new()
    {
        const int CALL_TIMEOUT_SECONDS = 15;
        IRestClient _rawRestClient;

        public FormRestClient(IRestClient rawRestClient)
        {
            _rawRestClient = rawRestClient;
        }

        public async Task<InvokeResult> AddAsync(string path, TModel model, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource == null) cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var json = JsonConvert.SerializeObject(model, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver(), });
            var response = await _rawRestClient.PostAsync(path, json, cancellationTokenSource);
            return response.ToInvokeResult();
        }

        public async Task<InvokeResult<TResponseModel>> PostAsync<TResponseModel>(string path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TResponseModel : new()
        {
            if (cancellationTokenSource == null) cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var json = JsonConvert.SerializeObject(model, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver(), });
            var response = await _rawRestClient.PostAsync(path, json, cancellationTokenSource);
            return response.ToInvokeResult<TResponseModel>();
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
            if (cancellationTokenSource == null) cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var response = await _rawRestClient.GetAsync(path, cancellationTokenSource);
            return response.ToDetailResponse<TModel>();

        }

        public async Task<DetailResponse<TResponseModel>> GetAsync<TResponseModel>(string path, CancellationTokenSource cancellationTokenSource = null) where TResponseModel : new()
        {
            if (cancellationTokenSource == null) cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var response = await _rawRestClient.GetAsync(path, cancellationTokenSource);
            return response.ToDetailResponse<TResponseModel>();
        }

        public async Task<InvokeResult> UpdateAsync(String path, TModel model, CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource == null) cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var json = JsonConvert.SerializeObject(model, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver(), });
            var response = await _rawRestClient.PutAsync(path, json, cancellationTokenSource);
            return response.ToInvokeResult();
        }
    }
}
