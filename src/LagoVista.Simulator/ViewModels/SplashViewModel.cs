using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels
{
    public class SplashViewModel : ViewModelBase
    {

        public SplashViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        public override async Task InitAsync()
        {
            if (await Storage.HasKVPAsync("AUTH"))
            {
                //ShowViewModel<MainViewModel>();
            }
        }

        public void Login()
        {
            ShowViewModel<Auth.LoginViewModel>();
        }

        public RelayCommand LoginCommand { get; private set; }
    }
}
