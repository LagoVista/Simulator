using LagoVista.Client.Core.ViewModels;
using LagoVista.IoT.Runtime.Core.Models.Messaging;
using System;
using System.Diagnostics;


namespace LagoVista.PlatformManager.Core.ViewModels
{
    public class MonitorInstanceViewModel : MonitoringViewModelBase
    {
        public override string GetChannelURI()
        {
            return $"/api/wsuri/instance/{LaunchArgs.ChildId}/normal";
        }

        public override void HandleMessage(Notification notification)
        {
            if (String.IsNullOrEmpty(notification.PayloadType))
            {
                Debug.WriteLine("----");
                Debug.WriteLine(notification.PayloadType);
                Debug.WriteLine(notification.Payload);
                Debug.WriteLine("----");
            }
            else
            {
                Debug.WriteLine(notification.Text);
            }
        }
    }
}
