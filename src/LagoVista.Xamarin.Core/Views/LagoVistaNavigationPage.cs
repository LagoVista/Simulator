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
            BarBackgroundColor = Color.FromRgb(0xBB, 0xDB, 0xFB);
            BarTextColor = Color.FromRgb(0x5B, 0x5B, 0x5B);
        }
    }
}
