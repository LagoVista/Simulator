using LagoVista.Core.ViewModels;
using Xamarin.Forms;

namespace LagoVista.XPlat.Core.Views
{
    public class LagoVistaStartupPage : ContentPage, ILagoVistaPage
    {
        public ViewModelBase ViewModel
        {
            get { return BindingContext as ViewModelBase; }
            set { BindingContext = value; }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (ViewModel != null)
            {
                await ViewModel.InitAsync();
            }
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
