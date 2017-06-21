﻿/*
Copyright (c) 2013, 2014 Paolo Patierno

All rights reserved. This program and the accompanying materials
are made available under the terms of the Eclipse Public License v1.0
and Eclipse Distribution License v1.0 which accompany this distribution. 

The Eclipse Public License is available at 
   http://www.eclipse.org/legal/epl-v10.html
and the Eclipse Distribution License is available at 
   http://www.eclipse.org/org/documents/edl-v10.php.

Contributors:
   Paolo Patierno - initial API and implementation and/or initial documentation
*/

using System;
using System.Diagnostics;

namespace uPLibrary.Networking.M2Mqtt.Utility
{
    /// <summary>
    /// Tracing levels
    /// </summary>
    public enum TraceLevel
    {
        Warning = 0x02,
        Information = 0x04,
        Verbose = 0x0F,
        Frame = 0x10,
        Queuing = 0x20
    }

    // delegate for writing trace
    //public delegate void WriteTrace(string format, params object[] args);

    /// <summary>
    /// Tracing class
    /// </summary>
    public static class Trace
    {
        private static bool _showDiagnostics = false;
        public static bool ShowDiagnostics
        {
            get { return _showDiagnostics; }
            set { _showDiagnostics = value; }
        }

        public static void WriteLine(TraceLevel level, string message, params object[] args)
        {

            switch(level)
            {
                case TraceLevel.Verbose:
                    if (ShowDiagnostics)
                    {
                        if (args != null && args.Length > 0)
                            LagoVista.Common.PlatformSupport.Services.Logger.Log(LagoVista.Common.PlatformSupport.LogLevel.Verbose, "MQTT", String.Format(message, args));
                        else
                            LagoVista.Common.PlatformSupport.Services.Logger.Log(LagoVista.Common.PlatformSupport.LogLevel.Verbose, "MQTT", message);
                    }
                    break;
                case TraceLevel.Frame:
                case TraceLevel.Information:
                case TraceLevel.Queuing:
                    if (ShowDiagnostics)
                    {
                        if (args != null && args.Length > 0)
                            LagoVista.Common.PlatformSupport.Services.Logger.Log(LagoVista.Common.PlatformSupport.LogLevel.Message, "MQTT", String.Format(message, args));
                        else
                            LagoVista.Common.PlatformSupport.Services.Logger.Log(LagoVista.Common.PlatformSupport.LogLevel.Message, "MQTT", message);
                    }
                    break;
                case TraceLevel.Warning:
                    if (args != null && args.Length > 0)
                        LagoVista.Common.PlatformSupport.Services.Logger.Log(LagoVista.Common.PlatformSupport.LogLevel.Warning, "MQTT", String.Format(message, args));
                    else
                        LagoVista.Common.PlatformSupport.Services.Logger.Log(LagoVista.Common.PlatformSupport.LogLevel.Warning, "MQTT", message);
                    break;

            }

        }
    }
}