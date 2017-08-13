using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace LagoVista.DeviceManager.Droid
{
    [Activity(Label = "Device Manager", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const string MOBILE_CENTER_KEY = "5bbcacd9-cd4c-4f01-8715-9e9edf743ea0";

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            /* https://play.google.com/apps/publish/?dev_acc=12258406958683843289 */
            LagoVista.XPlat.Droid.Startup.Init(BaseContext, MOBILE_CENTER_KEY);

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

