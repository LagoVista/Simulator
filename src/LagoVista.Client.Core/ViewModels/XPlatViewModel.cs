using LagoVista.Core.Commanding;
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
        public XPlatViewModel()
        {
            SaveCommand = new RelayCommand(SaveAsync, CanSave);
            EditCommand = new RelayCommand(Edit);
        }

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

        public virtual void Edit()
        {

        }

        public virtual void SaveAsync()
        {

        }

        public virtual bool CanSave()
        {
            return true;
        }

        public virtual bool CanCancel()
        {
            return true;
        }
       

        public RelayCommand SaveCommand { get; private set; }

        public RelayCommand EditCommand { get; private set; }

    }
}
