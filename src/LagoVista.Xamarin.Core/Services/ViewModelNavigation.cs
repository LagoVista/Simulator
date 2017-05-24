
using LagoVista.Core.IOC;
using LagoVista.Core.ViewModels;
using LagoVista.XPlat.Core.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core.Services
{
    public class ViewModelNavigation : IViewModelNavigation
    {
        Dictionary<Type, Type> _viewModelLookup = new Dictionary<Type, Type>();

        global::Xamarin.Forms.Application _app;
        global::Xamarin.Forms.INavigation _navigation;

        public ViewModelNavigation(global::Xamarin.Forms.Application app)
        {
            _app = app;
        }

        public void Start<V>() where V : ViewModelBase
        {
            var view = Activator.CreateInstance(_viewModelLookup[typeof(V)]) as LagoVistaContentPage;
            _navigation = view.Navigation;
            var viewModel = SLWIOC.CreateForType<V>();
            view.ViewModel = viewModel;
            _app.MainPage = new NavigationPage(view)
            {
                Title = "HelloWorld"
            };

            Debug.WriteLine(_app.MainPage);
        }

        public void Add<T, V>() where T : ViewModelBase where V : ILagoVistaPage
        {
            _viewModelLookup.Add(typeof(T), typeof(V));
        }

        public void Navigate<TViewModel>() where TViewModel : ViewModelBase
        {
            Navigate(new ViewModelLaunchArgs()
            {
                ViewModelType = typeof(TViewModel)
            });
        }

        public void GoBack()
        {
            _navigation.PopAsync();
        }

        public async void Navigate(ViewModelLaunchArgs args)
        {
            var viewModel = SLWIOC.CreateForType(args.ViewModelType);

            var view = Activator.CreateInstance(_viewModelLookup[args.ViewModelType]) as LagoVistaContentPage;
            view.ViewModel = viewModel as ViewModelBase;
            view.ViewModel.SetParameter(args.Parameter);

            await _navigation.PushAsync(view);
        }

        public void PopToRoot()
        {
            _navigation.PopToRootAsync();
        }

        public void SetAsNewRoot()
        {
            var currentPage = _navigation.NavigationStack.Last();
            _app.MainPage = new NavigationPage(currentPage);
        }

        public void SetAsNewRoot<TViewModel>() where TViewModel : ViewModelBase
        {
            SetAsNewRoot<TViewModel>(null);
        }

        public void SetAsNewRoot<TViewModel>(ViewModelLaunchArgs args) where TViewModel : ViewModelBase
        {
            var viewModel = SLWIOC.CreateForType<TViewModel>();
            var viewModelType = typeof(TViewModel);
            var view = Activator.CreateInstance(_viewModelLookup[viewModelType]) as LagoVistaContentPage;
            view.ViewModel = viewModel as ViewModelBase;
            if (args != null)
            {
                view.ViewModel.SetParameter(args.Parameter);
            }
            _navigation = view.Navigation;
            _app.MainPage = new NavigationPage(view);
        }

        public bool CanGoBack()
        {
            return _navigation.NavigationStack.Count > 1;
        }
    }
}