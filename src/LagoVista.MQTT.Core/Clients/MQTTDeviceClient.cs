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
        public const string DEVICE_ORGID = "DEVICE_ORG_ID";
        public const string DEVICE_AUTH_TOKEN = "DEVICE_AUTH_TOKEN";

        public const string DEVICE_TYPE = "DEVICE_TYPE";
        public const string DEVICE_ID = "DEVICE_ID";
        public const string SERVER_URL = "SERVER_URL";

        public MQTTDeviceClient(IMqttNetworkChannel channel) : base(channel)
        {
        }

        public String DeviceType
        {
            get;
            set;
        }

        public String DeviceId
        {
            get;
            set;
        }

        public bool SettingsReady
        {
            get { return !(String.IsNullOrEmpty(OrgId) || !String.IsNullOrEmpty(APIToken) || !String.IsNullOrEmpty(DeviceId) || !String.IsNullOrEmpty(DeviceType)) || !String.IsNullOrEmpty(ServerURL); }
        }

        public async Task<bool> ReadSettingsAsync()
        {
            OrgId = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(DEVICE_ORGID);
            APIToken = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(DEVICE_AUTH_TOKEN);
            DeviceType = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(DEVICE_TYPE);
            DeviceId = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(DEVICE_ID);
            ServerURL = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(SERVER_URL, DEFAULT_DOMAIN);

            return SettingsReady;
        }

        public async Task SaveSettingsAsync()
        {
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(DEVICE_ORGID, OrgId);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(DEVICE_AUTH_TOKEN, APIToken);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(DEVICE_TYPE, DeviceType);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(DEVICE_ID, DeviceId);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(SERVER_URL, ServerURL);
        }

        public override String ClientId
        {
            get { return String.Format("d:{0}:{1}:{2}", OrgId, DeviceType, DeviceId); }
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
