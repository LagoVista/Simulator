using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using LagoVista.Core.Models.Drawing;
using LagoVista.Core.ViewModels;
using LagoVista.XPlat.Core.Controls.Common;
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
        Label _title;
        ActivityIndicator _activityIndicator;

        View _originalcontent;

        bool _hasAppeared = false;

       
        public LagoVistaContentPage() : base()
        {
            _activityIndicator = new ActivityIndicator() { IsRunning = false };
            _activityIndicator.Color = NamedColors.NuvIoTDark.ToXamFormsColor();
            _loadingContainer = new Grid() { IsVisible = false };

            _loadingMask = new Grid() { BackgroundColor = Xamarin.Forms.Color.Black, Opacity = 0.10 };
            _loadingContainer.Children.Add(_loadingMask);
            _loadingContainer.Children.Add(_activityIndicator);
            _loadingContainer.SetValue(Grid.RowProperty, 1);

            _contentGrid = new Grid();
            _contentGrid.RowDefinitions.Add(new RowDefinition() { Height = 50 });
            _contentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            Content = _contentGrid;

            NavigationPage.SetHasNavigationBar(this, false);

            _menu = new Grid();
            _menu.TranslationX = -300;
            _menu.BackgroundColor = AppStyle.MenuBarBackground.ToXamFormsColor();
            _menu.WidthRequest = 300;
            _menu.HorizontalOptions = LayoutOptions.Start;
            _menu.SetValue(Grid.RowProperty, 1);

            _pageMenuMask = new Grid();
            _pageMenuMask.SetValue(Grid.RowProperty, 1);
            _pageMenuMask.IsVisible = false;
            _pageMenuMask = new Grid() { BackgroundColor = Xamarin.Forms.Color.Black, Opacity = 0.25 };

            AddToolBar();
        }

        private void AddToolBar()
        {
            var toolBar = new Grid();
            toolBar.BackgroundColor = AppStyle.TitleBarBackground.ToXamFormsColor();

            _title = new Label();
            _title.TextColor = AppStyle.TitleBarText.ToXamFormsColor();
            _title.FontSize = 24;
            _title.VerticalOptions = new LayoutOptions(LayoutAlignment.Center,false);
            _title.Margin = new Thickness(10, 0, 0, 0);

            IconButton _icoBtn = new IconButton(); ;
            _icoBtn.Text = "fa-500px";
            _icoBtn.HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
            _icoBtn.TextColor = Xamarin.Forms.Color.White;
            toolBar.Children.Add(_title);
            toolBar.Children.Add(_icoBtn);
            _contentGrid.Children.Add(toolBar);
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);

            _title.Text = Title;
        }

        private IAppStyle AppStyle { get { return SLWIOC.Get<IAppStyle>(); } }

        View _mainContent;
        public View MainContent
        {
            get { return _mainContent; }
            set
            {
                _mainContent = value;
                _mainContent.SetValue(Grid.RowProperty, 1);
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
