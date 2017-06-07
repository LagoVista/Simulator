using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LagoVista.Core.PlatformSupport;
using Microsoft.Azure.Mobile;

namespace LagoVista.XPlat.Droid.Loggers
{
    public class MobileCenterLogger : ILogger
    {
        public MobileCenterLogger(string key)
        {
            MobileCenter.Start($"android={key}");
        }

        public void Log(LagoVista.Core.PlatformSupport.LogLevel level, string area, string message, params KeyValuePair<string, string>[] args)
        {
            throw new NotImplementedException();
        }

        public void LogException(string area, Exception ex, params KeyValuePair<string, string>[] args)
        {
            throw new NotImplementedException();
        }

        public void SetKeys(params string[] args)
        {
            throw new NotImplementedException();
        }

        public void SetUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public void TrackEvent(string message, Dictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }
    }
}