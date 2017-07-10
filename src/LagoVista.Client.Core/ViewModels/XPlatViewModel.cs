﻿using LagoVista.Client.Core.ViewModels.Auth;
using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;

namespace LagoVista.Client.Core.ViewModels
{
    public enum RightMenuIcon
    {
        None,
        Add,
        Edit,
        Save,
        Delete,
        Settings,
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

        public virtual void HandleURIActivation(Uri uri, Dictionary<string, string> kvps)
        {
            if(uri.Host == "resetpassword")
            {
                if (!kvps.ContainsKey("code"))
                {
                    Logger.AddCustomEvent(LagoVista.Core.PlatformSupport.LogLevel.Error, "ResetPassword_HandleURIActivation", "Missing Code", new KeyValuePair<string, string>("queryString", uri.Query));
                }
                else
                {
                    var launchArgs = new ViewModelLaunchArgs();
                    launchArgs.ViewModelType = typeof(ResetPasswordViewModel);
                    launchArgs.Parameters.Add("code", WebUtility.UrlDecode(kvps["code"]));
                    ViewModelNavigation.NavigateAsync(launchArgs);
                }
            }
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

        /// <summary>
        /// Override this on the view model to determine if the data is dirty and we can close the page.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanCancel()
        {
            return true;
        }
       

        public RelayCommand SaveCommand { get; private set; }

        public RelayCommand EditCommand { get; private set; }

    }
}
