using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace LagoVista.PlatformManager.Droid
{
    [Activity(Label = "Platform Manager", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const string MOBILE_CENTER_KEY = "1276e0bd-4c34-4b88-81d1-61c999e0dd7e";

        protected override void OnCreate(Bundle bundle)
        {
            //https://play.google.com/apps/publish/?dev_acc=12258406958683843289
            LagoVista.XPlat.Droid.Startup.Init(BaseContext, MOBILE_CENTER_KEY);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

