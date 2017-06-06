using LagoVista.Core.ViewModels;
using System.Collections.Generic;

namespace LagoVista.Client.Core.ViewModels
{
    public enum RightMenuIcon
    {
        None,
        Add,
        Edit,
        Save,
        Delete,
        CustomIcon,
        CustomText
    }

    public enum LeftMenuIcon
    {
        None,
        Menu,
        Back,
        Cancel,
        CustomIcon,
        CustomText

    }

    public abstract class XPlatViewModel : ViewModelBase
    {
        RightMenuIcon _rightMenuIcon;
        public RightMenuIcon RightMenuIcon
        {
            get { return _rightMenuIcon; }
            set { Set(ref _rightMenuIcon, value); }
        }

        LeftMenuIcon _leftMenuIcon;
        public LeftMenuIcon LeftMenuIcon
        {
            get { return _leftMenuIcon; }
            set { Set(ref _leftMenuIcon, value); }
        }

        bool _menuVisible;
        public bool MenuVisible
        {
            get { return _menuVisible; }
            set { Set(ref _menuVisible, value); }
        }

        IEnumerable<MenuItem> _menuItems;
        public IEnumerable<MenuItem> MenuItems
        {
            get { return _menuItems; }
            set { Set(ref _menuItems, value); }
        }
    }
}
