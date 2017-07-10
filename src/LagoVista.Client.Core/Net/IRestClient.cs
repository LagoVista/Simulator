﻿using LagoVista.Client.Core.Models;
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
    public interface IRestClient
    {
        Task<RawResponse> GetAsync(String path, CancellationTokenSource tokenSource);
        Task<RawResponse> PostAsync(String path, String payload, CancellationTokenSource tokenSource);
        Task<RawResponse> PutAsync(String path, String payload, CancellationTokenSource tokenSource);
        Task<RawResponse> DeleteAsync(String path, String payload, CancellationTokenSource tokenSource);

        Task<InvokeResult> PostAsync<TModel>(String path, TModel model, CancellationTokenSource cancellationTokenSource = null) where TModel : new();
        Task<InvokeResult<TResponseModel>> GetAsync<TResponseModel>(String path, CancellationTokenSource cancellationTokenSource = null) where TResponseModel : new(); 
    }
}
