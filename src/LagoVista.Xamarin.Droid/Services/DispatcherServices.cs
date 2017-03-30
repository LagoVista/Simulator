using System;
using LagoVista.Core;

namespace LagoVista.XPlat.Droid.Services
{
    public class DispatcherServices : IDispatcherServices
    {
        public void Invoke(Action action)
        {
            var context = global::Android.App.Application.Context;
            
        }
    }
}