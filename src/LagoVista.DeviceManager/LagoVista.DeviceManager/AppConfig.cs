using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.DeviceManager
{
    public class AppConfig : IAppConfig
    {
        public PlatformTypes PlatformType => PlatformTypes.WindowsUWP;

        public Environments Environment => Environments.Local;

        public string WebAddress => "http://localhost:5000";

        public string AppName => "Device Manager";

        public string AppLogo => "";

        public string CompanyLogo => "";

        public bool EmitTestingCode => true;

        public string AppId => "D1C45FD985934A40A15FC3322BBCCCCB";
        public string ClientType => "mobileapp";

    }
}
