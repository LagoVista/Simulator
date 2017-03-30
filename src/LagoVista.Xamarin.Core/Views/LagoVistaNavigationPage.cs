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
        public LagoVistaContentPage() : base()
        {

        }

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
