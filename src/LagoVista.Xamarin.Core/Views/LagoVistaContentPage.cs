using LagoVista.Core.IOC;
using LagoVista.Core.ViewModels;
using LagoVista.XPlat.Core.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core
{
    public abstract class LagoVistaContentPage : ContentPage, ILagoVistaPage
    {
        Grid _loadingMask;
        Grid _loadingContainer;
        ActivityIndicator _activityIndicator;

        View _originalcontent;

        bool _hasAppeared = false;

        public LagoVistaContentPage() : base()
        {
            _activityIndicator = new ActivityIndicator() { IsRunning = false };
            _activityIndicator.Color = Color.White;
            _loadingContainer = new Grid() { IsVisible = false };
            _loadingMask = new Grid() { BackgroundColor = Color.Black, Opacity = 0.50 };
            _loadingContainer.Children.Add(_loadingMask);
            _loadingContainer.Children.Add(_activityIndicator);
        }

        public ViewModelBase ViewModel
        {
            get { return BindingContext as ViewModelBase; }
            set
            {
                BindingContext = value;
                value.PropertyChanged += Value_PropertyChanged;
            }
        }

        private void Value_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.IsBusy):
                    Debug.WriteLine(ViewModel.IsBusy ? " Showing busy" : "Hiding Busy");
                    _activityIndicator.IsRunning = ViewModel.IsBusy;
                    _loadingContainer.IsVisible = ViewModel.IsBusy;

                    Content = ViewModel.IsBusy ? _loadingMask : _originalcontent;
                    
                    break;
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            _originalcontent = this.Content;

            if (!_hasAppeared)
            {
                if (ViewModel != null)
                {
                    this.Content.BindingContext = ViewModel;
                    await ViewModel.InitAsync();
                }
            }
            else
            {
                if(ViewModel != null)
                {
                    this.Content.BindingContext = null;
                    this.Content.BindingContext = ViewModel;
                    await ViewModel.ReloadedAsync();
                }
            }

            _hasAppeared = true;
        }

        protected async override void OnDisappearing()
        {
            base.OnDisappearing();

            if (ViewModel != null)
            {
                await ViewModel.IsClosingAsync();
            }
        }
    }
}
