using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.IOC;
using LagoVista.Core.Validation;
using LagoVista.IoT.Runtime.Core.Models.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.PlatformManager.Core.ViewModels
{
    public abstract class MonitoringViewModelBase : AppViewModelBase
    {
        Uri _wsUri;
        IWebSocket _webSocket;

        public async override Task InitAsync()
        {
            var callResult = await PerformNetworkOperation(async () =>
            {
                var channelId = GetChannelURI();
                Debug.WriteLine("Asing for end URI: " + channelId);
                var wsResult = await RestClient.GetAsync<InvokeResult<string>>(channelId);
                if (wsResult.Successful)
                {
                    var url = wsResult.Result.Result;
                    Debug.WriteLine(url);
                    _wsUri = new Uri(url);
                    _webSocket = SLWIOC.Create<IWebSocket>();
                    _webSocket.MessageReceived += _webSocket_MessageReceived;
                    var wsOpenResult = await _webSocket.OpenAsync(_wsUri);
                    if(wsOpenResult.Successful)
                    {
                        Debug.WriteLine("OPENED CHANNEL");
                    }
                    return wsOpenResult;
                }
                else
                {
                    return wsResult.ToInvokeResult();
                }
            });
        }

        public async override Task IsClosingAsync()
        {
            if (_webSocket != null)
            {
                await _webSocket.CloseAsync();
                Debug.WriteLine("Web Socket is Closed.");
                _webSocket = null;
            }
        }

        private void _webSocket_MessageReceived(object sender, string json)
        {
            Debug.WriteLine(json);
            var notification = JsonConvert.DeserializeObject<Notification>(json);
            HandleMessage(notification);
        }

        public abstract void HandleMessage(Notification json);

        public abstract string GetChannelURI();
    }
}
