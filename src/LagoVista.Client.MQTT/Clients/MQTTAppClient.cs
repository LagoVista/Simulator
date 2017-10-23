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
        public const string APP_ID = "MQTT_APP_CLIENT_APP_ID";
        public const string APP_PASSWORD = "MQTT_APP_CLIENT_APP_PASSWORD";

        public const string BROKER_HOST_NAME = "MQTT_APP_CLIENT_BROKER_HOST_NAME";
        public const string BROKER_PORT_NUMBER = "MQTT_APP_CLIENT_BROKER_PORT_NUMBER";

        public MQTTAppClient(IMqttNetworkChannel channel) : base(channel)
        {
        }

        public String AppId { get; set; }
        public override String Password { get; set; }

        public override String UserName
        {
            get { return AppId; }
        }

        public bool SettingsReady
        {
            get { return !String.IsNullOrEmpty(BrokerHostName); }
        }

        public async Task<bool> ReadSettingsAsync()
        {
            AppId = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(APP_ID);
            Password = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(APP_PASSWORD);

            BrokerPort = Convert.ToInt32(await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(BROKER_PORT_NUMBER, "1883"));
            BrokerHostName = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<String>(BROKER_HOST_NAME, "");

            return SettingsReady;
        }

        public async Task SaveSettingsAsync()
        {
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(APP_ID, AppId);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(APP_PASSWORD, Password);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(BROKER_HOST_NAME, BrokerHostName);
            await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP<String>(BROKER_PORT_NUMBER, BrokerPort.ToString());
        }

        public ushort SubscribeToApplicationStatus()
        {
            throw new NotImplementedException();
        }
    }
}
