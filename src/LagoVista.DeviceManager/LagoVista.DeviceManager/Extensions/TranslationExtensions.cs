using LagoVista.DeviceManager.Core.Resources;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LagoVista.DeviceManager
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {

        // Look at: poeditor.com 

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return null;

            return DeviceManagerResources.ResourceManager.GetString(Text, CultureInfo.CurrentCulture);
        }
    }
}
