using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.PlatformSupport;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LagoVista.Core.Interfaces;
using LagoVista.Client.Core.Models;
using System.Net.Http.Headers;
using System.Text;

namespace LagoVista.Client.Core.Net
{
    /*
     * 100% of all authenticated calls to the server with go through this singleton, idea is that if
     * a refresh token is upodated and two calls happen at the same time with the same refresh token
     * only one will succeed and the user will be locked out since the refresh tokens are single use.
     * 
     */
    public class RawRestClient : IRawRestClient
    {
        HttpClient _httpClient;
        IAuthManager _authManager;
        ITokenManager _tokenManager;
        ILogger _logger;

        SemaphoreSlim _callSemaphore;

        public RawRestClient(HttpClient httpClient, IAuthManager authManager, ITokenManager tokenManager, ILogger logger)
        {
            _httpClient = httpClient;
            _authManager = authManager;
            _tokenManager = tokenManager;
            _logger = logger;
            _callSemaphore = new SemaphoreSlim(1);
        }

        public async Task<RawResponse> PerformCall(Func<Task<HttpResponseMessage>> call, CancellationTokenSource cancellationTokenSource)
        {
            await _callSemaphore.WaitAsync();
            if (!await _tokenManager.ValidateTokenAsync(_authManager, cancellationTokenSource))
            {
                return RawResponse.FromTokenError();
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authManager.AccessToken);

            var retry = true;

            while (retry)
            {
                try
                {
                    var response = await call();
                    if (response.IsSuccessStatusCode)
                    {
                        return RawResponse.FromSuccess(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        /* Check for 401 (I think, if so then attempt to get a new access token,  */
                        return RawResponse.FromHttpFault((int)response.StatusCode, response.ReasonPhrase);
                    }

                }
                catch (TaskCanceledException tce)
                {
                    _callSemaphore.Release();
                    return RawResponse.FromException(tce, tce.CancellationToken.IsCancellationRequested);
                }
                catch (Exception ex)
                {
                    _callSemaphore.Release();
                    return RawResponse.FromException(ex);
                }
            }

            return RawResponse.FromNotAuthorized();
        }

        public Task<RawResponse> GetAsync(string path, CancellationTokenSource cancellationTokenSource)
        {
            return PerformCall(() =>
            {
                return _httpClient.GetAsync(path, cancellationTokenSource.Token);
            }, cancellationTokenSource);
        }

        public Task<RawResponse> PostAsync(string path, string payload, CancellationTokenSource cancellationTokenSource)
        {
            return PerformCall(() =>
            {
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                return _httpClient.PostAsync(path, content, cancellationTokenSource.Token);
            }, cancellationTokenSource);
        }

        public Task<RawResponse> PutAsync(string path, string payload, CancellationTokenSource cancellationTokenSource)
        {
            return PerformCall(() =>
            {
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                return _httpClient.PutAsync(path, content, cancellationTokenSource.Token);
            }, cancellationTokenSource);
        }

        public Task<RawResponse> DeleteAsync(string path, string payload, CancellationTokenSource cancellationTokenSource)
        {
            return PerformCall(() =>
            {
                return _httpClient.DeleteAsync(path, cancellationTokenSource.Token);
            }, cancellationTokenSource);
        }
    }
}
