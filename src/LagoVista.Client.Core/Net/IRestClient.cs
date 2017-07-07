using LagoVista.Client.Core.Models;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.Net
{
    /* 
     * Methods to encapsulate making authenticated calls with refresh tokens to the server,
     * only working with string payloads (if applicable) and returning string.
     * calling methods are responsible for serialization/deserialization
     * should only really be used with relatively small chunks of data,
     * will need a different mechanism for uploading pictures.
     */
    public interface IRawRestClient
    {
        Task<RawResponse> GetAsync(String path, CancellationTokenSource tokenSource);
        Task<RawResponse> PostAsync(String path, String payload, CancellationTokenSource tokenSource);
        Task<RawResponse> PutAsync(String path, String payload, CancellationTokenSource tokenSource);
        Task<RawResponse> DeleteAsync(String path, String payload, CancellationTokenSource tokenSource);
    }


    public interface IRestClient<TModel> where TModel : new()
    {
        Task<DetailResponse<TModel>> CreateNewAsync(String path, CancellationTokenSource cancellationTokenSource = null);


        Task<InvokeResult> AddAsync(String path, TModel model, CancellationTokenSource cancellationTokenSource = null);
        Task<InvokeResult> UpdateAsync(String path, TModel model, CancellationTokenSource cancellationTokenSource = null);
        Task<InvokeResult<TResponseModel>> PostAsync<TResponseModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TResponseModel :new();

        Task<DetailResponse<TModel>> GetAsync(String path, CancellationTokenSource cancellationTokenSource = null);

        Task<DetailResponse<TResponseModel>> GetAsync<TResponseModel>(String path, CancellationTokenSource cancellationTokenSource = null) where TResponseModel : new();

        Task<InvokeResult> DeleteAsync(String path, TModel model, CancellationTokenSource cancellationTokenSource = null);
    }


    public interface IRestClient<TModel, TSummaryModel> : IRestClient<TModel> where TModel : new() where TSummaryModel : class
    {
        Task<ListResponse<TSummaryModel>> GetForOrgAsync(String path, CancellationTokenSource cancellationTokenSource = null);
    }
}
