using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LagoVista.Simulator.Views
{
    public class HomePage : MasterDetailPage
    {
        public HomePage()
        {
            MasterBehavior = MasterBehavior.Popover;
        }
    }
}
