using LagoVista.Core.Commanding;
using System.Threading.Tasks;

namespace LagoVista.Simulator.ViewModels
{
    public class SplashViewModel : SimulatorViewModelBase
    {

        public SplashViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        public override async Task InitAsync()
        {
            await AuthManager.LoadAsync();
            if(AuthManager.IsAuthenticated)
            {
                ViewModelNavigation.SetAsNewRoot<MainViewModel>();
            }
        }

        public void Login()
        {
            ViewModelNavigation.SetAsNewRoot<Auth.LoginViewModel>();
        }

        public RelayCommand LoginCommand { get; private set; }
    }
}