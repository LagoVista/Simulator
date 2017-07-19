using LagoVista.Client.Core.ViewModels;
using LagoVista.IoT.Runtime.Core.Models.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.PlatformManager.Core.ViewModels
{
    public class ListenerViewModel : MonitoringViewModelBase
    {
        public override string GetChannelURI()
        {
            return $"/api/wsuri/pipelinemodule/{LaunchArgs.ChildId}/normal";
        }

        public override void HandleMessage(Notification notification)
        {
            Debug.WriteLine(notification.PayloadType);
            Debug.WriteLine(notification.Payload);
        }  
    }
}
