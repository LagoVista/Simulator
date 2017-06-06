using LagoVista.Client.Core.Net;
using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.UWP.Networking;
using LagoVista.Core.UWP.Services;
using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace LagoVista.Simulator.Windows
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            UnhandledException += (sender, e) =>
            {
                Debug.WriteLine("EXCPETION");
                Debug.WriteLine(e.Exception);
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.Exception.StackTrace);
                if (global::System.Diagnostics.Debugger.IsAttached) global::System.Diagnostics.Debugger.Break();
            };
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                Xamarin.Forms.Forms.Init(e);
              
                LagoVista.Core.UWP.Startup.Init(this, rootFrame.Dispatcher);

                SLWIOC.RegisterSingleton<IDeviceInfo>(new DeviceInfo());
                SLWIOC.Register<ITCPClient, TCPClient>();

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated){ }

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                Window.Current.Activate();
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();                                          
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
