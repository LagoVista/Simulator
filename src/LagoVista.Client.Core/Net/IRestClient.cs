using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.Net
{
    public interface IRestClient<TModel> where TModel : new()
    {
        Task<DetailResponse<TModel>> CreateNewAsync(String path, CancellationTokenSource cancellationTokenSource = null);
        Task<InvokeResult> AddAsync(String path, TModel model, CancellationTokenSource cancellationTokenSource = null);
        Task<InvokeResult> UpdateAsync(String path, TModel model, CancellationTokenSource cancellationTokenSource = null);
        Task<InvokeResult<TResponseModel>> PostAsync<TResponseModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null);

        Task<DetailResponse<TModel>> GetAsync(String path, CancellationTokenSource cancellationTokenSource = null);

        Task<DetailResponse<TResponseModel>> GetAsync<TResponseModel>(String path, CancellationTokenSource cancellationTokenSource = null) where TResponseModel : new();

        Task<InvokeResult> DeleteAsync(String path, TModel model, CancellationTokenSource cancellationTokenSource = null);
    }

    public interface IRestClient<TModel, TSummaryModel> : IRestClient<TModel> where TModel : new() where TSummaryModel : class
    {
        Task<ListResponse<TSummaryModel>> GetForOrgAsync(String path, TModel model, CancellationTokenSource cancellationTokenSource = null);
    }
}
