using Foundation;
using System.Diagnostics;
using UIKit;

namespace LagoVista.Simulator.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            LagoVista.XPlat.iOS.Startup.Init(app);
            
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            app.StatusBarHidden = false;

            return base.FinishedLaunching(app, options);
        }
    }
}
