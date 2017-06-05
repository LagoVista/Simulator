using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using LagoVista.Core.ViewModels;
using LagoVista.XPlat.Core.Views;
using System.Diagnostics;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core
{
    public abstract class LagoVistaContentPage : ContentPage, ILagoVistaPage
    {
        Grid _contentGrid;
        Grid _loadingMask;
        Grid _loadingContainer;
        Grid _menu;
        Grid _pageMenuMask;
        ActivityIndicator _activityIndicator;

        View _originalcontent;

        bool _hasAppeared = false;

        

        public LagoVistaContentPage() : base()
        {
            _activityIndicator = new ActivityIndicator() { IsRunning = false };
            _activityIndicator.Color = Color.CornflowerBlue;
            _loadingContainer = new Grid() { IsVisible = false };
            _loadingMask = new Grid() { BackgroundColor = Color.Black, Opacity = 0.25 };
            _loadingContainer.Children.Add(_loadingMask);
            _loadingContainer.Children.Add(_activityIndicator);

            _contentGrid = new Grid();
            Content = _contentGrid;

            _menu = new Grid();
            _menu.TranslationX = -300;
            _menu.BackgroundColor = AppStyle.MenuBarBackground.ToXamFormsColor();
            _menu.WidthRequest = 300;
            _menu.HorizontalOptions = LayoutOptions.Start;

            _pageMenuMask = new Grid();
            _pageMenuMask = new Grid() { BackgroundColor = Color.Black, Opacity = 0.25 };
        }

        private IAppStyle AppStyle { get { return SLWIOC.Get<IAppStyle>(); } }

        View _mainContent;
        public View MainContent
        {
            get { return _mainContent; }
            set
            {
                _mainContent = value;
                _contentGrid.Children.Add(_mainContent);
                _contentGrid.Children.Add(_pageMenuMask);                
                _contentGrid.Children.Add(_menu);
                _contentGrid.Children.Add(_loadingContainer);
            }
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

        public static readonly BindableProperty MenuVisibleProperty = BindableProperty.Create("MenuVisible", typeof(bool), typeof(bool), false, BindingMode.TwoWay, null,
            (view, oldValue, newValue) =>
            {
                if((bool)newValue)
                {
                    (view as LagoVistaContentPage)._menu.TranslateTo(0, 0, 250, Easing.CubicInOut);
                }
                else
                {
                    (view as LagoVistaContentPage)._menu.TranslateTo(-300, 0, 250, Easing.CubicInOut);
                }

                (view as LagoVistaContentPage)._pageMenuMask.IsVisible = (bool)newValue;
                /* Property Changed */
                Debug.WriteLine($"changed {oldValue} {newValue}");
            },
            (view, oldValue, newValue) =>
            {
                /* Property Changing */
            },
            null,
            (view) =>
            {
                return false;
            });
        public bool MenuVisible
        {
            get { return (bool)GetValue(MenuVisibleProperty); }
            set { SetValue(MenuVisibleProperty, value); }
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
                if (ViewModel != null)
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
