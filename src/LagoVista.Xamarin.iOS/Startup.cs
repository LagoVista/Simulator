using UIKit;
using LagoVista.XPlat.iOS.Services;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core;
using LagoVista.Core.IOC;

namespace LagoVista.XPlat.iOS
{
    public static class Startup
    {
        public static void Init(UIApplication app)
        {
            SLWIOC.Register<IStorageService>(new StorageService());
            SLWIOC.Register<INetworkService>(new NetworkService());
            SLWIOC.Register<IDeviceInfo>(new DeviceInfo());
            SLWIOC.Register<IDispatcherServices>(new DispatcherService(app));

            IconFonts.IconFontSupport.RegisterFonts();
        }
    }
}