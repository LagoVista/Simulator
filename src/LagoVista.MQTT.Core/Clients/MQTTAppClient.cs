using LagoVista.Core.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LagoVista.MQTT.Core.Clients
{
    public class MQTTAppClient : MQTTClientBase, IMQTTAppClient
    {
        public const string APP_ORG_ID = "APP_ORG_ID";
        public const string APP_AUTH_KEY = "APP_AUTH_KEY";
        public const string APP_AUTH_TOKEN = "APP_AUTH_TOKEN";

        public const string APP_ID = "APP_ID";

        public MQTTAppClient(IMqttNetworkChannel channel) : base(channel)
        {
        }

        public String AppId
        {
            get;
            set;
        }

        public override String ClientId
        {
            get { return String.Format("a:{0}:{1}", OrgId, AppId); }
        }

        public bool SettingsReady
        {
            get { return !(String.IsNullOrEmpty(OrgId) || String.IsNullOrEmpty(APIKey) || String.IsNullOrEmpty(APIToken) || String.IsNullOrEmpty(AppId)); }
        }

        public async Task<bool> ReadSettingsAsync()
        {
            OrgId = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(APP_ORG_ID);
            APIKey = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(APP_AUTH_KEY);
            APIToken = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(APP_AUTH_TOKEN);
            AppId = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(APP_ID);

            return SettingsReady;
        }

        public async Task SaveSettingsAsync()
        {
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(APP_ORG_ID, OrgId);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(APP_AUTH_KEY, APIKey);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(APP_AUTH_TOKEN, APIToken);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(APP_ID, AppId);
        }

        public UInt16 SubscribeToDeviceStatus(String deviceType = "+", String deviceId = "+")
        {
            var topic = String.Format("iot-2/type/{0}/id/{1}/mon", deviceType, deviceId);

            return Subscribe(topic);
        }

        public UInt16 SubscribeToApplicationStatus()
        {
            var topic = String.Format("iot-2/app/{0}/mon");

            return Subscribe(topic);
        }


        public UInt16 SubscribeToDeviceEvents(string deviceType = "+", string deviceId = "+", string evt = "+", string format = "+")
        {
            var deviceTopic = String.Format("iot-2/type/{0}/id/{1}/evt/{2}/fmt/{3}", deviceType, deviceId, evt, format);

            return Subscribe(deviceTopic);
        }

        public UInt16 SubscribeToDeviceCommands(string deviceType = "+", string deviceId = "+", string cmd = "+", string format = "+")
        {
            var topic = String.Format("iot-2/type/{0}/id/{1}/cmd/{2}/fmt/{3}", deviceType, deviceId, cmd, format);

            return Subscribe(topic);
        }


        public UInt16 PublishCommand(String deviceType, String deviceId, String command, string format, string data)
        {
            var topic = String.Format("iot-2/type/{0}/id/{1}/cmd/{2}/fmt/{3}", deviceType, deviceId, command, format);
            var buffer = System.Text.Encoding.UTF8.GetBytes(data);

            return Publish(topic, buffer);
        }

        public UInt16 PublishEvent(String deviceType, String deviceId, String evt, string format, string data)
        {
            var topic = String.Format("iot-2/type/{0}/id/{1}/evt/{2}/fmt/{3}", deviceType, deviceId, evt, format);
            var buffer = System.Text.Encoding.UTF8.GetBytes(data);

            return Publish(topic, buffer);
        }
    }
}
