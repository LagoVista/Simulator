using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using LagoVista.Core.IOC;
using LagoVista.Core.PlatformSupport;
using LagoVista.XPlat.iOS.Services;
using LagoVista.Core;
using LagoVista.Client.Core.Loggers;

namespace LagoVista.Simulator.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            SLWIOC.Register<IStorageService>(new StorageService());
            SLWIOC.Register<INetworkService>(new NetworkService());
            SLWIOC.Register<IDeviceInfo>(new DeviceInfo());
            SLWIOC.Register<ILogger>(new ClientLogger());
            SLWIOC.Register<IDispatcherServices>(new DispatcherServices());


            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
