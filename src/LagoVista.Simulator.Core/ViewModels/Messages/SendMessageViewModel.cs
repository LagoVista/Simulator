using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
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

            return base.InitAsync();
        }

        public async void Send()
        {
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
                case TransportTypes.RestHttp:
                    {
                        try
                        {
                            var client = new HttpClient();
                            foreach (var hdr in MsgTemplate.MessageHeaders)
                            {
                             
                            }

                            switch (MsgTemplate.HttpVerb)
                            {
                                case MessageTemplate.HttpVerb_GET:
                                    var uri = $"{MsgTemplate.EndPoint}:{MsgTemplate.Port}/{MsgTemplate.PathAndQueryString}";
                                    var response = await client.GetAsync(uri);

                                    break;
                            }
                        }
                        catch(Exception ex)
                        {
                            ReceivedContennt = ex.Message;
                        }
                    }
                    break;

            }

            await Popups.ShowAsync("MESSAGE SENT!");
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

        public RelayCommand SendCommand { get; set; }
    }
}
