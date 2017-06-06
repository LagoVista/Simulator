using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Intentionally left blank

    }
}
