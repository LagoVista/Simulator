﻿using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using LagoVista.XPlat.Core.Icons;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core.Controls.Common
{
    /// <summary>
    /// Defines the <see cref="IconButton" /> control.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Button" />
    public class IconButton : Button
    {
        public IconButton()
        {
            BackgroundColor = Xamarin.Forms.Color.Transparent;
            TextColor = AppStyle.TitleBarText.ToXamFormsColor();
        }

        private IAppStyle AppStyle { get { return SLWIOC.Get<IAppStyle>(); } }

        public static BindableProperty IconKeyProperty = BindableProperty.Create(nameof(IconKey), typeof(string), typeof(IconButton), default(string), BindingMode.Default, null,
            (view, oldValue, newValue) => (view as Icon).IconKey = (string)newValue);

        public string IconKey
        {
            get { return (string)GetValue(Icon.IconKeyProperty); }
            set
            {
                SetValue(Icon.IconKeyProperty, value);
                var icon = Iconize.FindIconForKey(value);

                switch (Device.RuntimePlatform)
                {
                    case Device.UWP: FontFamily = $"{Iconize.FindModuleOf(icon).FontPath}#{Iconize.FindModuleOf(icon).FontName}"; break;
                    case Device.iOS: FontFamily = Iconize.FindModuleOf(icon).FontName; break;
                    case Device.Android: FontFamily = $"{Iconize.FindModuleOf(icon).FontPath}#{Iconize.FindModuleOf(icon).FontName}"; break;
                }

                Text = $"{icon.Character}";
            }
        }
    }
}
