using LagoVista.Core.IOC;
using LagoVista.Core.ViewModels;
using System;
using System.Collections.Generic;
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

        public LagoVistaContentPage() : base()
        {
            _activityIndicator = new ActivityIndicator() { IsRunning = false };
            _loadingContainer = new Grid() { IsVisible = false };
            _loadingMask = new Grid() { BackgroundColor = Color.Black, Opacity = 0.25 };
            _loadingContainer.Children.Add(_loadingMask);
            _loadingContainer.Children.Add(_activityIndicator);
            _loadingContainer.IsVisible = true;
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
                    _activityIndicator.IsRunning = ViewModel.IsBusy;
                    _loadingContainer.IsVisible = ViewModel.IsBusy;
                    break;
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var context = ViewModel;
            var content = this.Content;
            var grid = new Grid();
            grid.Children.Add(content);
            grid.Children.Add(_loadingContainer);

            if (ViewModel != null)
            {
                await ViewModel.InitAsync();

            }

            this.Content = grid;
            this.Content.BindingContext = context;
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
