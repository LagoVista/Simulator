using LagoVista.Core.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.MQTT.Core.Models
{
    public class ConnectResult
    {
        public ConnectResult(ConnAck result, string message = "")
        {
            Result = result;
            Message = message;
        }

        public ConnectResult(ConnAck result, Exception ex)
        {
            Result = result;
            Message = ex.Message;
        }

        public ConnAck Result { get; private set; }

        public string Message { get; private set; }

    }
}
