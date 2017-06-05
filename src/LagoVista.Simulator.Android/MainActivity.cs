using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.IOC;
using LagoVista.XPlat.Droid.Services;
using LagoVista.Core;
using LagoVista.Client.Core.Loggers;

namespace LagoVista.Simulator.Droid
{
    [Activity(Label = "LagoVista.Simulator", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            //https://play.google.com/apps/publish/?dev_acc=12258406958683843289

            SLWIOC.Register<IStorageService>(new StorageService());
            SLWIOC.Register<INetworkService>(new NetworkService());
            SLWIOC.Register<IDeviceInfo>(new DeviceInfo());
            SLWIOC.Register<ILogger>(new ClientLogger());
            SLWIOC.Register<IDispatcherServices>(new DispatcherServices());

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

