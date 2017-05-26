using LagoVista.Core.IOC;
using LagoVista.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core.Views
{
    public abstract class LagoVistaContentPage : ContentPage, ILagoVistaPage
    {
        Grid _loadingMask;
        Grid _loadingContainer;
        ActivityIndicator _activityIndicator;

        bool _hasAppeared = false;

        public LagoVistaContentPage() : base()
        {
            _activityIndicator = new ActivityIndicator() { IsRunning = false };
            _loadingContainer = new Grid() { IsVisible = false };
            _loadingMask = new Grid() { BackgroundColor = Color.Black, Opacity = 0.25 };
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
                    break;
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!_hasAppeared)
            {
                var context = ViewModel;
                var content = this.Content;
                var grid = new Grid();
                grid.Children.Add(content);
                grid.Children.Add(_loadingContainer);
                this.Content = grid;
                
                if (ViewModel != null)
                {
                    await ViewModel.InitAsync();

                }
                
                this.Content.BindingContext = context;
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
