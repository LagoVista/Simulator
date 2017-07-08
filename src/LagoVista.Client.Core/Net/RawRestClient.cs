﻿using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.PlatformSupport;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LagoVista.Core.Interfaces;
using LagoVista.Client.Core.Models;
using System.Net.Http.Headers;
using System.Text;
using LagoVista.Core.Validation;
using LagoVista.Core.Authentication.Interfaces;
using LagoVista.Core.Authentication.Models;

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
        ILogger _logger;
        IAuthClient _authClient;
        IDeviceInfo _deviceInfo;
        IAppConfig _appConfig;
        SemaphoreSlim _callSemaphore;

        public RawRestClient(HttpClient httpClient, IDeviceInfo deviceInfo, IAppConfig appConfig, IAuthClient authClient, IAuthManager authManager, ILogger logger)
        {
            _httpClient = httpClient;
            _authClient = authClient;
            _deviceInfo = deviceInfo;
            _authManager = authManager;
            _logger = logger;
            _appConfig = appConfig;
            _callSemaphore = new SemaphoreSlim(1);
        }

        private async Task<InvokeResult> RenewRefreshToken()
        {
            var authRequest = new AuthRequest();
            authRequest.AppId = _appConfig.AppId;
            authRequest.ClientType = "mobileapp";
            authRequest.DeviceId = _deviceInfo.DeviceUniqueId;
            authRequest.AppInstanceId = _authManager.AppInstanceId;
            authRequest.GrantType = "refreshtoken";
            authRequest.RefreshToken = _authManager.RefreshToken;

            var response = await _authClient.LoginAsync(authRequest);
            if (response.Successful)
            {
                _authManager.AccessToken = response.Result.AccessToken;
                _authManager.AccessTokenExpirationUTC = response.Result.AccessTokenExpiresUTC;
                _authManager.RefreshToken = response.Result.RefreshToken;
                _authManager.AppInstanceId = response.Result.AppInstanceId;
                _authManager.RefreshTokenExpirationUTC = response.Result.RefreshTokenExpiresUTC;
                await _authManager.PersistAsync();
                return InvokeResult.Success;
            }
            else
            {
                var result = new InvokeResult();
                result.Concat(response);
                return result;
            }
        }

        public async Task<RawResponse> PerformCall(Func<Task<HttpResponseMessage>> call, CancellationTokenSource cancellationTokenSource)
        {
            await _callSemaphore.WaitAsync();

            _httpClient.DefaultRequestHeaders.Clear();
            if (_authManager.IsAuthenticated)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authManager.AccessToken);
            }

            var retry = true;

            var rawResponse = RawResponse.FromNotCompleted();

            while (retry)
            {
                retry = false;
                try
                {
                    var response = await call();
                    if (response.IsSuccessStatusCode)
                    {
                        rawResponse = RawResponse.FromSuccess(await response.Content.ReadAsStringAsync());                        
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        retry = ((await RenewRefreshToken()).Successful);
                        if(!retry)
                        {
                            rawResponse = RawResponse.FromNotAuthorized();
                        }
                    }
                    else
                    {
                        /* Check for 401 (I think, if so then attempt to get a new access token,  */
                        rawResponse = RawResponse.FromHttpFault((int)response.StatusCode, response.ReasonPhrase);
                    }

                }
                catch (TaskCanceledException tce)
                {
                    rawResponse = RawResponse.FromException(tce, tce.CancellationToken.IsCancellationRequested);
                }
                catch (Exception ex)
                {
                    rawResponse = RawResponse.FromException(ex);
                }
            }

            _callSemaphore.Release();

            return rawResponse;
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
