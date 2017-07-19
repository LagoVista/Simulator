using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.IOC;
using LagoVista.Core.Validation;
using LagoVista.IoT.Runtime.Core.Models.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

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
