using LagoVista.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LagoVista.XPlat.iOS.Services
{
    public class DispatcherService : IDispatcherServices
    {
        public void Invoke(Action action)
        {
            action();
        }
    }
}