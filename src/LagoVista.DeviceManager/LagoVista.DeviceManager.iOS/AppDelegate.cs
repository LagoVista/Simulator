using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;


namespace LagoVista.DeviceManager.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public const string MOBILE_CENTER_KEY = "82b1c408-a2bf-42da-b285-eb56719ad2ed"; /* DEV */

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

            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
            app.StatusBarHidden = false;

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
