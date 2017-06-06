
using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.IOC;
using LagoVista.Core.ViewModels;
using LagoVista.XPlat.Core.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;


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

        public void Start<V>() where V : XPlatViewModel
        {
            var view = Activator.CreateInstance(_viewModelLookup[typeof(V)]) as LagoVistaContentPage;
            _navigation = view.Navigation;
            var viewModel = SLWIOC.CreateForType<V>();
            view.ViewModel = viewModel;
            _app.MainPage = new LagoVistaNavigationPage(view)
            {
                Title = "HelloWorld"
            };

            Debug.WriteLine(_app.MainPage);
        }

        public void Add<T, V>() where T : ViewModelBase where V : ILagoVistaPage
        {
            _viewModelLookup.Add(typeof(T), typeof(V));
        }

        private Task ShowViewModelAsync(ViewModelLaunchArgs args)
        {
            var view = Activator.CreateInstance(_viewModelLookup[args.ViewModelType]) as LagoVistaContentPage;
            var viewModel = SLWIOC.CreateForType(args.ViewModelType) as XPlatViewModel;
            viewModel.LaunchArgs = args;
            view.ViewModel = viewModel;
            return _navigation.PushAsync(view);

        }

        public Task NavigateAsync<TViewModel>() where TViewModel : ViewModelBase
        {
            var args = new ViewModelLaunchArgs();
            args.LaunchType = LaunchTypes.Other;
            args.ViewModelType = typeof(TViewModel);
            return ShowViewModelAsync(args);
        }

        public Task NavigateAsync(ViewModelLaunchArgs args) 
        {
            return ShowViewModelAsync(args);
        }

        public  Task GoBackAsync()
        {
            return _navigation.PopAsync();
        }


        public void PopToRoot()
        {
            _navigation.PopToRootAsync();
        }
     
       
        public Task SetAsNewRootAsync<TViewModel>() where TViewModel : ViewModelBase
        {
            var viewModel = SLWIOC.CreateForType<TViewModel>();
            var viewModelType = typeof(TViewModel);
            var view = Activator.CreateInstance(_viewModelLookup[viewModelType]) as LagoVistaContentPage;
            view.ViewModel = viewModel as XPlatViewModel;            
            _navigation = view.Navigation;
            _app.MainPage = new LagoVistaNavigationPage(view);

            return Task.FromResult(default(object));
        }

        public bool CanGoBack()
        {
            return _navigation.NavigationStack.Count > 1;
        }

        public  Task NavigateAndCreateAsync<TViewModel>(params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase
        {
            var launchArgs = new ViewModelLaunchArgs();
            launchArgs.LaunchType = LaunchTypes.Create;
            launchArgs.ViewModelType = typeof(TViewModel);
           
            foreach (var arg in args)
            {
                launchArgs.Parameters.Add(arg.Key, arg.Value);
            }

            return ShowViewModelAsync(launchArgs);
        }

        public Task NavigateAndCreateAsync<TViewModel>(object parent, params KeyValuePair<string, object>[] args)  where TViewModel : ViewModelBase
        {
            var launchArgs = new ViewModelLaunchArgs();
            launchArgs.LaunchType = LaunchTypes.Create;
            launchArgs.ViewModelType = typeof(TViewModel);
            launchArgs.Parent = parent;

            foreach (var arg in args)
            {
                launchArgs.Parameters.Add(arg.Key, arg.Value);
            }

            return ShowViewModelAsync(launchArgs);
        }

        public Task NavigateAndEditAsync<TViewModel>(object parent, object child, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase
        {
            var launchArgs = new ViewModelLaunchArgs();
            launchArgs.LaunchType = LaunchTypes.Edit;
            launchArgs.ViewModelType = typeof(TViewModel);
            launchArgs.Parent = parent;
            launchArgs.Child = child;


            foreach (var arg in args)
            {
                launchArgs.Parameters.Add(arg.Key, arg.Value);
            }

            return ShowViewModelAsync(launchArgs);
        }

        public Task NavigateAndPickAsync<TViewModel>(Action<Object> selectedAction, Action cancelledAction = null, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase
        {
            var launchArgs = new ViewModelLaunchArgs();
            launchArgs.LaunchType = LaunchTypes.Picker;
            launchArgs.ViewModelType = typeof(TViewModel);

            launchArgs.SelectedAction = selectedAction;
            launchArgs.CancelledAction = cancelledAction;

            foreach (var arg in args)
            {
                launchArgs.Parameters.Add(arg.Key, arg.Value);
            }

            return ShowViewModelAsync(launchArgs);
        }

        public Task NavigateAndEditAsync<TViewModel>(object parent, string id, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase
        {
            var launchArgs = new ViewModelLaunchArgs();
            launchArgs.LaunchType = LaunchTypes.Edit;
            launchArgs.ViewModelType = typeof(TViewModel);
            launchArgs.Parent = parent;
            launchArgs.ChildId = id;

            foreach (var arg in args)
            {
                launchArgs.Parameters.Add(arg.Key, arg.Value);
            }

            return ShowViewModelAsync(launchArgs);
        }

        public Task NavigateAndEditAsync<TViewModel>(string id, params KeyValuePair<string, object>[] args) where TViewModel : ViewModelBase
        {
            var launchArgs = new ViewModelLaunchArgs();
            launchArgs.LaunchType = LaunchTypes.Edit;
            launchArgs.ViewModelType = typeof(TViewModel);
            launchArgs.ChildId = id;

            foreach (var arg in args)
            {
                launchArgs.Parameters.Add(arg.Key, arg.Value);
            }

            return ShowViewModelAsync(launchArgs);
        }
    }   
}