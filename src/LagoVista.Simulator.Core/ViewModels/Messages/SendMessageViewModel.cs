using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using LagoVista.IoT.Simulator.Admin.Resources;
using LagoVista.Simulator.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Simulator.Core.ViewModels.Messages
{
    public class SendMessageViewModel : XPlatViewModel
    {
        public SendMessageViewModel()
        {
            SendCommand = new RelayCommand(Send);
        }

        public override Task InitAsync()
        {
            MsgTemplate = LaunchArgs.Parent as MessageTemplate;
            BuildRequestContent();
            return base.InitAsync();
        }

        private void BuildRequestContent()
        {
            var sentContent = new StringBuilder();

            switch (MsgTemplate.Transport.Value)
            {

                case TransportTypes.TCP:
                    break;
                case TransportTypes.UDP:
                    break;
                case TransportTypes.AMQP:
                    break;
                case TransportTypes.MQTT:
                    break;
                case TransportTypes.RestHttps:
                case TransportTypes.RestHttp:
                    {
                        var protocol = MsgTemplate.Transport.Value == TransportTypes.RestHttps ? "https" : "http";
                        var uri = $"{protocol}://{MsgTemplate.EndPoint}:{MsgTemplate.Port}/{MsgTemplate.PathAndQueryString}";
                        sentContent.AppendLine($"Method : {MsgTemplate.HttpVerb}");
                        sentContent.AppendLine($"Host   : {MsgTemplate.EndPoint}");
                        sentContent.AppendLine($"Port   : {MsgTemplate.Port}");
                        sentContent.AppendLine($"Query  : {MsgTemplate.PathAndQueryString}");

                        foreach (var hdr in MsgTemplate.MessageHeaders)
                        {
                            sentContent.AppendLine($"{hdr.HeaderName}\t:{hdr.Value}");
                        }

                    }
                    break;
            }

            SentContent = sentContent.ToString();
        }

        public async void Send()
        {
            IsBusy = true;
            var fullResponseString = new StringBuilder();
            
            Success = true;

            switch (MsgTemplate.Transport.Value)
            {
                case TransportTypes.TCP:
                    {
                        if (LaunchArgs.HasParam("tcpclient"))
                        {
                            var client = LaunchArgs.GetParam<ITCPClient>("tcpclient");
                            await client.WriteAsync(MsgTemplate.TextPayload);
                        }
                    }
                    break;
                case TransportTypes.UDP:
                    break;
                case TransportTypes.AMQP:
                    break;

                case TransportTypes.MQTT:
                    if (LaunchArgs.HasParam("tcpclient"))
                    {
                        var client = LaunchArgs.GetParam<ITCPClient>("tcpclient");
                        await client.WriteAsync(MsgTemplate.TextPayload);
                    }

                    break;

                case TransportTypes.RestHttps:
                case TransportTypes.RestHttp:
                    {
                        try
                        {
                            var client = new HttpClient();
                            var protocol = MsgTemplate.Transport.Value == TransportTypes.RestHttps ? "https" : "http";
                            var uri = $"{protocol}://{MsgTemplate.EndPoint}:{MsgTemplate.Port}/{MsgTemplate.PathAndQueryString}";

                            HttpResponseMessage responseMessage = null;
                            

                            foreach (var hdr in MsgTemplate.MessageHeaders)
                            {
                                client.DefaultRequestHeaders.Add(hdr.HeaderName, hdr.Value);
                            }

                            switch (MsgTemplate.HttpVerb)
                            {
                                case MessageTemplate.HttpVerb_GET:
                                    responseMessage = await client.GetAsync(uri);
                                    break;
                                case MessageTemplate.HttpVerb_POST:
                                    responseMessage = await client.PostAsync(uri, new StringContent(MsgTemplate.TextPayload));
                                    break;
                                case MessageTemplate.HttpVerb_PUT:
                                    responseMessage = await client.PutAsync(uri, new StringContent(MsgTemplate.TextPayload));
                                    break;
                                case MessageTemplate.HttpVerb_DELETE: responseMessage = await client.DeleteAsync(uri); break;
                            }

                            var responseContent = await responseMessage.Content.ReadAsStringAsync();

                            fullResponseString.AppendLine($"Response Code: {(int)responseMessage.StatusCode} ({responseMessage.ReasonPhrase})");
                            foreach (var hdr in responseMessage.Headers)
                            {
                                fullResponseString.AppendLine($"{hdr.Key}\t:{hdr.Value.FirstOrDefault()}");
                            }
                            fullResponseString.AppendLine();
                            fullResponseString.Append(responseContent);
                        }
                        catch (Exception ex)
                        {
                            fullResponseString.AppendLine(Resources.SimulatorCoreResources.SendMessage_ErrorSendingMessage);
                            fullResponseString.AppendLine();
                            fullResponseString.Append(ex.Message);
                            Success = false;
                        }
                    }
                    break;

            }

            ReceivedContennt = fullResponseString.ToString();

            IsBusy = false;
            await Popups.ShowAsync(Success ? SimulatorCoreResources.SendMessage_MessageSent : SimulatorCoreResources.SendMessage_ErrorSendingMessage);
        }

        private string _sentContent;
        public String SentContent
        {
            get { return _sentContent; }
            set { Set(ref _sentContent, value); }
        }

        private string _receivedContent;
        public String ReceivedContennt
        {
            get { return _receivedContent; }
            set { Set(ref _receivedContent, value); }
        }


        MessageTemplate _message;
        public MessageTemplate MsgTemplate
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        private bool _success;
        public bool Success
        {
            get { return _success; }
            set { Set(ref _success, value); }
        }

        public RelayCommand SendCommand { get; set; }
    }
}
