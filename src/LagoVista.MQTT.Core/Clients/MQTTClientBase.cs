using LagoVista.Core.Networking.Interfaces;
using LagoVista.MQTT.Core.Exceptions;
using LagoVista.MQTT.Core.Messages;
using LagoVista.MQTT.Core.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LagoVista.MQTT.Core.Clients
{
    public abstract class MQTTClientBase : IMQTTClientBase
    {
        protected static String DEVICE_EVENT_PATTERN = "iot-2/type/(.+)/id/(.+)/evt/(.+)/fmt/(.+)";
        protected static String DEVICE_STATUS_PATTERN = "iot-2/type/(.+)/id/(.+)/mon";
        protected static String APP_STATUS_PATTERN = "iot-2/app/(.+)/mon";
        //protected static String DEVICE_COMMAND_PATTERN = "iot-2/type/(.+)/id/(.+)/cmd/(.+)/fmt/(.+)";

        protected static String DEVICE_COMMAND_PATTERN = "iot-2/cmd/(.+)/fmt/(.+)";

        protected static readonly String CLIENT_ID_DELIMITER = ":";
        protected static readonly String DEFAULT_DOMAIN = "messaging.internetofthings.ibmcloud.com";
        protected static readonly int MQTTS_PORT = 8883;
        private MqttClient _mqttClient;

        private IMqttNetworkChannel _channel;

        public event EventHandler<bool> ConnectionStateChanged;

        public event EventHandler<IMQTTAppStatusReceivedEventArgs> AppStatusReceived;
        public event EventHandler<IMQTTCommandEventArgs> CommandReceived;
        public event EventHandler<IMQTTEventReceivedEventArgs> EventReceived;
        public event EventHandler<IMQTTEventDeviceStatusReceivedEventArgs> DeviceStatusReceived;

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

        public abstract String ClientId { get; }

        public async Task<ConnAck> Connect()
        {
            try
            {
                _mqttClient = new MqttClient(_channel);
                _mqttClient.Init(MQTTAddress, 1883, false);
                _mqttClient.ConnectionClosed += _mqttClient_ConnectionClosed;
                _mqttClient.MqttMsgPublished += client_MqttMsgPublished;
                _mqttClient.MqttMsgSubscribed += _mqttClient_MqttMsgSubscribed;
                _mqttClient.MqttMsgPublishReceived += client_MqttMsgArrived;

                var result = await _mqttClient.Connect(ClientId, String.IsNullOrEmpty(APIKey) ? "use-token-auth" : APIKey, APIToken);

                if (ConnectionStateChanged != null)
                    ConnectionStateChanged(this, true);

                return ConnAck.Accepted;
            }
            catch (MqttCommunicationException ex)
            {
                LogException("BLUEMIXMQTT-Connect", ex);
                return ConnAck.Exception;
            }
            catch (Exception ex)
            {
                LogException("BLUEMIXMQTT-Connect", ex);
                return ConnAck.Exception;
            }
        }

        private void _mqttClient_ConnectionClosed(object sender, EventArgs e)
        {
            if (ConnectionStateChanged != null)
                ConnectionStateChanged(this, false);
        }

        void _mqttClient_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            Trace.WriteLine(Utility.TraceLevel.Verbose, "BLUEMIXMQTT-Message Subscribed", e.MessageId.ToString());
        }

        void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Trace.WriteLine(Utility.TraceLevel.Verbose, "BLUEMIXMQTT-Message Published", e.MessageId.ToString());
        }

        public void client_MqttMsgArrived(object sender, MqttMsgPublishEventArgs e)
        {
            Trace.WriteLine(Utility.TraceLevel.Verbose, "BLUEMIXMQTT-Message Arrived", "[" + System.Text.Encoding.UTF8.GetString(e.Message, 0, e.Message.Length) + "]");
            var result = System.Text.Encoding.UTF8.GetString(e.Message, 0, e.Message.Length);
            var topic = e.Topic;
            var tokens = topic.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            var matchEvent = Regex.Match(topic, DEVICE_EVENT_PATTERN);
            if (matchEvent.Success)
            {
                if (EventReceived != null)
                    EventReceived(this, MQTTEventReceivedEventArgs.Create("", "", matchEvent.Groups[3].Value, matchEvent.Groups[4].Value, result));

                OnEventReceived(matchEvent.Groups[3].Value, matchEvent.Groups[4].Value, result);
            }

            Match matchDeviceStatus = Regex.Match(topic, DEVICE_STATUS_PATTERN);
            if (matchDeviceStatus.Success)
            {
                if (DeviceStatusReceived != null)
                    DeviceStatusReceived(this, MQTTEventDeviceStatusReceivedEventArgs.Create(matchEvent.Groups[1].Value, matchEvent.Groups[2].Value, result));

                OnDeviceStatusReceived(matchDeviceStatus.Groups[1].Value, matchDeviceStatus.Groups[2].Value, result);
            }

            var matchAppStatus = Regex.Match(topic, APP_STATUS_PATTERN);
            if (matchAppStatus.Success)
            {
                if (AppStatusReceived != null)
                    AppStatusReceived(this, MQTTAppStatusReceivedEventArgs.Create(matchAppStatus.Groups[1].Value, result));

                OnAppStatusReceived(matchAppStatus.Groups[1].Value, result);
            }

            var matchCommand = Regex.Match(topic, DEVICE_COMMAND_PATTERN);
            if (matchCommand.Success)
            {
                if (CommandReceived != null)
                    CommandReceived(this, MQTTCommandEventArgs.Create(matchCommand.Groups[1].Value, matchCommand.Groups[2].Value, result));

                OnCommandReceived(matchCommand.Groups[3].Value, matchCommand.Groups[4].Value, result);
            }
        }

        public virtual void OnEventReceived(String evtName, string format, string payload)
        {

        }

        public virtual void OnDeviceStatusReceived(String deviceType, string deviceId, string payload)
        {

        }

        public virtual void OnCommandReceived(String cmdName, string format, string payload)
        {

        }

        public virtual void OnAppStatusReceived(String appId, string payload)
        {

        }

        protected UInt16 Subscribe(string[] topics, byte[] qosLevels)
        {
            return _mqttClient.Subscribe(topics, qosLevels);
        }

        protected UInt16 Publish(String topic, byte[] data)
        {
            return _mqttClient.Publish(topic, data);
        }

        protected UInt16 Subscribe(string topic, byte qosLevel = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE)
        {
            return Subscribe(new String[] { topic }, new byte[] { qosLevel });
        }


        public void Disconnect()
        {
            if (_mqttClient == null)
                return;

            Debug.WriteLine("BLUEMIXMQTT-Disconnect - Started");
            try
            {

                _mqttClient.Disconnect();

                Debug.WriteLine("BLUEMIXMQTT-Disconnect - Disconnected");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BLUEMIXMQTT-Disconnect" + ex.Message);
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

        public String OrgId
        {
            get;
            set;
        }

        public String APIKey
        {
            get;
            set;
        }

        public String APIToken
        {
            get;
            set;
        }

        public String ServerURL
        {
            get;
            set;
        }

        private String MQTTAddress
        {
            get { return String.Format("{0}.{1}", OrgId, ServerURL); }
        }


        public override string ToString()
        {
            return "[" + ClientId + "] " + "Connected = " + IsConnected;
        }

    }

    public class MQTTEventReceivedEventArgs : EventArgs, IMQTTEventReceivedEventArgs
    {
        public static MQTTEventReceivedEventArgs Create(String deviceType, String deviceId, String evtName, string fmt, string payload)
        {
            return new MQTTEventReceivedEventArgs()
            {
                EventName = evtName,
                Format = fmt,
                Payload = payload
            };
        }

        public String DeviceType { get; private set; }
        public String DeviceId { get; private set; }

        public String EventName { get; private set; }
        public String Format { get; private set; }

        public String Payload { get; private set; }

        public T DeserializePayload<T>()
        {
            return JsonConvert.DeserializeObject<T>(Payload);
        }
    }

    public class MQTTCommandEventArgs : EventArgs, IMQTTCommandEventArgs
    {
        public static MQTTCommandEventArgs Create(String cmdName, string fmt, string payload)
        {
            return new MQTTCommandEventArgs()
            {
                CommandName = cmdName,
                Format = fmt,
                Payload = payload
            };
        }


        public String CommandName { get; private set; }
        public String Format { get; private set; }

        public String Payload { get; private set; }

        public T DeserializePayload<T>()
        {
            return JsonConvert.DeserializeObject<T>(Payload);
        }
    }

    public class MQTTEventDeviceStatusReceivedEventArgs : EventArgs, IMQTTEventDeviceStatusReceivedEventArgs
    {
        public static MQTTEventDeviceStatusReceivedEventArgs Create(String deviceType, String deviceId, String payload)
        {
            return new MQTTEventDeviceStatusReceivedEventArgs()
            {
                DeviceType = deviceType,
                DeviceId = deviceId,
                Payload = payload
            };
        }

        public String DeviceType { get; private set; }
        public String DeviceId { get; private set; }

        public String Payload { get; private set; }

        public T DeserializePayload<T>()
        {
            return JsonConvert.DeserializeObject<T>(Payload);
        }
    }

    public class MQTTAppStatusReceivedEventArgs : EventArgs, IMQTTAppStatusReceivedEventArgs
    {
        public static MQTTAppStatusReceivedEventArgs Create(String appId, String payload)
        {
            return new MQTTAppStatusReceivedEventArgs()
            {
                AppId = appId,
                Payload = payload
            };
        }

        public String AppId { get; private set; }

        public String Payload { get; private set; }

        public T DeserializePayload<T>()
        {
            return JsonConvert.DeserializeObject<T>(Payload);
        }
    }
}
