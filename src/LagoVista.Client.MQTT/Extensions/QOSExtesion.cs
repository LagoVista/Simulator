using LagoVista.Core.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.MQTT.Core
{
    public static class QOSExtesion
    {
        public static byte ToByte(this QOS value)
        {
            switch(value)
            {
                case QOS.QOS0: return 0;
                case QOS.QOS1: return 1;
                case QOS.QOS2: return 2;
            }

            throw new Exception("Unexpected QOS value");
        }

        public static QOS ToQOS(this byte value)
        {
            switch (value)
            {
                case 0: return QOS.QOS0;
                case 1: return QOS.QOS1;
                case 2: return QOS.QOS2;
            }

            throw new Exception("Unexpected QOS value");
        }
    }
}
