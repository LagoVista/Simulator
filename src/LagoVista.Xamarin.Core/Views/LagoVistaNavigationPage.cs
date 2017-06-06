using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core.Views
{
    public class LagoVistaNavigationPage : NavigationPage
    {
        public LagoVistaNavigationPage(Page root) : base(root)
        {
            BarBackgroundColor = AppStyle.TitleBarBackground.ToXamFormsColor();
            BarTextColor = AppStyle.TitleBarText.ToXamFormsColor();
            BackgroundColor = AppStyle.PageBackground.ToXamFormsColor();

        }


//        https://wolfprogrammer.com/2016/07/07/custom-app-header-in-forms/

        private IAppStyle AppStyle { get { return SLWIOC.Get<IAppStyle>(); } }
    }
}
