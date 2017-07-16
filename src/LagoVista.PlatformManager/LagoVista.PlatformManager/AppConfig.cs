using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LagoVista.PlatformManager
{
    public class AppConfig : IAppConfig
    {
        public PlatformTypes PlatformType
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    
                }

                return PlatformTypes.WindowsUWP;
            }
        }

        public Environments Environment => Environments.Local;

        public string WebAddress => "http://localhost:5000";

        public string AppName => "Platform Manager";

        public string AppLogo => "";

        public string CompanyLogo => "";

        public bool EmitTestingCode => true;

        public string AppId => "0200AC799F384D15B1D233E73793D416";
        public string ClientType => "mobileapp";

    }
}
