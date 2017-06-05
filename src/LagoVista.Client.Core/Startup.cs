using LagoVista.Client.Core.Resources;
using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Client.Core
{
    public static class Startup
    {
        public static void Init()
        {
            SLWIOC.RegisterSingleton<IAppStyle>(new AppStyle());
        }
    }
}
