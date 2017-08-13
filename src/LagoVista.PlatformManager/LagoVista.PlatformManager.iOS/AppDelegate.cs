using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace LagoVista.PlatformManager.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private const string MOBILE_CENTER_KEY = "e3d48293-22fe-4bfb-9a7d-9fa41112c23f";

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            LagoVista.XPlat.iOS.Startup.Init(app, MOBILE_CENTER_KEY);

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
