﻿using LagoVista.XPlat.Core.Controls.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.UWP;
using Xamarin.Forms;
using LagoVista.Xamarin.UWP.CustomRenderers;
using LagoVista.XPlat.Core;

[assembly: ExportRenderer(typeof(IconButton), typeof(TitleBarButtonRenderer))]
namespace LagoVista.Xamarin.UWP.CustomRenderers
{

    public class TitleBarButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<global::Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
        }
    }
}
