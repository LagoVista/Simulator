using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Networking.Models;
using LagoVista.MQTT.Core.Exceptions;
using LagoVista.MQTT.Core.Messages;
using LagoVista.MQTT.Core.Utility;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LagoVista.MQTT.Core.Clients
{
    public abstract class MQTTClientBase : IMQTTClientBase
    {
        private String _clientId;

        protected static readonly int MQTTS_PORT = 8883;
        private MqttClient _mqttClient;

        private IMqttNetworkChannel _channel;

        public event EventHandler<bool> ConnectionStateChanged;

        public event EventHandler<MqttMsgPublishEventArgs> MessageReceived;

        private bool _showDiagnostics = false;
        public bool ShowDiagnostics
        {
            get { return _showDiagnostics; }
            set
            {
                Trace.ShowDiagnostics = value;
                _showDiagnostics = value;
            }
        }

        public MQTTClientBase(IMqttNetworkChannel channel)
        {
            _channel = channel;
            _clientId = Guid.NewGuid().ToString();
        }


        private void Log(String message, params object[] args)
        {

        }

        private void LogDetails(String message, params object[] args)
        {

        }

        private void LogException(String area, Exception message)
        {

        }

        public String ClientId { get { return _clientId; } }

        public abstract String UserName { get; }

        public abstract string Password { get; set; }

        public async Task<MQTTConnectResult> ConnectAsync(bool ssl = false)
        {
            try
            {
                _mqttClient = new MqttClient(_channel);
                _mqttClient.Init(BrokerHostName, BrokerPort, false);
                _mqttClient.ConnectionClosed += MqttClient_ConnectionClosed;
                _mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;
                _mqttClient.MqttMsgSubscribed += MqttClient_MqttMsgSubscribed;
                _mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgArrived;

                var result = await _mqttClient.Connect(ClientId, UserName, Password);

                if (ConnectionStateChanged != null && result == ConnAck.Accepted)
                    ConnectionStateChanged(this, true);

                return new MQTTConnectResult(ConnAck.Accepted);
            }
            catch (MqttCommunicationException ex)
            {
                LogException("MQTTClient-Connect", ex);
                return new MQTTConnectResult(ConnAck.Exception, ex.Message);
            }
            catch (MqttConnectionException ex)
            {
                return new MQTTConnectResult(ConnAck.ServerUnavailable, ex.Message);
            }
            catch (MqttClientException ex)
            {
                return new MQTTConnectResult(ConnAck.Exception, ex.Message);
            }
            catch (Exception ex)
            {
                LogException("MQTTClient-Connect", ex);
                return new MQTTConnectResult(ConnAck.Exception, ex.Message);
            }
        }

        private void MqttClient_ConnectionClosed(object sender, EventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, false);
        }

        void MqttClient_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            Trace.WriteLine(Utility.TraceLevel.Verbose, "BLUEMIXMQTT-Message Subscribed", e.MessageId.ToString());
        }

        void MqttClient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Trace.WriteLine(Utility.TraceLevel.Verbose, "BLUEMIXMQTT-Message Published", e.MessageId.ToString());
        }

        public void MqttClient_MqttMsgArrived(object sender, MqttMsgPublishEventArgs e)
        {
            Trace.WriteLine(Utility.TraceLevel.Verbose, "BLUEMIXMQTT-Message Arrived", "[" + System.Text.Encoding.UTF8.GetString(e.Message, 0, e.Message.Length) + "]");

            MessageReceived?.Invoke(this, e);
        }

        public virtual void OnEventReceived(String evtName, string format, string payload) { }
        public virtual void OnDeviceStatusReceived(String deviceType, string deviceId, string payload) { }
        public virtual void OnCommandReceived(String cmdName, string format, string payload) { }
        public virtual void OnAppStatusReceived(String appId, string payload) { }

        protected UInt16 Subscribe(string[] topics, byte[] qosLevels)
        {
            return _mqttClient.Subscribe(topics, qosLevels);
        }

        public UInt16 Publish<T>(String topic, T payload, byte qosLevel = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE)
        {
            var json = JsonConvert.SerializeObject(payload);
            return Publish(topic, json, qosLevel);
        }

        public UInt16 Publish(String topic, String payload = "", byte qosLevel = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE)
        {
            var buffer = String.IsNullOrEmpty(payload) ? new byte[0] : System.Text.UTF8Encoding.UTF8.GetBytes(payload);
            return _mqttClient.Publish(topic, buffer, qosLevel, false);
        }


        public UInt16 Subscribe(string topic, byte qosLevel = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE)
        {
            return Subscribe(new String[] { topic }, new byte[] { qosLevel });
        }


        public void Disconnect()
        {
            if (_mqttClient == null)
                return;

            Debug.WriteLine("MQTTCLient-Disconnect - Started");
            try
            {
                Log("MQTTClient_Disconnect", "Disconnecting");
                _mqttClient.Disconnect();
                Log("MQTTClient_Disconnect", "Disconnected");
            }
            catch (Exception ex)
            {
                LogException("MQTTClient_Disconnect", ex);
            }
            finally
            {
                _mqttClient = null;
            }
        }

        public void Dispose()
        {
            if (_mqttClient != null)
            {
                Disconnect();
            }
        }

        public bool IsConnected
        {
            get
            {
                if (_mqttClient == null)
                    return false;

                return _mqttClient.IsConnected;
            }
        }


        public String BrokerHostName { get; set; }

        public int BrokerPort { get; set; }

        public override string ToString()
        {
            return "[" + ClientId + "] " + "Connected = " + IsConnected;
        }
    }
}
