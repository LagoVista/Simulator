using FormsPlugin.Iconize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core.Views
{
    public class LagoVistaNavigationPage : IconNavigationPage
    {
        public LagoVistaNavigationPage(Page root) : base(root)
        {
            BarBackgroundColor = LagoVista.Core.Models.Drawing.NamedColors.NuvIoTBlack.ToXamFormsColor();
            BarTextColor = Color.White;
            BackgroundColor = Color.FromRgb(0x2e, 0x35, 0x3D);
        }
    }
}
