using UIKit;
using LagoVista.XPlat.iOS.Services;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core;
using LagoVista.Core.IOC;
using LagoVista.XPlat.Core.Services;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Push;
using LagoVista.Client.Core.Net;

namespace LagoVista.XPlat.iOS
{
    public static class Startup
    {
        public static void Init(UIApplication app, string key)
        {
            SLWIOC.RegisterSingleton<ILogger>(new Loggers.MobileCenterLogger(key));
            SLWIOC.Register<IStorageService>(new StorageService());
            SLWIOC.Register<INetworkService>(new NetworkService());
            SLWIOC.Register<IDeviceInfo>(new DeviceInfo());
            SLWIOC.Register<IPopupServices>(new PopupServices());
            SLWIOC.Register<IWebSocket,Services.WebSocket>();
            SLWIOC.Register<IDispatcherServices>(new DispatcherService(app));

		    IconFonts.IconFontSupport.RegisterFonts();
        }
    }
}