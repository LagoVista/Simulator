using System;
using System.Threading.Tasks;
using LagoVista.MQTT.Core;

namespace LagoVista.XPlat.iOS.Services
{
    public class MqttNetworkChannel : IMqttNetworkChannel
    {
        public MqttNetworkChannel()
        {
        }

        public bool DataAvailable => throw new NotImplementedException();

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public Task InitAsync(string uri, int port, bool secure)
        {
            throw new NotImplementedException();
        }

        public int Receive(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int Receive(byte[] buffer, int timeout)
        {
            throw new NotImplementedException();
        }

        public int Send(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}
