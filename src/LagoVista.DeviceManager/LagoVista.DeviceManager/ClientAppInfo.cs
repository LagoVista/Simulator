using LagoVista.Client.Core;
using LagoVista.DeviceManager.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.DeviceManager
{
    public class ClientAppInfo : IClientAppInfo
    {
        public Type MainViewModel => typeof(MainViewModel);
    }
}
