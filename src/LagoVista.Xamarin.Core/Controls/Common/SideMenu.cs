using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core.Controls.Common
{
    public class SideMenu : ScrollView
    {
        StackLayout _container;

        public SideMenu()
        {
            _container = new StackLayout();
            Content = _container;
        }

        IEnumerable<LagoVista.Client.Core.ViewModels.MenuItem> _menuItems;
        public IEnumerable<LagoVista.Client.Core.ViewModels.MenuItem> MenuItems
        {
            get { return _menuItems; }
            set
            {
                _menuItems = value;
                _container.Children.Clear();
                if (_menuItems != null)
                {
                    foreach (var menuItem in _menuItems)
                    {
                        _container.Children.Add(new SideMenuItem(menuItem));
                    }
                }
            }
        }
    }
}
