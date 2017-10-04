﻿using LagoVista.Core.UWP.Controls;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ResolutionGroupName(LagoVista.XPlat.Core.UnderlineEffect.EffectNamespace)]
[assembly: ExportEffect(typeof(UnderlineEffect), nameof(UnderlineEffect))]
namespace LagoVista.Core.UWP.Controls
{
    public class UnderlineEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                var txtBlock = Control as TextBlock;
                txtBlock.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
               
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
        }
    }
}