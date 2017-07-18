using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.IOC;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.PlatformManager.Core.ViewModels
{
    public class MonitorInstanceViewModel : AppViewModelBase
    {
        Uri _wsUri;
        IWebSocket _webSocket;

        public async override Task InitAsync()
        {
            var callResult = await PerformNetworkOperation(async () =>
            {
                var wsResult = await RestClient.GetAsync<InvokeResult<string>>($"/api/wsuri/instance/{LaunchArgs.ChildId}/normal");
                if (wsResult.Successful)
                {
                    _wsUri = new Uri(wsResult.Result.Result);
                    _webSocket = SLWIOC.Create<IWebSocket>();
                    _webSocket.MessageReceived += _webSocket_MessageReceived;
                    var wsOpenResult = await _webSocket.OpenAsync(_wsUri);
                    return wsOpenResult;
                }
                else
                {
                    return wsResult.ToInvokeResult();
                }
            });
        }

        public override Task IsClosingAsync()
        {
            if (_webSocket != null)
            {
                return _webSocket.CloseAsync();
            }

            return Task.FromResult(default(object));
        }

        private void _webSocket_MessageReceived(object sender, string e)
        {
            Debug.WriteLine(e);
        }
    }
}
