using LagoVista.Core.Networking.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.MQTT.Core.Clients
{
    public class MQTTDeviceClient : MQTTClientBase, IMQTTDeviceClient
    {        
        const string DEVICE_PASSWORD = "MQTT_APP_CLIENT_DEVICE_AUTH_TOKEN";
        const string DEVICE_ID = "MQTT_APP_CLIENT_DEVICE_ID";
        const string DEVICE_TYPE = "MQTT_APP_CLIENT_DEVICE_PASSWORD";

        const string BROKER_HOST_NAME = "MQTT_APP_CLIENT_BROKER_HOST_NAME";
        const string BROKER_PORT_NUMBER = "MQTT_APP_CLIENT_BROKER_PORT_NUMBER";

        public MQTTDeviceClient(IMqttNetworkChannel channel) : base(channel)
        {

        }

        public String DeviceType { get; set; }
        public String DeviceId { get; set; }
        public override  String Password { get; set; }

        public bool SettingsReady
        {
            get { return !String.IsNullOrEmpty(BrokerHostName); }
        }

        public async Task<bool> ReadSettingsAsync()
        {
            BrokerPort = Convert.ToInt32(await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(BROKER_PORT_NUMBER, "1883"));
            Password = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(DEVICE_PASSWORD);
            DeviceType = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(DEVICE_TYPE);
            DeviceId = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(DEVICE_ID);
            BrokerHostName = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(BROKER_HOST_NAME, DEFAULT_DOMAIN);

            return SettingsReady;
        }

        public async Task SaveSettingsAsync()
        {
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(DEVICE_PASSWORD, Password);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(DEVICE_TYPE, DeviceType);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(DEVICE_ID, DeviceId);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(BROKER_HOST_NAME, BrokerHostName);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(BROKER_PORT_NUMBER, BrokerPort.ToString());
        }

        public override String UserName
        {
            get { return DeviceId; }
        }

        public UInt16 PublishEvent(String evt, String format, String msg, byte qosLevel = 0)
        {
            var topic = String.Format("iot-2/evt/{0}/fmt/{1}", evt, format);
            var buffer = System.Text.UTF8Encoding.UTF8.GetBytes(msg);

            return Publish(topic, buffer);
        }

        public UInt16 PublishEvent<T>(String evt, String format, T payload)
        {
            var msg = JsonConvert.SerializeObject(payload);
            var topic = String.Format("iot-2/evt/{0}/fmt/{1}", evt, format);
            var buffer = System.Text.UTF8Encoding.UTF8.GetBytes(msg);

            return Publish(topic, buffer);
        }

        public UInt16 SubscribeCommand(String cmd, String format, byte qosLevel = 0)
        {
            var topic = String.Format("iot-2/cmd/{0}/fmt/{1}", cmd, format);

            return Subscribe(topic);
        }
    }
}
