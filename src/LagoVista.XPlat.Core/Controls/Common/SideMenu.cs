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
        public event EventHandler<Client.Core.ViewModels.MenuItem> MenuItemTapped;

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
                        menuItem.MenuItemTapped += MenuItem_MenuItemTapped1;
                        _container.Children.Add(new SideMenuItem(menuItem));
                    }
                }
            }
        }

        private void MenuItem_MenuItemTapped1(object sender, Client.Core.ViewModels.MenuItem e)
        {
            MenuItemTapped?.Invoke(sender, e);
        }
    }
}
