using LagoVista.Client.Core.Net;
using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Simulator.Core.ViewModels.Messages
{
    public class SendMessageViewModel : ViewModelBase
    {
        public SendMessageViewModel()
        {
            SendCommand = new RelayCommand(Send);
        }

        public override Task InitAsync()
        {
            MessageTemplate = LaunchArgs.Parent as MessageTemplate;

            return base.InitAsync();
        }

        public void Send()
        {
            switch (MessageTemplate.Transport.Value)
            {
                case TransportTypes.TCP:
                    if(LaunchArgs.HasParam("tcpclient"))
                    {
                        var client = LaunchArgs.GetParam<ITCPClient>("tcpclient");
                        client.WriteAsync(MessageTemplate.TextPayload);
                    }

                    break;
                case TransportTypes.UDP:
                    break;
                case TransportTypes.AMQP:
                    break;
                case TransportTypes.RestHttp:
                    break;

            }

            Popups.ShowAsync("MESSAGE SENT!");
        }


        MessageTemplate _message;
        public MessageTemplate MessageTemplate
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        public RelayCommand SendCommand { get; set; }
    }
}
