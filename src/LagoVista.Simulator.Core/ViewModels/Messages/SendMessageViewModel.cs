using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Commanding;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.ViewModels;
using LagoVista.IoT.Simulator.Admin.Models;
using LagoVista.IoT.Simulator.Admin.Resources;
using LagoVista.Simulator.Core.Resources;
using Microsoft.Azure.EventHubs;
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
            ApplySettingsCommand = new RelayCommand(ApplySettings);
            ShowSettingsCommand = new RelayCommand(ShowSettings);
        }

        public override Task InitAsync()
        {
            MsgTemplate = LaunchArgs.Parent as MessageTemplate;

            BuildRequestContent();

            return base.InitAsync();
        }

        private String ReplaceTokens(String input)
        {
            foreach (var attr in MsgTemplate.DynamicAttributes)
            {
                input = input.Replace($"~{attr.Key}~", attr.DefaultValue);
            }

            return input;
        }

        public void ShowSettings()
        {
            SettingsVisible = true;
        }

        public void ApplySettings()
        {
            SettingsVisible = false;
            BuildRequestContent();
        }

        private void BuildRequestContent()
        {
            var sentContent = new StringBuilder();

            switch (MsgTemplate.Transport.Value)
            {

                case TransportTypes.TCP:
                    sentContent.AppendLine($"Host   : {MsgTemplate.EndPoint}");
                    sentContent.AppendLine($"Port   : {MsgTemplate.Port}");
                    sentContent.AppendLine($"Body");
                    sentContent.AppendLine($"---------------------------------");
                    sentContent.Append(ReplaceTokens(MsgTemplate.TextPayload));

                    break;
                case TransportTypes.UDP:
                    sentContent.AppendLine($"Host   : {MsgTemplate.EndPoint}");
                    sentContent.AppendLine($"Port   : {MsgTemplate.Port}");
                    sentContent.AppendLine($"Body");
                    sentContent.AppendLine($"---------------------------------");
                    sentContent.Append(ReplaceTokens(MsgTemplate.TextPayload));

                    break;
                case TransportTypes.AzureEventHub:
                    sentContent.AppendLine($"Host   : {MsgTemplate.Name}");
                    sentContent.AppendLine($"Port   : {MsgTemplate.Port}");
                    sentContent.AppendLine($"Body");
                    sentContent.AppendLine($"---------------------------------");
                    sentContent.Append(ReplaceTokens(MsgTemplate.TextPayload));


                    break;
                case TransportTypes.MQTT:
                    sentContent.AppendLine($"Host   : {MsgTemplate.EndPoint}");
                    sentContent.AppendLine($"Port   : {MsgTemplate.Port}");
                    sentContent.AppendLine($"Topic  : {ReplaceTokens(MsgTemplate.Topic)}");

                    sentContent.Append(ReplaceTokens(MsgTemplate.TextPayload));

                    break;
                case TransportTypes.RestHttps:
                case TransportTypes.RestHttp:
                    {
                        var protocol = MsgTemplate.Transport.Value == TransportTypes.RestHttps ? "https" : "http";
                        var uri = $"{protocol}://{MsgTemplate.EndPoint}:{MsgTemplate.Port}/{MsgTemplate.PathAndQueryString}";
                        sentContent.AppendLine($"Method : {MsgTemplate.HttpVerb}");
                        sentContent.AppendLine($"Host   : {MsgTemplate.EndPoint}");
                        sentContent.AppendLine($"Port   : {MsgTemplate.Port}");
                        sentContent.AppendLine($"Query  : {ReplaceTokens(MsgTemplate.PathAndQueryString)}");

                        foreach (var hdr in MsgTemplate.MessageHeaders)
                        {
                            sentContent.AppendLine($"{hdr.HeaderName}\t:{ReplaceTokens(hdr.Value)}");
                        }

                        sentContent.Append(ReplaceTokens(MsgTemplate.TextPayload));
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
                    if (LaunchArgs.HasParam("tcpclient"))
                    {
                        try
                        {
                            var client = LaunchArgs.GetParam<ITCPClient>("tcpclient");
                            var msg = MsgTemplate.TextPayload;
                            if (MsgTemplate.AppendCR) msg += "\r";
                            if (MsgTemplate.AppendLF) msg += "\n";
                            await client.WriteAsync(ReplaceTokens(msg));
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
                case TransportTypes.UDP:
                    if (LaunchArgs.HasParam("udpclient"))
                    {
                        try
                        {
                            var client = LaunchArgs.GetParam<IUDPClient>("udpclient");
                            var msg = MsgTemplate.TextPayload;
                            if (MsgTemplate.AppendCR) msg += "\r";
                            if (MsgTemplate.AppendLF) msg += "\n";
                            await client.WriteAsync(ReplaceTokens(msg));
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
                case TransportTypes.AzureEventHub:
                    try
                    {
                        var client = LaunchArgs.GetParam<EventHubClient>("ehclient");
                        await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(ReplaceTokens(MsgTemplate.TextPayload))));
                    }
                    catch (Exception ex)
                    {
                        fullResponseString.AppendLine(Resources.SimulatorCoreResources.SendMessage_ErrorSendingMessage);
                        fullResponseString.AppendLine();
                        fullResponseString.Append(ex.Message);
                        Success = false;
                    }
                    break;

                case TransportTypes.MQTT:
                    try
                    {
                        if (LaunchArgs.HasParam("mqttclient"))
                        {
                            var client = LaunchArgs.GetParam<IMQTTDeviceClient>("mqttclient");
                            client.Publish(ReplaceTokens(MsgTemplate.Topic), ReplaceTokens(MsgTemplate.TextPayload));
                        }
                    }
                    catch (Exception ex)
                    {
                        fullResponseString.AppendLine(Resources.SimulatorCoreResources.SendMessage_ErrorSendingMessage);
                        fullResponseString.AppendLine();
                        fullResponseString.Append(ex.Message);
                        Success = false;
                    }
                    break;

                case TransportTypes.RestHttps:
                case TransportTypes.RestHttp:
                    {
                        try
                        {
                            var client = new HttpClient();
                            var protocol = MsgTemplate.Transport.Value == TransportTypes.RestHttps ? "https" : "http";
                            var uri = $"{protocol}://{MsgTemplate.EndPoint}:{MsgTemplate.Port}/{ReplaceTokens(MsgTemplate.PathAndQueryString)}";

                            HttpResponseMessage responseMessage = null;


                            foreach (var hdr in MsgTemplate.MessageHeaders)
                            {
                                client.DefaultRequestHeaders.Add(hdr.HeaderName, ReplaceTokens(hdr.Value));
                            }

                            switch (MsgTemplate.HttpVerb)
                            {
                                case MessageTemplate.HttpVerb_GET:
                                    responseMessage = await client.GetAsync(uri);
                                    break;
                                case MessageTemplate.HttpVerb_POST:
                                    responseMessage = await client.PostAsync(uri, new StringContent(ReplaceTokens(MsgTemplate.TextPayload)));
                                    break;
                                case MessageTemplate.HttpVerb_PUT:
                                    responseMessage = await client.PutAsync(uri, new StringContent(ReplaceTokens(MsgTemplate.TextPayload)));
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

        public byte[] GetBinaryPayload(string binaryPayload)
        {
            if (String.IsNullOrEmpty(binaryPayload))
            {
                return new byte[0];
            }

            try
            {
                var bytes = new List<Byte>();

                var bytesList = binaryPayload.Split(' ');
                foreach (var byteStr in bytesList)
                {
                    var lowerByteStr = byteStr.ToLower();
                    if (lowerByteStr.Contains("soh"))
                    {
                        bytes.Add(0x01);

                    }
                    else if (lowerByteStr.Contains("stx"))
                    {
                        bytes.Add(0x02);
                    }
                    else if (lowerByteStr.Contains("etx"))
                    {
                        bytes.Add(0x03);
                    }
                    else if (lowerByteStr.Contains("eot"))
                    {
                        bytes.Add(0x04);
                    }
                    else if (lowerByteStr.Contains("ack"))
                    {
                        bytes.Add(0x06);
                    }
                    else if (lowerByteStr.Contains("cr"))
                    {
                        bytes.Add(0x0d);
                    }
                    else if (lowerByteStr.Contains("lf"))
                    {
                        bytes.Add(0x0a);
                    }
                    else if (lowerByteStr.Contains("nak"))
                    {
                        bytes.Add(0x15);
                    }
                    else if (lowerByteStr.Contains("esc"))
                    {
                        bytes.Add(0x1b);
                    }
                    else if (lowerByteStr.Contains("del"))
                    {
                        bytes.Add(0x1b);
                    }
                    else if (lowerByteStr.StartsWith("0x"))
                    {
                        bytes.Add(Byte.Parse(byteStr.Substring(2), System.Globalization.NumberStyles.HexNumber));
                    }
                    else
                    {
                        bytes.Add(Byte.Parse(byteStr, System.Globalization.NumberStyles.HexNumber));
                    }
                }

                return bytes.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception(SimulatorCoreResources.SendMessage_InvalidBinaryPayload + " " + ex.Message);
            }
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

        private bool _settingsVisible = false;
        public bool SettingsVisible
        {
            get { return _settingsVisible; }
            set { Set(ref _settingsVisible, value); }
        }
       

        public RelayCommand ShowSettingsCommand { get; set; }

        public RelayCommand ApplySettingsCommand { get; set; }

        public RelayCommand SendCommand { get; set; }
    }
}
