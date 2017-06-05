using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.XPlat.Core
{
    public class Button : Xamarin.Forms.Button
    {
        public Button()
        {
            BackgroundColor = AppStyle.ButtonBackground.ToXamFormsColor();
            TextColor = AppStyle.ButtonForeground.ToXamFormsColor();
        }

        private IAppStyle AppStyle { get { return SLWIOC.Get<IAppStyle>(); } }
    }
}
